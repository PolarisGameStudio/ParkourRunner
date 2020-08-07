using System;
using UnityEngine;
using UnityEngine.UI;
using AEngine;
using MainMenuAndShop.Helmets;
using Managers;

public class HelmetSelection : MonoBehaviour {
	public static event Action<Helmets.HelmetsType> OnSelectHelmet;
	public static event Action<int>                 OnNotEnoughCoins;
	private static Helmets.HelmetsType              _currentSelection;

	[SerializeField] private Helmets             Configuration;
	[SerializeField] private Helmets.HelmetsType HelmetType;
	[SerializeField] private GameObject          ActiveSelection;
	[SerializeField] private GameObject          SelectedSelection;
	[SerializeField] private GameObject          DisableSelection;

	[Header("Buy block")] [SerializeField] private Text       PriceText;
	[SerializeField]                       private GameObject PriceBlock;
	[SerializeField]                       private GameObject LockBlock;
	[SerializeField]                       private GameObject SelectCaption;
	[SerializeField]                       private GameObject BuyCaption;

	private Helmets.Data _data;
	private Wallet       _wallet;


	private void Awake() {
		_data = Configuration.GetHelmetData(HelmetType);

		PriceText.text = _data.Price.ToString();
		PriceBlock.SetActive(!_data.Bought);
		BuyCaption.SetActive(!_data.Bought);
		SelectCaption.SetActive(_data.Bought);

		_wallet = Wallet.Instance;
	}


	private void OnEnable() {
		OnSelectHelmet -= OnSelectHelmetHandle;
		OnSelectHelmet += OnSelectHelmetHandle;

		if (Helmets.CurrentHelmetType == HelmetType) OnSelectHelmet.SafeInvoke(HelmetType);
	}


	private void OnDisable() {
		OnSelectHelmet -= OnSelectHelmetHandle;
	}


	private void Start() {
		if (Helmets.CurrentHelmetType == HelmetType) {
			_currentSelection = HelmetType;
			OnSelectHelmet.SafeInvoke(HelmetType);
		}
	}


	private void RefreshSelection() {
		if (_currentSelection == HelmetType) {
			SelectedSelection.SetActive(true);
			ActiveSelection.SetActive(false);
			DisableSelection.SetActive(false);
		}
		else {
			SelectedSelection.SetActive(false);
			ActiveSelection.SetActive(Helmets.CurrentHelmetType == HelmetType);
			DisableSelection.SetActive(!ActiveSelection.activeSelf);
		}

		PriceBlock.SetActive(!_data.Bought);
		LockBlock.SetActive(!_data.Bought);
		BuyCaption.SetActive(!_data.Bought);

		SelectCaption.SetActive(_data.Bought);
	}


	public void OnSelectButtonClick() {
		if (_currentSelection != HelmetType) {
			AudioManager.Instance.PlaySound(Sounds.Tap);
		}

		_currentSelection = HelmetType;

		if (_data.Bought) {
			Helmets.CurrentHelmetType = HelmetType;
		}

		OnSelectHelmet.SafeInvoke(HelmetType);
	}


	public void OnBuyButtonClick() {
		if (!_data.Bought && _wallet.SpendCoins(_data.Price)) {
			AudioManager.Instance.PlaySound(Sounds.ShopSlot);

			_currentSelection = HelmetType;
			_data.Bought      = true;

			OnSelectHelmet.SafeInvoke(HelmetType);
			AppsFlyerManager.ShopBuyHelmet(HelmetType.ToString());
		}
		else {
			if (_data.Bought)
				OnSelectButtonClick();
			else {
				AudioManager.Instance.PlaySound(Sounds.Tap);
				OnNotEnoughCoins.SafeInvoke(Mathf.Abs(_wallet.AllCoins - _data.Price));
			}
		}
	}


	private void OnSelectHelmetHandle(Helmets.HelmetsType helmet) {
		RefreshSelection();
	}
}