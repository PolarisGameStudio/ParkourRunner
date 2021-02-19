using TapjoyUnity;
using UnityEngine;

public class TapjoyController : MonoBehaviour {
	private const string OfferwallPlacementName = "offerwall_unit";
	private static TJPlacement OfferwallPlacement;

	public static bool OfferwallAvailable => OfferwallPlacement?.IsContentAvailable() ?? false;


	private void Start() {
		if (Tapjoy.IsConnected) return;
		Debug.Log("C#: TapjoySample start and adding Tapjoy Delegates");

		// Connect Delegates
		Tapjoy.OnConnectSuccess += HandleConnectSuccess;
		Tapjoy.OnConnectFailure += HandleConnectFailure;
	}


	private void OnDestroy() {
		Debug.Log("C#: Disabling and removing Tapjoy Delegates");
		// Connect Delegates
		Tapjoy.OnConnectSuccess -= HandleConnectSuccess;
		Tapjoy.OnConnectFailure -= HandleConnectFailure;
	}


	public static void ShowOfferwall() {
		if (OfferwallAvailable) OfferwallPlacement.ShowContent();
	}


	public static void UpdateCurrency() {
		Tapjoy.GetCurrencyBalance();
	}


	public static void SpendCurrency(int amount) {
		Tapjoy.SpendCurrency(amount);
	}


	#region Tapjoy Delegate Handlers

	#region Connect Delegate Handlers

	private static void HandleConnectSuccess() {
		Debug.Log("C#: Handle Connect Success");
		AddTapjoyDelegates();
		// Create offerwall placement
		if (OfferwallPlacement == null) {
			OfferwallPlacement = TJPlacement.CreatePlacement(OfferwallPlacementName);
			// OfferwallPlacement.RequestContent();
		}
	}


	private static void HandleConnectFailure() {
		Debug.Log("C#: Handle Connect Failure");
	}


	private static void AddTapjoyDelegates() {
		TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
		TJPlacement.OnRequestFailure += HandlePlacementRequestFailure;
		TJPlacement.OnContentReady   += HandlePlacementContentReady;
		TJPlacement.OnContentShow    += HandlePlacementContentShow;
		TJPlacement.OnContentDismiss += HandlePlacementContentDismiss;
		TJPlacement.OnClick          += HandlePlacementOnClick;

		// Currency Delegates
		Tapjoy.OnAwardCurrencyResponse             += HandleAwardCurrencyResponse;
		Tapjoy.OnAwardCurrencyResponseFailure      += HandleAwardCurrencyResponseFailure;
		Tapjoy.OnSpendCurrencyResponse             += HandleSpendCurrencyResponse;
		Tapjoy.OnSpendCurrencyResponseFailure      += HandleSpendCurrencyResponseFailure;
		Tapjoy.OnGetCurrencyBalanceResponse        += HandleGetCurrencyBalanceResponse;
		Tapjoy.OnGetCurrencyBalanceResponseFailure += HandleGetCurrencyBalanceResponseFailure;
		Tapjoy.OnEarnedCurrency                    += HandleEarnedCurrency;
	}


	private static void RemoveTapjoyDelegates() {
		Debug.Log("C#: Disabling and removing Tapjoy Delegates");

		// Placement delegates
		TJPlacement.OnRequestSuccess -= HandlePlacementRequestSuccess;
		TJPlacement.OnRequestFailure -= HandlePlacementRequestFailure;
		TJPlacement.OnContentReady   -= HandlePlacementContentReady;
		TJPlacement.OnContentShow    -= HandlePlacementContentShow;
		TJPlacement.OnContentDismiss -= HandlePlacementContentDismiss;
		TJPlacement.OnClick          -= HandlePlacementOnClick;

		// Currency Delegates
		Tapjoy.OnAwardCurrencyResponse             -= HandleAwardCurrencyResponse;
		Tapjoy.OnAwardCurrencyResponseFailure      -= HandleAwardCurrencyResponseFailure;
		Tapjoy.OnSpendCurrencyResponse             -= HandleSpendCurrencyResponse;
		Tapjoy.OnSpendCurrencyResponseFailure      -= HandleSpendCurrencyResponseFailure;
		Tapjoy.OnGetCurrencyBalanceResponse        -= HandleGetCurrencyBalanceResponse;
		Tapjoy.OnGetCurrencyBalanceResponseFailure -= HandleGetCurrencyBalanceResponseFailure;
		Tapjoy.OnEarnedCurrency                    -= HandleEarnedCurrency;
	}

