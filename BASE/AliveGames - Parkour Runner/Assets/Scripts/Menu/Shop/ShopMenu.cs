using System;
using UnityEngine;
using DG.Tweening;
using AEngine;
using Managers;

public class ShopMenu : Menu {
	public static event Action OnShowMenu;
	public static event Action OnHideMenu;

	[Header("Animation settings")] [SerializeField]
	private float _showAfterBgDelay;
	[SerializeField] private float           _hideForBgDelay;
	[SerializeField] private MovingAnimation _homeButtonAnim;
	[SerializeField] private MovingAnimation _playerStatusAnim;
	[SerializeField] private MovingAnimation _shopAnim;
	[SerializeField] private AlphaAnimation  _backgroundAnim;

	[Space] [SerializeField] private GameObject RestoreButton;


	private void Start() {
		if (Application.platform != RuntimePlatform.IPhonePlayer &&
			Application.platform != RuntimePlatform.OSXPlayer) {
			RestoreButton.SetActive(false);
		}
	}


	protected override void Show() {
		base.Show();
		OnShowMenu.SafeInvoke();

		var sequence = DOTween.Sequence();
		sequence.Append(_backgroundAnim.Hide());

		sequence.Insert(_showAfterBgDelay, _shopAnim.Show());
		sequence.Insert(_showAfterBgDelay, _playerStatusAnim.Show());
		sequence.Insert(_showAfterBgDelay, _homeButtonAnim.Show());

		AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.shop_menu_open);
	}


	protected override void StartHide(Action callback) {
		base.StartHide(callback);
		OnHideMenu.SafeInvoke();

		var secuance = DOTween.Sequence();

		secuance.Append(_shopAnim.Hide());

		secuance.Insert(0f,              _playerStatusAnim.Hide());
		secuance.Insert(0f,              _homeButtonAnim.Hide());
		secuance.Insert(_hideForBgDelay, _backgroundAnim.Show());

		secuance.OnComplete(() => { FinishHide(callback); });
	}


	#region Events

	public void OnHomeButtonClick() {
		_audio.PlaySound(Sounds.Tap);
		_menuController.OpenMenu(MenuKinds.MainMenu);
	}


	public void OnRestoreButton() {
		InAppManager.Instance.RestorePurchases();
	}

	#endregion
}