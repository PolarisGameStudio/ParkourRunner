using System;
using Managers;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour {
	[SerializeField] private PhotonView PhotonView;
	[SerializeField] private Canvas Canvas;
	[SerializeField] private Text Nickname;
	[SerializeField] private GameObject LoadedText, ReadyText;


	private void Start() {
		if(!PhotonGameManager.IsMultiplayer) return;

		Canvas.worldCamera = Camera.current;
		Nickname.gameObject.SetActive(true);

		Nickname.text = PhotonView.Owner.NickName;
		if (PhotonView.IsMine) Nickname.text += "(You)";
		else LoadedText.SetActive(true);
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
}