using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using AEngine;
using Managers;
using Photon.Pun;
using UnityEngine.UI;

public class MultiplayerMenu : Menu {
	public event Action OnShowMenu;
	public event Action OnHideMenu;

	// Обязательно больше нуля
	[SerializeField] private int            LevelIndex = 1;
	[SerializeField] private GameObject     _gameLoader;
	[SerializeField] private AlphaAnimation _searchPlayersAnim;
	[SerializeField] private CanvasGroup    _canvasGroup;

	[SerializeField] private GameObject StatusPanel;
	[SerializeField] private Text       StatusText;


	private void Start() {
		PhotonNetwork.AutomaticallySyncScene = true;
	}


	private void Update() {
		if (!PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom && PhotonNetwork.LevelLoadingProgress > 0 && !_gameLoader.activeSelf) {
			OpenGame();
			Hide(null);
		}
	}


	protected override void Show() {
		base.Show();

		var sequence = DOTween.Sequence();
		sequence.Append(_searchPlayersAnim.Show());
		sequence.OnComplete(delegate { OnShowMenu.SafeInvoke(); });

		_canvasGroup.blocksRaycasts = true;
		_canvasGroup.interactable   = true;

		AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.multiplayer_menu_open);
	}


	protected override void StartHide(Action callback) {
		print("Hide");
		base.StartHide(callback);

		var sequence = DOTween.Sequence();
		sequence.Insert(0f, _searchPlayersAnim.Hide());
		sequence.OnComplete(() => {
			FinishHide(callback);
			OnHideMenu.SafeInvoke();
		});

		_canvasGroup.blocksRaycasts = false;
		_canvasGroup.interactable   = false;
	}


	public void Hide(Action callback) {
		StartHide(callback);
	}


	public void OpenGame() {
		AdManager.Instance.HideBottomBanner();
		print("Open Game");
		_audio.PlaySound(Sounds.Tap);

		EnvironmentController.CurrentMode = GameModes.Multiplayer;

		MenuController.TransitionTarget = MenuKinds.None;
		_gameLoader.SetActive(true);
	}


	public void SetStatus(string text) {
		StatusPanel.SetActive(true);
		StatusText.text = text;
	}


	public void HideStatusPanel() {
		StatusPanel.SetActive(false);
	}


	#region Events

	public void OnHomeButtonClick() {
		_audio.PlaySound(Sounds.Tap);
		_menuController.OpenMenu(MenuKinds.SelectLevelType);
	}

	#endregion
}