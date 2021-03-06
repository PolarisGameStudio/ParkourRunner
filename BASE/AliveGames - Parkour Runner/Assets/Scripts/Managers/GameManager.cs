﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.ParkourRunner.Scripts.Track.Pick_Ups.Bonuses;
using ParkourRunner.Scripts.Player;
using ParkourRunner.Scripts.Player.InvectorMods;
using ParkourRunner.Scripts.Track.Generator;
using ParkourRunner.Scripts.Track.Pick_Ups.Bonuses;
using RootMotion.Dynamics;
using UnityEngine;
using AEngine;
using AppsFlyerSDK;
using Managers;
using Photon.Pun;

namespace ParkourRunner.Scripts.Managers {
	public class GameManager : MonoBehaviour {
		public event Action OnDie;

		public enum GameState {
			Run,
			Pause,
			Dead
		}


		#region Singleton

		public static GameManager  Instance;
		private       AudioManager _audio;


		private void Awake() {
			if (Instance == null) {
				Instance = this;

				this.ActiveBonuses = new List<BonusName>();
				_wallet            = Wallet.Instance;
				_audio             = AudioManager.Instance;
			}
			else {
				Destroy(gameObject);
			}
		}

		#endregion


		public List<BonusName> ActiveBonuses { get; set; }

		public MuscleDismember[] Limbs;

		public GameState gameState { get; private set; }

		public bool
			PlayerCanBeDismembered =
				true; //Можно ли оторвать конечность? Используется скриптом на каждой кости рэгдолла.

		public float VelocityToDismember = 10f;

		public  float MaxCoinsSoundPitch  = 1.25f;
		public  float CoinsSoundPitchStep = 0.01f;
		private float _coinsSoundPitch    = 1f;
		private Coroutine _resetCoinSoundPitchCoroutine;

		public  int   CoinMultipiler      = 1;
		public  float TrickMultipiler     = 1;

		public float GameSpeed = 1f;

		public int ReviveCost {
			get {
				float distance           = Mathf.Clamp(((int) DistanceRun / 10), 1f, Mathf.Infinity);
				int   cost               = (StaticConst.InitialReviveCost + (int) distance / 10) * (_revives + 1);
				float distanceCostFactor;

				if (distance <= 50)
					distanceCostFactor = 0.18f;
				else if (distance <= 100)
					distanceCostFactor = 0.15f;
				else
					distanceCostFactor = 0.12f;

				cost = StaticConst.InitialReviveCost +
						Mathf.RoundToInt((StaticConst.ReviveGrowCost + distance * distanceCostFactor) * (_revives + 1));

				return Mathf.RoundToInt(Mathf.Clamp(cost, StaticConst.InitialReviveCost, Mathf.Infinity));
			}
		}

		public bool IsLevelComplete { get; set; }

		public  float DistanceRun;
		private float _distanceRunOffset; //TODO origin reset

		private int _revives; //Сколько раз игрок возрождался за этот забег?

		[SerializeField] private float _restoreImmuneDuration;

		////Состояние
		//Наличие конечностей
		[SerializeField] private bool _leftHand  = true;
		[SerializeField] private bool _rightHand = true;
		[SerializeField] private bool _leftLeg   = true;
		[SerializeField] private bool _rightLeg  = true;

		private ParkourThirdPersonController _player;
		private Animator                     _playerAnimator;

		private HUDManager _hud;
		private Wallet     _wallet;

		public float RestoreImmuneDuration {
			get { return _restoreImmuneDuration; }
		}

		public bool IsOneLeg {
			get { return !_leftLeg || !_rightLeg; }
		}


		private void Start() {
			FindObjectOfType<BehaviourPuppet>().onLoseBalance.unityEvent.AddListener(ResetSpeed);

			_hud            = HUDManager.Instance;
			Limbs           = FindObjectsOfType<MuscleDismember>();
			_player         = ParkourThirdPersonController.instance;
			_playerAnimator = _player.GetComponent<Animator>();

			StartGame();

			_audio.LoadAudioBlock(AudioBlocks.Game);
			_audio.PlayMusic();
		}


