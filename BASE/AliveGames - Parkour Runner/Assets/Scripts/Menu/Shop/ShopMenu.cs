using System;
using UnityEngine;
using DG.Tweening;
using AEngine;
using Managers;

public class ShopMenu : Menu {
	public const string FirstOpenKey = "ShopOpenedEarlier";

	public static event Action OnShowMenu;
	public static event Action OnHideMenu;

	[Header("Animation settings")] [SerializeField]
	private float _showAfterBgDelay;
	[SerializeField] private float           _hideForBgDelay;
	[SerializeField] private MovingAnimation _homeButtonAnim;
	[SerializeField] private MovingAnimation _playerStatusAnim;
	[SerializeField] private MovingAnimation _shopAnim;
	[SerializeField] private AlphaAnimation  _backgroundAnim;

	[Space] [SerializeField] private GameObject  RestoreButton;
	[SerializeField]         private GameObject  CharacterBlock;
	[SerializeField]         private StarterPack StarterPack;


	private void Start() {
		if (Application.platform != RuntimePlatform.IPhonePlayer &&
			Application.platform != RuntimePlatform.OSXPlayer) {
			RestoreButton.SetActive(false);
		}
	}


	private void Update() {
		if (Input.GetKeyDown(KeyCode.Keypad1)) LootBoxesRoom.OpenLootBox(LootBoxesRoom.LootBoxType.Brown);
		else if (Input.GetKeyDown(KeyCode.Keypad2)) LootBoxesRoom.OpenLootBox(LootBoxesRoom.LootBoxType.Red);
		else if (Input.GetKeyDown(KeyCode.Keypad3)) LootBoxesRoom.OpenLootBox(LootBoxesRoom.LootBoxType.Purple);
		else if (Input.GetKeyDown(KeyCode.Keypad4)) LootBoxesRoom.OpenLootBox(LootBoxesRoom.LootBoxType.Gold);
	}


	protected override void Show() {
		base.Show();
		OnShowMenu.SafeInvoke();
		CharacterBlock.SetActive(true);

		var sequence = DOTween.Sequence();
		sequence.Append(_backgroundAnim.Hide());

		sequence.Insert(_showAfterBgDelay, _shopAnim.Show());
		sequence.Insert(_showAfterBgDelay, _playerStatusAnim.Show());
		sequence.Insert(_showAfterBgDelay, _homeButtonAnim.Show());
		sequence.onComplete += delegate {
			if (!PlayerPrefs.HasKey(FirstOpenKey) || PlayerPrefs.GetInt(FirstOpenKey) == 0) {
				if (StarterPack.CanBuy) {
					StarterPack.Show();
					PlayerPrefs.SetInt(FirstOpenKey, 1);
				}
			}
		};

		AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.shop_menu_open);
	}


	protected override void StartHide(Action callback) {
		print("Hide shop");
		base.StartHide(callback);
		OnHideMenu.SafeInvoke();

		var sequence = DOTween.Sequence();

		sequence.Append(_shopAnim.Hide());

		sequence.Insert(0f,              _playerStatusAnim.Hide());
		sequence.Insert(0f,              _homeButtonAnim.Hide());
		sequence.Insert(_hideForBgDelay, _backgroundAnim.Show());

		sequence.OnComplete(() => {
			FinishHide(callback);
			CharacterBlock.SetActive(false);
		});
	}


	#region Events

	public void OnHomeButtonClick() {
		_audio.PlaySound(Sounds.Tap);
		_menuController.OpenMenu(MenuKinds.MainMenu);
	}


	public void OnRestoreButton() {
		_audio.PlaySound(Sounds.Tap);
		InAppManager.Instance.RestorePurchases();
	}

	#endregion
}