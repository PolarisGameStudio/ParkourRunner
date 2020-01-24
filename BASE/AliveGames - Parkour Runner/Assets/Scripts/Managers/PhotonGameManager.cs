using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Managers {
	public class PhotonGameManager : MonoBehaviourPunCallbacks {
		public static List<PhotonPlayer> Players = new List<PhotonPlayer>();
		public Transform[] PedestalPositions;


		private void Start() {
			if (PlayerPrefs.GetInt(EnvironmentController.MULTIPLAYER_KEY) != 1 || !PhotonNetwork.IsConnectedAndReady) {
				return;
			}

			for (int i = 0; i < Players.Count; i++) {
				Players[i].transform.position = PedestalPositions[i].position;
			}
		}
	}
}