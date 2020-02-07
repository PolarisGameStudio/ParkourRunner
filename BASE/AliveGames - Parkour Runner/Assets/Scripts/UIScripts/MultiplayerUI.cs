using System;
using Managers;
using UnityEngine;

public class MultiplayerUI : MonoBehaviour {
	public static MultiplayerUI Instance;

	[SerializeField] private GameObject WaitingPanel;


	private void Awake() {
		if (!PhotonGameManager.IsMultiplayer) {
			gameObject.SetActive(false);
			return;
		}
		Instance = this;
	}


	public void HideWaitingPanel() {
		WaitingPanel.SetActive(false);
	}
}