		private void OnDisable() {
			ProgressManager.SaveSettings();
		}


		private void StartGame() {
			if (PhotonGameManager.IsMultiplayerAndConnected) {
				gameState = GameState.Pause;
			}
			else {
				if (EnvironmentController.CurrentMode == GameModes.Levels) {
					var level = PlayerPrefs.GetInt(EnvironmentController.LEVEL_KEY);
					AppsFlyerManager.StartLevel(level);
				}
				gameState = GameState.Run;
			}

			IsLevelComplete = false;
			StartCoroutine(IncreaseGameSpeed());
		}


		// Update is called once per frame
		void Update () {
			if (_player) {
				DistanceRun = _player.transform.position.z;
				_hud.UpdateDistance(DistanceRun + _distanceRunOffset);
			}
		}


		//Оторвать конечность (или приклеить обратно)
		public void SetLimbState(Bodypart bodypart, bool dismember) {
			switch (bodypart) {
				case (Bodypart.Body): //or
				case (Bodypart.Head):
					if (!dismember) {
						print("HEAD DIE");
						Die();
					}
					else {
						ParkourCamera.Instance.OnHeadRegenerated();
					}
					break;

				case (Bodypart.LHand):
					_leftHand = dismember;
					break;

				case (Bodypart.RHand):
					_rightHand = dismember;
					break;

				case (Bodypart.RLeg):
					_rightLeg = dismember;
					break;

				case (Bodypart.LLeg):
					_leftLeg = dismember;
					break;
			}

			_playerAnimator.SetBool("LeftHand",  _leftHand);
			_playerAnimator.SetBool("RightHand", _rightHand);
			_playerAnimator.SetBool("LeftLeg",   _leftLeg);
			_playerAnimator.SetBool("RightLeg",  _rightLeg);

			if (!dismember)
				CheckLimbLost(); //Смотрим не проиграли ли мы ещё
		}


		private void CheckLimbLost() {
			if (!_leftHand && !_rightHand) {
				Die();
			}
			else if (!_leftLeg && !_rightLeg) {
				Die();
			}
		}


		private void Die() {
			if (gameState != GameState.Dead) {
				gameState = GameState.Dead;
				_player.Die();
				OnDie.SafeInvoke();

				this.ActiveBonuses.Clear();
				_player.Immune        = false;
				_player.RestoreImmune = false;

				if (PhotonGameManager.IsMultiplayer) {
					Invoke("Revive", 2f);
				}
				else {
					Invoke("ShowPostMortem", 4f);
				}

				_audio.PlaySound(Gender.Kind == Gender.GenderKinds.Male ? Sounds.Death : Sounds.DeathFem);
			}
		}


		public void CompleteLevel() {
			this.ActiveBonuses.Clear();
			_player.Immune        = false;
			_player.RestoreImmune = false;
			this.IsLevelComplete  = true;

			if (EnvironmentController.CurrentMode == GameModes.Tutorial) {
				AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.complete_tutorial);
			}
			else if (EnvironmentController.CurrentMode == GameModes.Levels) {
				var level = PlayerPrefs.GetInt(EnvironmentController.LEVEL_KEY);
				var coins = Wallet.Instance.InGameCoins;

				AppsFlyerManager.LevelComplete(level, coins);
			}
		}


		public void ShowPostMortem() {
			_hud.ShowPostMortem();
		}


		public void Revive() {
			var revivePos = LevelGenerator.Instance.GetRevivePosition();

			_player.PuppetMaster.enabled    = false; //mode = PuppetMaster.Mode.Disabled;
			_player.transform.root.position = revivePos;
			_player.transform.position      = revivePos;
			_player.PuppetMaster.enabled    = true; //.mode = PuppetMaster.Mode.Kinematic;

			//Heal player
			gameState = GameState.Run;
			HealFull();
			_player.Revive();

			_revives++;
		}


