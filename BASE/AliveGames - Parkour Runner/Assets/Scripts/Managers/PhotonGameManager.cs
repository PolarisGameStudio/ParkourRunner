using System;
using System.Collections.Generic;
using System.Linq;
using ParkourRunner.Scripts.Player.InvectorMods;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Managers {
	public class PhotonGameManager : MonoBehaviourPunCallbacks {
		public static List<PhotonPlayer> Players = new List<PhotonPlayer>();
		public static PhotonPlayer       LocalPlayer;
		public static bool IsMultiplayer => PlayerPrefs.GetInt(EnvironmentController.MULTIPLAYER_KEY) == 1 &&
											PhotonNetwork.IsConnectedAndReady                              &&
											PhotonNetwork.InRoom;
		private static Vector3 StartCameraOffset;

		public Transform[] PedestalPositions;


		private void Start() {
			if (!IsMultiplayer) return;

			var playerIndex = PhotonNetwork.LocalPlayer.ActorNumber;
			print($"Actor number: {playerIndex}");

			LocalPlayer.transform.position         = PedestalPositions[playerIndex - 1].position;
			LocalPlayer.transform.localEulerAngles = new Vector3(0, 180, 0);

			StartCameraOffset = ParkourCamera.Instance.Offset;

			var newPos = PedestalPositions[0].position + new Vector3(1, 0.25f, -5);
			var newOffset = newPos - LocalPlayer.transform.position;
			ParkourCamera.Instance.Offset = newOffset;
		}


		public void OnPlayerReadyButton() {
			MultiplayerUI.Instance.HideWaitingPanel();
			LocalPlayer.PhotonView.RPC("PlayerReady", RpcTarget.All);
		}


		public static void CheckReady() {
			if (!Players.All(p => p.Ready)) return;

			ParkourCamera.Instance.Offset = StartCameraOffset;
			LocalPlayer.StartGame();
		}


		public override void OnPlayerLeftRoom(Player otherPlayer) {
			Players.Remove(null);
		}
	}
}