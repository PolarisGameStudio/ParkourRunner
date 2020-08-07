using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using AEngine;
using MainMenuAndShop.Helmets;
using MainMenuAndShop.Jetpacks;
using Managers;

public class JetpackSelection : MonoBehaviour {
	public static event Action<Jetpacks.JetpacksType> OnSelectJetpack;
	public static event Action<int>                   OnNotEnoughCoins;
	private static Jetpacks.JetpacksType              _currentSelection;

	[SerializeField] private Jetpacks.JetpacksType JetpackType;
	[SerializeField] private GameObject            ActiveSelection;
	[SerializeField] private GameObject            SelectedSelection;
	[SerializeField] private GameObject            DisableSelection;

	[Header("Buy block")] [SerializeField] private Text       PriceText;
	[SerializeField]                       private GameObject PriceBlock;
	[SerializeField]                       private Text       BoostTimeText;
	[SerializeField]                       private GameObject LockBlock;
	[SerializeField]                       private GameObject SelectCaption;
	[SerializeField]                       private GameObject BuyCaption;

	[Space] [SerializeField] private CharactersView CharactersView;

	private Jetpacks.Jetpack _jetpack;
	private Wallet           _wallet;


	private void Awake() {
		_jetpack = Jetpacks.GetJetpackData(JetpackType);

		PriceText.text     = _jetpack.Price.ToString();
		BoostTimeText.text = _jetpack.BoostTime.ToString(CultureInfo.InvariantCulture);
		PriceBlock.SetActive(!_jetpack.Bought);
		BuyCaption.SetActive(!_jetpack.Bought);
		SelectCaption.SetActive(_jetpack.Bought);

		_wallet = Wallet.Instance;
	}


	private void OnEnable() {
		CharactersView.RotateToShowSpine();

		OnSelectJetpack -= OnSelectJetpackHandle;
		OnSelectJetpack += OnSelectJetpackHandle;

		if (Jetpacks.ActiveJetpackType == JetpackType) OnSelectJetpack.SafeInvoke(JetpackType);
	}


	private void OnDisable() {
		CharactersView.ResetRotation();
		OnSelectJetpack -= OnSelectJetpackHandle;
	}


	private void RefreshSelection() {
		if (_currentSelection == JetpackType) {
			SelectedSelection.SetActive(true);
			ActiveSelection.SetActive(false);
			DisableSelection.SetActive(false);
		}
		else {
			SelectedSelection.SetActive(false);
			ActiveSelection.SetActive(Jetpacks.ActiveJetpackType == JetpackType);
			DisableSelection.SetActive(!ActiveSelection.activeSelf);
		}

		PriceBlock.SetActive(!_jetpack.Bought);
		LockBlock.SetActive(!_jetpack.Bought);
		BuyCaption.SetActive(!_jetpack.Bought);

		SelectCaption.SetActive(_jetpack.Bought);
	}


	public void OnSelectButtonClick() {
		if (_currentSelection != JetpackType) {
			AudioManager.Instance.PlaySound(Sounds.Tap);
		}

		_currentSelection = JetpackType;

		if (_jetpack.Bought) {
			Jetpacks.ActiveJetpackType = JetpackType;
		}

		OnSelectJetpack.SafeInvoke(JetpackType);
	}


	public void OnBuyButtonClick() {
		if (!_jetpack.Bought && _wallet.SpendCoins(_jetpack.Price)) {
			AudioManager.Instance.PlaySound(Sounds.ShopSlot);

			_currentSelection = JetpackType;
			_jetpack.Bought   = true;

			OnSelectJetpack.SafeInvoke(JetpackType);
			AppsFlyerManager.ShopBuyHelmet(JetpackType.ToString());
		}
		else {
			if (_jetpack.Bought)
				OnSelectButtonClick();
			else {
				AudioManager.Instance.PlaySound(Sounds.Tap);
				OnNotEnoughCoins.SafeInvoke(Mathf.Abs(_wallet.AllCoins - _jetpack.Price));
			}
		}
	}


	private void OnSelectJetpackHandle(Jetpacks.JetpacksType jetpacksType) {
		RefreshSelection();
	}
}