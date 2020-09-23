using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using AEngine;
using UnityEngine.Purchasing;

public class ShopDonatsPanel : MonoBehaviour {
	[SerializeField] private DonatShopData _donatData;
	[SerializeField] private Button        _buyButton;

	[Header("UI view")] [SerializeField] private Text       _coinsText;
	[SerializeField]                     private GameObject _purchaseText;
	[SerializeField]                     private GameObject _onPurchasedText;
	[SerializeField]                     private Text       _purchasePrice;
	[SerializeField]                     private Text       _purchaseCurrency;

	private InAppManager _purchaseManager;
	private bool         _isInitialized = false;


	private void Start() {
		_purchaseManager = InAppManager.Instance;

		if (_coinsText != null)
			_coinsText.text = _donatData.DonatValue;

		if (!_isInitialized)
			StartCoroutine(LoadingDataProcess());

		InAppManager.OnBuySuccess += OnBuySuccess;
	}


	private void OnDisable() {
		InAppManager.OnBuySuccess -= OnBuySuccess;
	}


	private IEnumerator LoadingDataProcess() {
		while (!_purchaseManager.IsInitialized())
			yield return null;

		RefreshProductData();

		_isInitialized = true;
	}


	private void RefreshProductData() {
		if (_purchasePrice != null)
			_purchasePrice.text = _purchaseManager.GetLocalizedPrice(_donatData.ProductGameId);
		if (_purchaseCurrency != null)
			_purchaseCurrency.text = _purchaseManager.GetLocalizedCurrency(_donatData.ProductGameId);
	}


	#region Events

	public void OnBuyButtonClick() {
		AudioManager.Instance.PlaySound(Sounds.Tap);
#if UNITY_EDITOR
		OnBuySuccess(_donatData.DonatKind);
		return;
#endif
		_purchaseManager.BuyProductID(_donatData.ProductGameId);
	}


	private void OnBuySuccess(DonatShopData.DonatKinds productKind) {
		// print($"OnBuySuccess: {productKind.ToString()}");
		if (_donatData.DonatKind == productKind) {
			Shoping.GetDonat(_donatData);
			AudioManager.Instance.PlaySound(Sounds.ShopSlot);
			if (_donatData.PurchaseType == ProductType.NonConsumable) {
				Purchased();
			}
		}
	}


	public void Purchased() {
		_buyButton.interactable = false;

		_purchasePrice.gameObject.SetActive(false);
		_purchaseCurrency.gameObject.SetActive(false);

		if (_purchaseText) _purchaseText.SetActive(false);
		if (_onPurchasedText) _onPurchasedText.SetActive(true);
	}

	#endregion
}