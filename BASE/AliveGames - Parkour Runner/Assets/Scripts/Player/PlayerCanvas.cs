using System;
using Managers;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour {
	[HideInInspector] public PhotonView PhotonView;

	[SerializeField] private Canvas     Canvas;
	[SerializeField] private Text       Nickname;
	[SerializeField] private GameObject Reward;
	[SerializeField] private Text       RewardValue;
	[SerializeField] private GameObject LoadedText, ReadyText;


	private void Awake() {
		if (!PhotonGameManager.IsMultiplayerAndConnected) return;

		Canvas.worldCamera = Camera.current;
		Nickname.gameObject.SetActive(true);

		if (!PhotonView.IsMine) LoadedText.SetActive(true);
	}


	public void SetNickname(string nickname) {
		Nickname.text = nickname;
	}


	public void PlayerReady() {
		LoadedText.SetActive(false);
		ReadyText.SetActive(true);
	}


	public void HideReady() {
		ReadyText.SetActive(false);
	}


	public void ShowNickname() {
		Nickname.gameObject.SetActive(true);
	}


	public void HideNickname() {
		Nickname.gameObject.SetActive(false);
	}


	public void SetReward(int reward) {
		Reward.gameObject.SetActive(true);
		RewardValue.text = reward.ToString();
	}
}