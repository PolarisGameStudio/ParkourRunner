using System;
using System.Linq;
using Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
using Photon.Pun;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour {
	[SerializeField] private GameObject[] TurnOffObjects;
	[SerializeField] private Behaviour[]  TurnOffComponents;
	[SerializeField] private ParkourThirdPersonInput PlayerInput;
	[SerializeField] private PhotonView PhotonView;


	private void Awake() {
		if (PlayerPrefs.GetInt(EnvironmentController.MULTIPLAYER_KEY) != 1 || !PhotonNetwork.IsConnectedAndReady) {
			PlayerInput.LockRunning = false;
			return;
		}

		if (!PhotonView.IsMine) {
			TurnOffObjects.ToList().ForEach(o => o.SetActive(false));
			TurnOffComponents.ToList().ForEach(o => o.enabled = false);
		}

		PhotonGameManager.Players.Add(this);
	}
}