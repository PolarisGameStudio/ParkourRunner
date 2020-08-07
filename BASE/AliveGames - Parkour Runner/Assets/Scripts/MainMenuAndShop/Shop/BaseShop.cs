using UnityEngine;
using UnityEngine.UI;
using AEngine;
using System;

public class BaseShop : MonoBehaviour {
	public enum ShopsType {
		charShop,
		coinsShop,
		bonusShop,
		helmetsShop,
		jetpackShop,
	}

	[Serializable]
	private class Tab {
		public Button button;
		public Image  image;
		public Sprite enable;
		public Sprite disable;
	}

	[SerializeField] private GameObject[] _allShops;
	[SerializeField] private GameObject   _notEnoughCoinsWindow;
	[SerializeField] private Text         _notEnougtCoins;
	[SerializeField] private Tab          _charTab;
	[SerializeField] private Tab          _helmetsTab;
	[SerializeField] private Tab          _coinstTab;
	[SerializeField] private Tab          _bonusesTab;
	[SerializeField] private Tab          _jetpacksTab;

	private AudioManager _audio;


	private void Awake() {
		_audio = AudioManager.Instance;
	}


	private void Start() {
		_charTab.button.onClick.AddListener(() => OnSelectShopClick(ShopsType.charShop,        true));
		_helmetsTab.button.onClick.AddListener(() => OnSelectShopClick(ShopsType.helmetsShop,  true));
		_coinstTab.button.onClick.AddListener(() => OnSelectShopClick(ShopsType.coinsShop,     true));
		_bonusesTab.button.onClick.AddListener(() => OnSelectShopClick(ShopsType.bonusShop,    true));
		_jetpacksTab.button.onClick.AddListener(() => OnSelectShopClick(ShopsType.jetpackShop, true));

		OnActivateDefaultTab(false);
	}


	private void OnEnable() {
		CharacterSelection.OnNotEnoughCoins += OnShowNotEnoughWindow;
		HelmetSelection.OnNotEnoughCoins    += OnShowNotEnoughWindow;
		JetpackSelection.OnNotEnoughCoins   += OnShowNotEnoughWindow;
	}


	private void OnDisable() {
		CharacterSelection.OnNotEnoughCoins -= OnShowNotEnoughWindow;
		HelmetSelection.OnNotEnoughCoins    -= OnShowNotEnoughWindow;
		JetpackSelection.OnNotEnoughCoins   -= OnShowNotEnoughWindow;
	}


	private void ActivateTargetShop(GameObject shop) {
		foreach (var item in _allShops) {
			item.SetActive(item == shop);
		}
	}


	private void ActivateTargetTab(Tab target) {
		_charTab.image.sprite     = _charTab     == target ? _charTab.enable : _charTab.disable;
		_helmetsTab.image.sprite  = _helmetsTab  == target ? _helmetsTab.enable : _helmetsTab.disable;
		_coinstTab.image.sprite   = _coinstTab   == target ? _coinstTab.enable : _coinstTab.disable;
		_bonusesTab.image.sprite  = _bonusesTab  == target ? _bonusesTab.enable : _bonusesTab.disable;
		_jetpacksTab.image.sprite = _jetpacksTab == target ? _jetpacksTab.enable : _jetpacksTab.disable;
	}


	#region Events

	public void OnActivateDefaultTab(bool playSound) {
		OnSelectShopClick(ShopsType.charShop, playSound);
	}


	private void OnSelectShopClick(ShopsType shop, bool playSound) {
		switch (shop) {
			case ShopsType.coinsShop:
				ActivateTargetShop(_allShops[(int) ShopsType.coinsShop]);
				ActivateTargetTab(_coinstTab);
				break;

			case ShopsType.helmetsShop:
				ActivateTargetShop(_allShops[(int) ShopsType.helmetsShop]);
				ActivateTargetTab(_helmetsTab);
				break;

			case ShopsType.bonusShop:
				ActivateTargetShop(_allShops[(int) ShopsType.bonusShop]);
				ActivateTargetTab(_bonusesTab);
				break;

			case ShopsType.charShop:
				ActivateTargetShop(_allShops[(int) ShopsType.charShop]);
				ActivateTargetTab(_charTab);
				break;

			case ShopsType.jetpackShop:
				ActivateTargetShop(_allShops[(int) ShopsType.jetpackShop]);
				ActivateTargetTab(_jetpacksTab);
				break;

			default:
				break;
		}

		if (playSound) {
			_audio.PlaySound(Sounds.Tap);
		}
	}


	public void OnShowNotEnoughWindow(int coins) {
		_notEnougtCoins.text = coins.ToString();
		_notEnoughCoinsWindow.SetActive(true);
	}


	public void OnBuyNotEnoughWindowClick() {
		_audio.PlaySound(Sounds.Tap);

		ActivateTargetShop(_allShops[(int) ShopsType.coinsShop]);
		ActivateTargetTab(_coinstTab);

		_notEnoughCoinsWindow.SetActive(false);
	}


	public void OnCloseNotEnoughCoinsWindow() {
		_audio.PlaySound(Sounds.Tap);
		_notEnoughCoinsWindow.SetActive(false);
	}

	#endregion
}