		private void HealFull() {
			while (HealLimb()) ; //Heal while theres limbs
		}


		public bool HealLimb() //public чтобы лечиться с heal бонуса
		{
			foreach (var limb in Limbs) {
				if (limb.IsDismembered) {
					limb.HealRecursive();
					SetLimbState(limb.Bodypart, true); //Записываем в аниматор что подлечились
					return true;
				}
			}
			return false;
		}


		public void DoTrick(Trick trick) {
			AddCoin((int) (trick.MoneyReward * CoinMultipiler * TrickMultipiler));
			HUDManager.Instance.ShowTrickReward(trick, TrickMultipiler * CoinMultipiler);
		}


		public void AddCoin(int amount = 1) {
			_wallet.AddCoins(amount * CoinMultipiler, Wallet.WalletMode.InGame);
			_audio.PlaySound(Sounds.Coin, false, _coinsSoundPitch);

			_coinsSoundPitch = _coinsSoundPitch < MaxCoinsSoundPitch
				? _coinsSoundPitch + CoinsSoundPitchStep
				: MaxCoinsSoundPitch;

			if(_resetCoinSoundPitchCoroutine != null) StopCoroutine(_resetCoinSoundPitchCoroutine);
			_resetCoinSoundPitchCoroutine = StartCoroutine(ResetCoinSoundPitch());
		}


		public IEnumerator ResetCoinSoundPitch() {
			yield return new WaitForSeconds(2f);
			_coinsSoundPitch = 1f;
		}


		public void AddBonus(BonusName bonusName) {
			switch (bonusName) {
				case (BonusName.Magnet):
					GetComponent<MagnetBonus>().RefreshTime();
					break;
				case (BonusName.Jump):
					GetComponent<JumpBonus>().RefreshTime();
					break;
				case (BonusName.Shield):
					GetComponent<ShieldBonus>().RefreshTime();
					break;
				case (BonusName.DoubleCoins):
					GetComponent<DoubleCoinsBonus>().RefreshTime();
					break;
				case (BonusName.Boost):
					GetComponent<BoostBonus>().RefreshTime();
					break;
			}

			PlayBonusSound(bonusName);
		}


		private void PlayBonusSound(BonusName bonusName) {
			_audio = AudioManager.Instance;

			switch (bonusName)
			{
				case BonusName.DoubleCoins:
					_audio.PlaySound(Sounds.BonusX2);
					break;

				case BonusName.Shield:
					_audio.PlaySound(Sounds.Bonus);
					_audio.PlaySound(Sounds.BonusShield);
					break;

				default:
					_audio.PlaySound(Sounds.Bonus);
					break;
			}
		}


		public Transform GetRandomLimb() {
			var list = Limbs.ToList().Where(x => !x.IsDismembered).ToList();
			return list[UnityEngine.Random.Range(0, list.Count())].transform;
		}


		private IEnumerator IncreaseGameSpeed() {
			while (true) {
				if (gameState == GameState.Run) {
					GameSpeed += StaticConst.SpeedGrowPerSec * Time.deltaTime;
					GameSpeed =  Math.Min(GameSpeed, StaticConst.MaxGameSpeed);
				}
				yield return null;
			}
		}


		public void ResetSpeed() {
			GameSpeed = 1f;
		}


		public void OnHeadLost(Transform head) {
			ParkourCamera.Instance.OnHeadLost(head); //todo
		}


		public void Pause() {
			gameState = GameState.Pause;
			ParkourSlowMo.Instance.SmoothlyStopTime();
		}


		public void UnPause() {
			//gameState = GameState.Run;
			ParkourSlowMo.Instance.SmoothlyContinueTime();
			StartCoroutine(UnpauseProcess());
		}


		private IEnumerator UnpauseProcess() {
			yield return new WaitForSeconds(0.3f);
			gameState = GameState.Run;
		}
	}
}