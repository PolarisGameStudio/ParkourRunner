using System;
using System.Collections.Generic;
using System.Linq;
using MainMenuAndShop.Jetpacks;
using Managers;
using ParkourRunner.Scripts.Player;
using ParkourRunner.Scripts.Player.InvectorMods;
using Photon.Pun;
using Photon.Realtime;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.Serialization;

public class PhotonPlayer : MonoBehaviourPunCallbacks {
	[HideInInspector] public PlayerCanvas  PlayerCanvas;
	[HideInInspector] public PhotonView    PhotonView;
	[HideInInspector] public BotController BotController;
	[HideInInspector] public bool          IsFinished;
	[HideInInspector] public int           FinishPlace;
	[HideInInspector] public bool          Ready;
	[HideInInspector] public string        UserId;

	public bool IsBot;

	private ParkourThirdPersonInput _playerInput;
	private Animator                _animator;
	private Transform               _transform;

	private Vector3    _freezePosition;
	private Quaternion _freezeRotation;
	private bool       _freeze;


	private void Awake() {
		PhotonView = GetComponent<PhotonView>();
		if (!PhotonGameManager.IsMultiplayerAndConnected) return;

		GetComponents();
		if (!PhotonView.IsMine) {
			DestroyComponents();
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
		}

		StopRun();
	}


	private void Start() {
		if (!PhotonGameManager.IsMultiplayerAndConnected) return;
		PhotonGameManager.AddPlayer(this);
		if (!IsBot) {
			PlayerCanvas.SetNickname(PhotonView.IsMine ? "(You)" : PhotonView.Owner.NickName);
			if (PhotonView.IsMine) {
				transform.parent.GetComponentInChildren<HeadDismember>().ActivateHelmet();
				GetComponent<JetpackController>().CreateJetpack(Jetpacks.GetActiveJetpack());
			}
		}
	}


	private void GetComponents() {
		_playerInput  = GetComponent<ParkourThirdPersonInput>();
		_animator     = GetComponent<Animator>();
		_transform    = transform;
		BotController = GetComponent<BotController>();

		for (int i = 0; i < _transform.childCount; i++) {
			var child = _transform.GetChild(i);
			if (child.name.Equals("MultiplayerCanvas")) {
				PlayerCanvas            = child.GetComponent<PlayerCanvas>();
				PlayerCanvas.PhotonView = PhotonView;
				UserId                  = PhotonView.Owner.UserId;
				break;
			}
		}
	}


	private void Update() {
		if (!PhotonView.IsMine) return;

		if (Input.GetKeyDown(KeyCode.Backspace)) {
			var controller = GetComponent<ParkourThirdPersonController>();
			if (controller.PuppetMaster.state == PuppetMaster.State.Alive) controller.Die();
			else controller.Revive();
		}
		else if (Input.GetKeyDown(KeyCode.P)) {
			_playerInput.LockRunning = !_playerInput.LockRunning;
		}

		if (_freeze) {
			_transform.position      = _freezePosition;
			_transform.localRotation = _freezeRotation;
		}
	}


	public void DestroyComponents() {
		var parent = _transform.parent;

		var childs = new List<Transform>();
		for (int i = 0; i < parent.childCount; i++) {
			childs.Add(parent.GetChild(i));
		}
		childs.Where(c => c.name == "Behaviours" || c.name == "PuppetMaster").ToList()
			.ForEach(c => Destroy(c.gameObject));

		Destroy(GetComponent<ParkourThirdPersonController>());
		Destroy(GetComponent<ParkourThirdPersonInput>());
		Destroy(GetComponent<GenericActionPlusPuppet>());
		Destroy(GetComponent<InvectorPlusPuppet>());
		Destroy(GetComponent<CharacterEffects>());
		Destroy(GetComponent<JetpackController>());
		Destroy(_transform.parent.GetComponent<ExtremlyReloader>());

		_animator.applyRootMotion = false;
	}


	public void StopRun() {
		_playerInput.Stop();
	}


	public void StopInput() {
		_playerInput.lockInput = true;
	}


	public void StartRun() {
		_playerInput.LockRunning = false;
		_playerInput.lockInput   = false;
	}


	public override void OnPlayerLeftRoom(Player otherPlayer) {
		var player = PhotonGameManager.Players.FirstOrDefault(p => p.UserId == otherPlayer.UserId);
		if (!player) return;

		PhotonGameManager.Players.Remove(player);
		Destroy(player.gameObject);
	}


	[PunRPC]
	public virtual void PlayerReady() {
		print($"Player {gameObject.name} is ready");
		PlayerCanvas.PlayerReady();
		Ready = true;

		PhotonGameManager.CheckReady();
	}


	[PunRPC]
	public virtual void StartGame() {
		if (!PhotonView.IsMine) return;

		var startPosition = ParkourThirdPersonController.instance.StartPosition;
		startPosition.x          = _transform.position.x;
		_transform.position      = startPosition;
		_transform.localRotation = Quaternion.identity;
		StartRun();
	}


	[PunRPC]
	public void PlayAnimation(string animationName) {
		_animator.CrossFadeInFixedTime(animationName, 0.1f);
	}


	[PunRPC]
	public void Die() {
		_animator.SetBool("isDead", true);
	}


	[PunRPC]
	public void LoseBalance() {
		_animator.SetBool("isDead", true);
	}


	[PunRPC]
	public void RegainBalance() {
		_animator.SetBool("isDead", false);
	}


	/*[PunRPC]
	public void Finish() {
		if (PhotonView.IsMine) {
			StopInput();
		}
	}*/


	[PunRPC]
	public virtual void SetPosition(Vector3 position) {
		if (!PhotonView.IsMine) return;

		_transform.position = position;
	}


	[PunRPC]
	public virtual void SetLocalRotation(Quaternion rotation) {
		if (!PhotonView.IsMine) return;
		_transform.localRotation = rotation;
	}


	[PunRPC]
	public virtual void FreezePosition(Vector3 position, Quaternion rotation) {
		if (!PhotonView.IsMine) return;

		_freezePosition = position;
		_freezeRotation = rotation;
		_freeze         = true;

		GetComponent<Rigidbody>().isKinematic = true;
	}


	// [PunRPC]
	public void LockCamera() {
		ParkourCamera.Instance.LockCamera = true;
	}


	// [PunRPC]
	public void UnlockCamera() {
		ParkourCamera.Instance.LockCamera = false;
	}


	[PunRPC]
	public virtual void SetReward(int reward) {
		PlayerCanvas.SetReward(reward);
		if (PhotonView.IsMine) {
			Wallet.Instance.AddCoins(reward, Wallet.WalletMode.InGame);
		}
	}


	[PunRPC]
	public virtual void SetFinishPlace(int place) {
		FinishPlace = place;
	}


	[PunRPC]
	public void SetIsBot(string botName, int botIndex) {
		IsBot  = true;
		UserId = botName;

		DestroyComponents();

		BotController.ActivateBot(botIndex);
		PlayerCanvas.SetNickname(botName);
		PlayerCanvas.SetNickname(botName);
	}
}