	#endregion


	#region Placement Delegate Handlers

	private static void HandlePlacementRequestSuccess(TJPlacement placement) {
		if (placement.IsContentAvailable()) {
			Debug.Log("C#: Content available for " + placement.GetName());
			if (placement.GetName() == OfferwallPlacementName) {
				// Show offerwall immediately
				// placement.ShowContent();
			}
		}
		else {
			Debug.Log("C#: No content available for " + placement.GetName());
		}
	}


	private static void HandlePlacementRequestFailure(TJPlacement placement, string error) {
		Debug.Log("C#: HandlePlacementRequestFailure");
		Debug.Log("C#: Request for " + placement.GetName() + " has failed because: " + error);
	}


	private static void HandlePlacementContentReady(TJPlacement placement) {
		Debug.Log("C#: HandlePlacementContentReady");
		if (placement.IsContentAvailable()) {
			//placement.ShowContent();
		}
		else {
			Debug.Log("C#: no content");
		}
	}


	private static void HandlePlacementContentShow(TJPlacement placement) {
		Debug.Log("C#: HandlePlacementContentShow");
	}


	private static void HandlePlacementContentDismiss(TJPlacement placement) {
		Debug.Log("C#: HandlePlacementContentDismiss");
		Debug.Log("TJPlacement " + placement.GetName() + " has been dismissed");
		if (placement.GetName() == OfferwallPlacementName) {
			OfferwallPlacement.RequestContent();
		}
	}


	private static void HandlePlacementOnClick(TJPlacement placement) {
		Debug.Log("C#: HandlePlacementOnClick");
	}

	#endregion


	#region Currency Delegate Handlers

	private static void HandleAwardCurrencyResponse(string currencyName, int balance) {
		Debug.Log("C#: HandleAwardCurrencySucceeded: currencyName: " + currencyName + ", balance: " + balance);
		Debug.Log("Awarded Currency -- "                             + currencyName + " Balance: "  + balance);
	}


	private static void HandleAwardCurrencyResponseFailure(string error) {
		Debug.Log("C#: HandleAwardCurrencyResponseFailure: " + error);
	}


	private static void HandleGetCurrencyBalanceResponse(string currencyName, int balance) {
		Debug.Log("C#: HandleGetCurrencyBalanceResponse: currencyName: " + currencyName + ", balance: " + balance);
		Debug.Log(currencyName                                           + " Balance: " + balance);
	}


	private static void HandleGetCurrencyBalanceResponseFailure(string error) {
		Debug.Log("C#: HandleGetCurrencyBalanceResponseFailure: " + error);
	}


	private static void HandleSpendCurrencyResponse(string currencyName, int balance) {
		Debug.Log("C#: HandleSpendCurrencyResponse: currencyName: " + currencyName + ", balance: " + balance);
		Debug.Log(currencyName                                      + " Balance: " + balance);
	}


	private static void HandleSpendCurrencyResponseFailure(string error) {
		Debug.Log("C#: HandleSpendCurrencyResponseFailure: " + error);
	}


	private static void HandleEarnedCurrency(string currencyName, int amount) {
		Debug.Log("C#: HandleEarnedCurrency: currencyName: " + currencyName + ", amount: " + amount);
		Debug.Log(currencyName                               + " Earned: "  + amount);

		Wallet.Instance.AddCoins(amount, Wallet.WalletMode.Global);
		Tapjoy.ShowDefaultEarnedCurrencyAlert();
		SpendCurrency(amount);
	}

	#endregion

	#endregion
}