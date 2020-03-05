using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ParkourRunner.Scripts.Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers {
	public class PhotonGameManager : MonoBehaviourPunCallbacks {
		public const int StartTimer  = 3;
		public const int FinishTimer = 5;

		public static List<PhotonPlayer> Players = new List<PhotonPlayer>();
		public static PhotonPlayer       LocalPlayer;
		public static bool IsMultiplayer => PlayerPrefs.GetInt(EnvironmentController.MULTIPLAYER_KEY) == 1 &&
											PhotonNetwork.IsConnectedAndReady                              &&
											PhotonNetwork.InRoom;
		public static  bool              GameIsStarted;
		public static  bool              GameEnded;
		private static Vector3           StartCameraOffset;
		private static PhotonGameManager Instance;

		public Transform[] StartPositions;
		public Transform[] PedestalPositions;

		private List<GameObject> FinishedPlayers = new List<GameObject>();
		private Coroutine _startTimerCoroutine, _finishTimerCoroutine;

		private PhotonView PhotonView;


		private void Start() {
			if (!IsMultiplayer) return;
			Instance = this;
			PhotonView = GetComponent<PhotonView>();

			var playerIndex = PhotonNetwork.LocalPlayer.ActorNumber;
			var startPlace  = StartPositions[playerIndex - 1];

			LocalPlayer.transform.position      = startPlace.position;
			LocalPlayer.transform.localRotation = startPlace.rotation;

			StartCameraOffset = ParkourCamera.Instance.Offset;

			var newPos    = StartPositions[0].position + new Vector3(1, 0.25f, -5);
			var newOffset = newPos                     - LocalPlayer.transform.position;
			ParkourCamera.Instance.Offset = newOffset;
		}


		public static void AddPlayer(PhotonPlayer player) {
			if(Players.Contains(player)) return;

			Players.Add(player);
			if (player.PhotonView.IsMine) LocalPlayer = player;

			CheckReady();
		}


		public void OnPlayerReadyButton() {
			LocalPlayer.PhotonView.RPC("PlayerReady", RpcTarget.All);
		}


		public override void OnPlayerLeftRoom(Player otherPlayer) {
			Players.Remove(null);
			if(!GameIsStarted && !GameEnded) CheckReady();
		}


		public static void CheckReady() {
			// Выход если загрузились не все
			if(Players.Count < PhotonNetwork.CurrentRoom.PlayerCount) return;

			// Установка кнопки (вкл/выкл) в зависимости от готовности игрока
			MultiplayerUI.Instance.SetReadyButton(!LocalPlayer.Ready);

			// Выход если готовы не все
			if (!Players.All(p => p.Ready)) return;

			MultiplayerUI.Instance.HideWaitingText();
			HUDManager.Instance.FadeIn(delegate {
				// Установка смещения камеры в режим для бега
				ParkourCamera.Instance.Offset = StartCameraOffset;
				HUDManager.Instance.FadeOut(delegate {
					Instance._startTimerCoroutine = LocalPlayer.StartCoroutine(Instance.RunStartTimer());
				});
			});
		}


		public static int GetPlayerPosition(PhotonPlayer player = null) {
			if (!player) player = LocalPlayer;
			if (player.IsFinished) {
				return Instance.FinishedPlayers.IndexOf(player.gameObject) + 1;
			}
			var runningPlayers = Players.Where(p => !p.IsFinished);
			var playersOrder    = runningPlayers.OrderByDescending(p => p.transform.position.z);
			var position        = playersOrder.ToList().IndexOf(player ? player : LocalPlayer);
			return position + Instance.FinishedPlayers.Count + 1;
		}


		private IEnumerator RunStartTimer() {
			var timer = StartTimer;
			MultiplayerUI.Instance.ShowTimer();
			LocalPlayer.transform.localRotation = Quaternion.identity;

			foreach (var player in Players) {
				player.PlayerCanvas.HideReady();
				player.PlayerCanvas.HideNickname();
			}

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
			GameIsStarted = true;
			MultiplayerUI.Instance.ShowPosition();

			if(!PhotonNetwork.IsMasterClient) return;
			Players.ForEach(p => p.PhotonView.RPC("StartGame", RpcTarget.All));
		}


		public static void OnPlayerFinish(GameObject player) {
			if(!PhotonNetwork.IsMasterClient) return;

			var photonPlayer = player.GetComponent<PhotonPlayer>();
			photonPlayer.IsFinished  = true;
			photonPlayer.PhotonView.RPC("Finish", RpcTarget.All);

			Instance.FinishedPlayers.Add(player);

			// Если финишировал первый игрок, то запуск обратного отсчета
			if (Instance.FinishedPlayers.Count <= 1) {
				Instance.StartCoroutine(FinishTimerCoroutine());
			}
		}


		private static IEnumerator FinishTimerCoroutine() {
			print("Run finish timer");
			var timer = FinishTimer;
			MultiplayerUI.Instance.ShowTimer();

			while (timer > 0 && Instance.FinishedPlayers.Count < 3 && Instance.FinishedPlayers.Count < Players.Count) {
				MultiplayerUI.Instance.SetTimerTextValue((timer--).ToString());
				yield return new WaitForSeconds(1f);
			}

			print("End timer");
			EndGame();
		}


		private static void EndGame() {
			GameIsStarted = false;
			GameEnded     = true;
			LocalPlayer.StopRun();
			MultiplayerUI.Instance.HidePosition();
			MultiplayerUI.Instance.HideTimer();

			HUDManager.Instance.FadeIn(delegate {
				ShowPedestal();
				HUDManager.Instance.FadeOut(null);
			});
		}


		private static void ShowPedestal() {
			print("Show pedestal");
			var position = GetPlayerPosition(LocalPlayer);
			if (position > 3) return;

			var pedestalPlace = Instance.PedestalPositions[position - 1];
			LocalPlayer.transform.position      = pedestalPlace.position;
			LocalPlayer.transform.localRotation = pedestalPlace.rotation;
			print($"Set position {pedestalPlace.position}");
			print($"Set local rotation {pedestalPlace.rotation}");

			LocalPlayer.PlayerCanvas.ShowNickname();

			// Перемещение камеры
			var newPos    = Instance.PedestalPositions[0].position + new Vector3(1, 0.25f, -5);
			ParkourCamera.Instance.transform.position = newPos;
			ParkourCamera.Instance.transform.LookAt(Instance.FinishedPlayers[0].transform);
		}
	}
}