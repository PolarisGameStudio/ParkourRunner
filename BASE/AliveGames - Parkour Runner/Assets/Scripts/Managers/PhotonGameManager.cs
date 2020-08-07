using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ParkourRunner.Scripts.Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
using Photon.Pun;
using Photon.Realtime;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers {
	public class PhotonGameManager : MonoBehaviourPunCallbacks {
		public const int StartTimer  = 3;
		public const int FinishTimer = 30;

		public static List<PhotonPlayer> Players = new List<PhotonPlayer>();
		public static PhotonPlayer       LocalPlayer;
		public static bool IsMultiplayer =>
			PlayerPrefs.GetInt(EnvironmentController.MULTIPLAYER_KEY) == 1;
		public static bool IsConnected =>
			PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom;
		public static bool IsMultiplayerAndConnected => IsMultiplayer && IsConnected;

		public static  bool              GameIsStarted;
		public static  bool              GameEnded;
		private static PhotonGameManager Instance;

		public Transform[] StartPositions;
		public Transform[] PedestalPositions;

		private List<GameObject> FinishedPlayers = new List<GameObject>();
		private Coroutine        _finishTimerCoroutine;

		private PhotonView PhotonView;
		private int        _bank;


		private void Awake() {
			Instance   = this;
			PhotonView = GetComponent<PhotonView>();
		}


		private void Start() {
			if (!IsMultiplayerAndConnected) return;
			if (PhotonNetwork.InLobby) PhotonNetwork.LeaveLobby();

			var room = PhotonNetwork.CurrentRoom;
			var bet  = (room.CustomProperties.ContainsKey("bet")) ? (int) room.CustomProperties["bet"] : 0;
			Wallet.Instance.AddCoins(-bet, Wallet.WalletMode.InGame);

			LocalPlayer.LockCamera();

			var playerIndex = PhotonNetwork.LocalPlayer.ActorNumber;
			var startPlace  = StartPositions[playerIndex - 1];

			LocalPlayer.transform.position      = startPlace.position;
			LocalPlayer.transform.localRotation = startPlace.rotation;

			var cameraPosition = StartPositions[0].position + new Vector3(1, 0.25f, -5);
			var lookPosition   = StartPositions[0].position + new Vector3(0, 1.5f,  0);
			ParkourCamera.Instance.transform.position = cameraPosition;
			ParkourCamera.Instance.transform.LookAt(lookPosition);
		}


		private void OnDestroy() {
			Reset();
		}


		public static void AddPlayer(PhotonPlayer player) {
			if (Players.Contains(player)) return;

			Players.Add(player);
			if (player.PhotonView.IsMine) LocalPlayer = player;

			CheckReady();

			var properties = PhotonNetwork.CurrentRoom.CustomProperties;
			var bet        = (int) properties["bet"];
			Instance._bank += bet;
		}


		public void OnPlayerReadyButton() {
			LocalPlayer.PhotonView.RPC("PlayerReady", RpcTarget.All);
		}


		public void OnContinueFinishButton() {
			HUDManager.Instance.PostMortemScreen.ShowMultiplayerResultScreen();
			MultiplayerUI.Instance.SetContinueFinishButton(false);
			PhotonNetwork.LeaveRoom();
		}


		private void Reset() {
			PhotonNetwork.Disconnect();
			Players.Clear();
			LocalPlayer   = null;
			GameIsStarted = false;
			GameEnded     = false;
		}


		public override void OnPlayerLeftRoom(Player otherPlayer) {
			print($"Player {otherPlayer.NickName} left the room");
			Players.Remove(null);
			if (!GameIsStarted && !GameEnded) CheckReady();
		}


		public static void CheckReady() {
			// Выход если загрузились не все
			if (Players.Count < PhotonNetwork.CurrentRoom.PlayerCount) return;

			// Установка кнопки (вкл/выкл) в зависимости от готовности игрока
			MultiplayerUI.Instance.SetReadyButton(!LocalPlayer.Ready);

			// Выход если готовы не все
			if (!Players.All(p => p.Ready)) return;

			MultiplayerUI.Instance.HideWaitingText();
			HUDManager.Instance.FadeIn(delegate {
				GameManager.Instance.UnPause();
				LocalPlayer.UnlockCamera();
				LocalPlayer.transform.localRotation = Quaternion.identity;
				foreach (var player in Players) {
					player.PlayerCanvas.HideReady();
					player.PlayerCanvas.HideNickname();
				}

				// Установка смещения камеры в режим для бега
				HUDManager.Instance.FadeOut(delegate { LocalPlayer.StartCoroutine(Instance.RunStartTimer()); });
			});
		}


		public static int GetPlayerPosition(PhotonPlayer player = null) {
			if (!player) player = LocalPlayer;
			if (player.IsFinished) {
				return Instance.FinishedPlayers.IndexOf(player.gameObject) + 1;
			}
			var runningPlayers = Players.Where(p => !p.IsFinished);
			var playersOrder   = runningPlayers.OrderByDescending(p => p.transform.position.z);
			var position       = playersOrder.ToList().IndexOf(player ? player : LocalPlayer);
			return position + Instance.FinishedPlayers.Count + 1;
		}


		private IEnumerator RunStartTimer() {
			var timer = StartTimer;
			MultiplayerUI.Instance.ShowTimer();

			while (timer > 0) {
				MultiplayerUI.Instance.SetTimerTextValue((timer--).ToString());
				yield return new WaitForSeconds(1f);
			}

			if (PhotonNetwork.IsMasterClient) {
				PhotonView.RPC("StartGame", RpcTarget.All);
			}

			MultiplayerUI.Instance.SetTimerTextValue("GO!");
			yield return new WaitForSeconds(1f);
			MultiplayerUI.Instance.HideTimer();
		}


		[PunRPC]
		public void StartGame() {
			SendStartEvent();
			GameIsStarted = true;
			MultiplayerUI.Instance.ShowPosition();
			LocalPlayer.StartGame();
		}


		public static void OnPlayerFinish(GameObject player) {
			ParkourCamera.Instance.SetFpsCamActive(false);
			Instance.FinishedPlayers.Add(player);
			if (!PhotonNetwork.IsMasterClient) return;

			var photonPlayer = player.GetComponent<PhotonPlayer>();
			photonPlayer.IsFinished = true;
			// photonPlayer.PhotonView.RPC("Finish", RpcTarget.All);

			// Если финишировал первый игрок, то запуск обратного отсчета
			if (Instance.FinishedPlayers.Count <= 1) {
				Instance.PhotonView.RPC("RunFinishTimer", RpcTarget.All);
			}
		}


		[PunRPC]
		public void RunFinishTimer() {
			_finishTimerCoroutine = StartCoroutine(FinishTimerCoroutine());
		}


		private IEnumerator FinishTimerCoroutine() {
			print("Run finish timer");
			var timer = FinishTimer;
			MultiplayerUI.Instance.ShowTimer();

			while (timer > 0 && FinishedPlayers.Count < 3 && FinishedPlayers.Count < Players.Count) {
				MultiplayerUI.Instance.SetTimerTextValue((timer--).ToString());
				yield return new WaitForSeconds(1f);
			}

			print("End timer");
			if (PhotonNetwork.IsMasterClient) {
				PhotonView.RPC("EndGame", RpcTarget.All);
			}
		}


		[PunRPC]
		public void EndGame() {
			GameIsStarted = false;
			GameEnded     = true;

			SendFinishedEvent();

			HUDManager.Instance.FadeIn(delegate {
				ShowPedestal();
				LocalPlayer.GetComponent<ParkourThirdPersonController>().PuppetMaster.enabled = false;
				LocalPlayer.StopRun();
				LocalPlayer.PlayerCanvas.ShowNickname();
				MultiplayerUI.Instance.HidePosition();
				MultiplayerUI.Instance.HideTimer();
				if (_finishTimerCoroutine != null) StopCoroutine(_finishTimerCoroutine);

				if (PhotonNetwork.IsMasterClient) {
					SetFinishPositions();
				}
				HUDManager.Instance.FadeOut(null);
			});
		}


		public void SetFinishPositions() {
			foreach (var player in Players) {
				var position = GetPlayerPosition(player);

				player.PhotonView.RPC("SetFinishPlace", RpcTarget.All, position);

				if (position > 3) continue;
				var reward = GetReward(position);
				print($"Reward for {position} place: {reward}");

				var pedestalPlace = Instance.PedestalPositions[position - 1];
				player.PhotonView.RPC("SetPosition",      RpcTarget.All, pedestalPlace.position);
				player.PhotonView.RPC("SetLocalRotation", RpcTarget.All, pedestalPlace.rotation);
				if (reward > 0) player.PhotonView.RPC("SetReward", RpcTarget.All, reward);
			}
		}


		private int GetReward(int position) {
			var properties    = PhotonNetwork.CurrentRoom.CustomProperties;
			var bet           = (int) properties["bet"];
			var winMultiplier = 1f;

			switch (Players.Count) {
				case 0: return 0;
				case 1: return bet;
				case 2:
					switch (position) {
						case 1:
							winMultiplier = 1f;
							break;
						case 2:
							winMultiplier = 0;
							break;
					}
					break;
				case 3:
					switch (position) {
						case 1:
							winMultiplier = 0.65f;
							break;
						case 2:
							winMultiplier = 0.35f;
							break;
						case 3:
							winMultiplier = 0;
							break;
					}
					break;
				default:
					switch (position) {
						case 1:
							winMultiplier = 0.5f;
							break;
						case 2:
							winMultiplier = 0.35f;
							break;
						case 3:
							winMultiplier = 0.15f;
							break;
					}
					break;
			}
			return (int) (_bank * winMultiplier);
		}


		private void ShowPedestal() {
			print("Show pedestal");
			LocalPlayer.PlayerCanvas.ShowNickname();
			LocalPlayer.LockCamera();

			// Перемещение камеры
			var newPos  = Instance.PedestalPositions[0].position + new Vector3(0, 0.25f, -5);
			var lookPos = Instance.PedestalPositions[0].position + new Vector3(0, 1.5f,  0);
			ParkourCamera.Instance.transform.position = newPos;
			ParkourCamera.Instance.transform.LookAt(lookPos);

			MultiplayerUI.Instance.SetContinueFinishButton(true);
		}


		private void SendStartEvent() {
			var room       = PhotonNetwork.CurrentRoom;
			var players    = room.Players.Values.Select(p => p.NickName);
			var properties = PhotonNetwork.CurrentRoom.CustomProperties;
			var bet        = (int) properties["bet"];

			AppsFlyerManager.MultiplayerIsRunning(players, bet);
		}


		private void SendFinishedEvent() {
			var room       = PhotonNetwork.CurrentRoom;
			var players    = room.Players.Values.Select(p => p.NickName);
			var properties = PhotonNetwork.CurrentRoom.CustomProperties;
			var bet        = (int) properties["bet"];

			AppsFlyerManager.MultiplayerIsFinished(players, bet);
		}
	}
}