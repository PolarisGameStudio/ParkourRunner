using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerUI : MonoBehaviour {
	public static MultiplayerUI Instance;

	[SerializeField] private GameObject WaitingText;
	[SerializeField] private GameObject ReadyButton;
	[SerializeField] private GameObject ContinueFinishButton;
	[SerializeField] private Text       Timer;
	[SerializeField] private Text       PositionText;


	private void Awake() {
		Instance = this;
		if (!PhotonGameManager.IsMultiplayer) {
			gameObject.SetActive(false);
			return;
		}

		ShowWaitingText();
	}


	private void Update() {
		if (PhotonGameManager.GameIsStarted) {
			var playersCount = PhotonGameManager.Players.Count;
			var position = PhotonGameManager.GetPlayerPosition();
			var text = $"{position}/{playersCount}";
			PositionText.text = text;
		}
	}


	public void ShowWaitingText() {
		WaitingText.SetActive(true);
	}


	public void HideWaitingText() {
		WaitingText.SetActive(false);
	}


	public void SetReadyButton(bool isOn) {
		ReadyButton.SetActive(isOn);
	}


	public void SetContinueFinishButton(bool isOn) {
		ContinueFinishButton.SetActive(isOn);
	}


	public void ShowTimer() {
		Timer.gameObject.SetActive(true);
	}


	public void HideTimer() {
		Timer.gameObject.SetActive(false);
	}


	public void ShowPosition() {
		PositionText.gameObject.SetActive(true);
	}


	public void HidePosition() {
		PositionText.gameObject.SetActive(false);
	}


	public void SetTimerTextValue(string val) {
		Timer.text = val;
	}
}