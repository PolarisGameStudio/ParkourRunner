using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BotWaypoints;
using MainMenuAndShop.Jetpacks;
using Managers;
using ParkourRunner.Scripts.Managers;
using ParkourRunner.Scripts.Player;
using ParkourRunner.Scripts.Track.Generator;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotController : MonoBehaviour {
	private const float JumpForce               = 200f;
	private const float BigJumpForce            = 250f;
	private const float SmallPlatformJumpForce  = 500f;
	private const float MiddlePlatformJumpForce = 600f;
	private const float HighPlatformJumpForce   = 750f;

	private const float NextWaypointDistance  = 0.5f;
	private const float FallTime              = 1.25f;
	private const float FallChance            = 0.25f;
	private const float FallUnderGroundHeight = -10f;
	private const float MaxStoppingRunTimer   = 2f;
	private const float MinSpeed              = 2f;

	private static readonly int _inputMagnitude = Animator.StringToHash("InputMagnitude");

	[SerializeField] private GameObject[] Helmets;

	private PhotonPlayer _player;
	private Animator     _animator;
	private Transform    _transform;
	private Rigidbody    _rigidbody;

	private int _botIndex        = -1;
	private int _blockLevelIndex = -1;
	private int _waypointIndex   = 0;

	private Dictionary<int, List<SavedWaypoint>> Level = new Dictionary<int, List<SavedWaypoint>>();

	private Coroutine _fallCoroutine, _respawnCoroutine;
	private Vector3   _velocity;
	private float     _stoppingRunTime;


	private void Awake() {
		print("Bot controller awaking");
		_player    = GetComponent<PhotonPlayer>();
		_animator  = GetComponent<Animator>();
		_rigidbody = GetComponent<Rigidbody>();
		_transform = transform;
	}


	public void ActivateBot(int botIndex) {
		print($"activate bot {botIndex}");

		_botIndex              = botIndex;
		print("setup rigidbody");
		_rigidbody.isKinematic = false;

		print("set tag");
		gameObject.tag = "Bot Player";
		print("add listener to LevelGenerator");
		LevelGenerator.LevelUpdate += OnLevelUpdate;

		print("setup animator");
		_animator.SetFloat(_inputMagnitude, 0f);
		print("Spawn helmet");
		SpawnRandomHelmet();

		print("check player is mine");
		if (_player.photonView.IsMine) {
			_player.PhotonView.RPC(nameof(PhotonPlayer.PlayerReady), RpcTarget.All);
			print("Spawn jetpack");
			SpawnRandomJetpack();
		}
	}


	private void OnDestroy() {
		LevelGenerator.LevelUpdate -= OnLevelUpdate;
	}


	public void FixedUpdate() {
		if (!_player.PhotonView.IsMine || _botIndex == -1) return;
		_animator.SetFloat(_inputMagnitude, 0f);
		if (!PhotonGameManager.GameIsStarted || PhotonGameManager.GameEnded) return;

		WaypointsProcess();
		CheckFalling();
		CheckStoppingRun();
	}


	private void OnLevelUpdate() {
		var activeBlocks = LevelGenerator.Instance.BlockPool;

		foreach (var block in activeBlocks) {
			var blockId = block.GetInstanceID();
			if (Level.ContainsKey(blockId)) continue;

			if (block.WaypointLines.Count <= _botIndex) {
				Debug.LogError($"No ways for block {block.name}");
				continue;
			}
			var waypointsLine = block.WaypointLines[_botIndex];
			var waypoints     = waypointsLine.Waypoints.Select(w => new SavedWaypoint(w.transform.position, w.WaypointType, w.CanFall)).ToList();

			Level.Add(blockId, waypoints);
		}

		// Если бот отстал, а блок на котором был следующий WP удалился, то его ТП на ближайший блок
		if (_blockLevelIndex >= 0) {
			var activeBlocksId = activeBlocks.Select(b => b.GetInstanceID()).ToList();
			if (!activeBlocksId.Contains(_blockLevelIndex)) {
				Respawn();
			}
		}

		if (Level.Count      == 0) return;
		if (_blockLevelIndex == -1) _blockLevelIndex = Level.Keys.ToList()[0];
	}


	private void ResetWaypoints() {
		// Получение списка активных блоков
		var activeBlocks = LevelGenerator.Instance.BlockPool;
		// Получение id первого блока
		_blockLevelIndex = activeBlocks[0].GetInstanceID();
		var waypointsLine = Level[_blockLevelIndex];
		var playerPosZ    = _transform.position.z;
		// Получение списка точек перед ботом
		var nearWaypoints = waypointsLine.Where(w => w.Position.z > playerPosZ).ToList();
		if (!nearWaypoints.Any()) { // Если нет таких точек, то выбираем последнюю
			_waypointIndex = waypointsLine.Count - 1;
			return;
		}

		// Выбираем ближайщую точку
		var nearWaypoint = waypointsLine[0];
		foreach (var waypoint in nearWaypoints) {
			if (waypoint.Position.z > nearWaypoint.Position.z) continue;
			nearWaypoint = waypoint;
		}
		_waypointIndex = waypointsLine.IndexOf(nearWaypoint);
	}


	private void WaypointsProcess() {
		if (_blockLevelIndex == -1) {
			// print("_blockLevelIndex == -1");
			return;
		}

		if (_fallCoroutine != null) return;
		var waypointsLine = Level[_blockLevelIndex];

		if (_waypointIndex >= waypointsLine.Count - 1) {
			var levelKeys      = Level.Keys.ToList();
			var levelKeyIndex  = levelKeys.IndexOf(_blockLevelIndex);
			var nextLevelIndex = levelKeyIndex + 1;
			if (nextLevelIndex >= levelKeys.Count) {
				// print("Not found next block");
				return;
			}
			// print("Переключение на следующий блок");
			_blockLevelIndex = levelKeys[nextLevelIndex];
			_waypointIndex   = 0;
			WaypointsProcess();
			return;
		}
		var waypoint     = waypointsLine[_waypointIndex];
		var zBotPosition = _transform.position.z;
		if (zBotPosition > waypoint.Position.z) {
			// print("Пробежал дальше точки");
			_waypointIndex++;
			WaypointsProcess();
			return;
		}

		if (waypoint.Position.z - zBotPosition < NextWaypointDistance) {
			// print("Почти подбежал к точке");
			_waypointIndex++;
			if (waypoint.Type != Waypoint.Type.Moving && !waypoint.IsPlatform && waypoint.CanFall) {
				var randomFall = Random.value;
				if (randomFall < FallChance) {
					Fall();
					return;
				}
			}

			if (waypoint.Type == Waypoint.Type.Jump) {
				Jump(JumpForce);
			}
			else if (waypoint.Type == Waypoint.Type.BigJump) {
				Jump(BigJumpForce);
			}
			else if (waypoint.IsPlatform) {
				var t = waypoint.Type;
				var force = t == Waypoint.Type.SmallPlatformJump  ? SmallPlatformJumpForce :
					t         == Waypoint.Type.MiddlePlatformJump ? MiddlePlatformJumpForce :
					t         == Waypoint.Type.HighPlatformJump   ? HighPlatformJumpForce : 0f;
				PlatformJump(force);
			}
			else if (waypoint.Type == Waypoint.Type.Roll) {
				Roll();
			}
		}

		MoveToWaypoint(waypoint);
	}


	private void Jump(float force) {
		var animName = "JumpMove";
		_rigidbody.AddForce(_transform.up * force, ForceMode.Impulse);
		_animator.CrossFadeInFixedTime(animName, .2f);
		_player.PhotonView.RPC(nameof(PhotonPlayer.PlayAnimation), RpcTarget.Others, animName);
	}


	private void PlatformJump(float force) {
		var animName = "JumpMove";
		_rigidbody.AddForce(_transform.up * force, ForceMode.Impulse);
		_animator.CrossFadeInFixedTime(animName, .2f);
		_player.PhotonView.RPC(nameof(PhotonPlayer.PlayAnimation), RpcTarget.Others, animName);
	}


	private void Roll() {
		string randomRoll = RandomTricks.GetRoll();
		_animator.CrossFadeInFixedTime(randomRoll, 0.1f);
		_player.PhotonView.RPC(nameof(PhotonPlayer.PlayAnimation), RpcTarget.Others, randomRoll);
	}


	private void Fall() {
		if (_fallCoroutine != null) return;
		_fallCoroutine = StartCoroutine(FallCoroutine());
	}


	private IEnumerator FallCoroutine() {
		_player.PhotonView.RPC(nameof(PhotonPlayer.LoseBalance), RpcTarget.All);
		yield return  new WaitForSeconds(FallTime);
		_player.PhotonView.RPC(nameof(PhotonPlayer.RegainBalance), RpcTarget.All);
		yield return  new WaitForSeconds(FallTime);
		_fallCoroutine = null;
	}


	private void Respawn() {
		if (_respawnCoroutine != null) return;
		_respawnCoroutine = StartCoroutine(RespawnCoroutine());
	}


	private IEnumerator RespawnCoroutine() {
		_player.PhotonView.RPC(nameof(PhotonPlayer.LoseBalance), RpcTarget.All);
		yield return  new WaitForSeconds(FallTime);
		_player.PhotonView.RPC(nameof(PhotonPlayer.RegainBalance), RpcTarget.All);
		_transform.position = LevelGenerator.Instance.GetRevivePosition();
		ResetWaypoints();
		yield return  new WaitForSeconds(FallTime);
		_respawnCoroutine = null;
	}


	private void FastRespawn() {
		_transform.position = LevelGenerator.Instance.GetRevivePosition();
		ResetWaypoints();
	}


	private void MoveToWaypoint(SavedWaypoint waypoint) {
		// print($"move to wp {waypoint.Position}");
		var playerPosition = _transform.position;
		var direction      = (waypoint.Position - playerPosition).normalized;
		var targetSpeed    = StaticConst.MinRunSpeed * GameManager.Instance.GameSpeed;
		var velocity       = direction               * targetSpeed;

		var rbVelocity     = _rigidbody.velocity;
		var smoothVelocity = Vector3.Slerp(rbVelocity, velocity, Time.fixedDeltaTime * StaticConst.MaxRunSpeed);
		var speed          = new Vector2(smoothVelocity.x, smoothVelocity.z).magnitude;
		var maxSpeed       = StaticConst.MinRunSpeed * StaticConst.MaxGameSpeed;
		var animSpeed      = speed / maxSpeed        * StaticConst.MaxAnimSpeed;
		// print($"velocity: {smoothVelocity}, speed: {speed}, maxSpeed: {maxSpeed}, animSpeed: {animSpeed}");
		_animator.SetFloat(_inputMagnitude, animSpeed);
		_rigidbody.velocity = new Vector3(smoothVelocity.x, rbVelocity.y, smoothVelocity.z);
		// _transform.LookAt(waypoint.Position);
		var lookPos = waypoint.Position - playerPosition;
		lookPos.y = 0;
		var rotation = Quaternion.LookRotation(lookPos);
		_transform.rotation = Quaternion.Slerp(_transform.rotation, rotation, Time.fixedDeltaTime * 5f);
	}


	private void CheckFalling() {
		if (_transform.position.y > FallUnderGroundHeight) return;
		FastRespawn();
	}


	private void CheckStoppingRun() {
		if (_blockLevelIndex == -1 || _waypointIndex >= Level[_blockLevelIndex].Count - 1 || _fallCoroutine != null || !PhotonGameManager.GameIsStarted ||
			PhotonGameManager.GameEnded) {
			_stoppingRunTime = 0f;
			return;
		}

		var horizontalVelocity = _rigidbody.velocity;
		horizontalVelocity.y = 0;
		var speed = horizontalVelocity.magnitude;

		if (speed < MinSpeed) {
			_stoppingRunTime += Time.fixedDeltaTime;
			if (_stoppingRunTime > MaxStoppingRunTimer) {
				Respawn();
				_stoppingRunTime = 0f;
			}
		}
		else {
			_stoppingRunTime = 0f;
		}
	}


	private void SpawnRandomHelmet() {
		var helmets = Enum.GetNames(typeof(MainMenuAndShop.Helmets.Helmets.HelmetsType));
		var helmetIndex = Random.Range(0, helmets.Length);
		if(helmetIndex == 0) return;

		Helmets[helmetIndex - 1].SetActive(true);
	}


	private void SpawnRandomJetpack() {
		var jets = Enum.GetNames(typeof(Jetpacks.JetpacksType));
		var jetIndex = Random.Range(0, jets.Length);
		if(jetIndex == 0) return;

		var jetpackController = GetComponent<JetpackController>();
		jetpackController.CreateJetpack(Jetpacks.GetJetpackData((Jetpacks.JetpacksType) jetIndex));
		Destroy(jetpackController);
	}


	private class SavedWaypoint {
		public Vector3       Position;
		public bool          CanFall;
		public Waypoint.Type Type;

		public bool IsPlatform => Type == Waypoint.Type.SmallPlatformJump || Type == Waypoint.Type.MiddlePlatformJump || Type == Waypoint.Type.HighPlatformJump;


		public SavedWaypoint(Vector3 position, Waypoint.Type type, bool canFall) {
			Position = position;
			Type     = type;
			CanFall  = canFall;
		}
	}
}