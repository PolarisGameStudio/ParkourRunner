using System;
using Managers.Advertising;
using UnityEngine;

public class AdManager : MonoBehaviour {
	#region Singleton

	public static AdManager Instance;


	private void Awake() {
		if (Instance == null) {
			Instance = this;

			ResetAdvertisingOrder();

			DontDestroyOnLoad(this);
		}
		else {
			Destroy(this.gameObject);
		}
	}


	#endregion


	[SerializeField] private AppodealAdController _ad;

	private int _gameSessionCount;

	public static bool EnableAds => PlayerPrefs.GetInt("NoAds", 0) != 1;


	public void Start() {
		_ad.Initialize();
		ShowInterstitial(null, null, null);
	}


	public bool InterstitialIsAvailable() {
		return _ad != null && _ad.InterstitialIsLoaded();
	}


	public bool RewardedVideoIsAvailable() {
		return _ad != null && _ad.RewardedVideoLoaded();
	}


	public bool NonSkippableVideoIsAvailable() {
		return _ad != null && _ad.NonSkippableVideoIsLoaded();
	}


	public void ShowInterstitial(Action finishedCallback = null, Action skippedCallback = null, Action failedCallback = null) {
		// Debug.Log("Show Interstitial");
		_ad.InitCallbackHandlers(finishedCallback, skippedCallback, failedCallback, AdType.Interstitial);

		if (EnableAds) _ad.ShowInterstitial();
		else _ad.HandleAdResult(AdResults.Finished, AdType.Interstitial);
	}


	public void ShowRewardedVideo(Action finishedCallback, Action skippedCallback, Action failedCallback) {
		// Debug.Log("Show Rewarded Video");
		_ad.InitCallbackHandlers(finishedCallback, skippedCallback, failedCallback, AdType.RewardedVideo);
		_ad.ShowRewardedVideo();
	}


	public void ShowNonSkippableVideo(Action finishedCallback, Action skippedCallback, Action failedCallback) {
		// Debug.Log("Show Non Skippable Video");
		_ad.InitCallbackHandlers(finishedCallback, skippedCallback, failedCallback, AdType.NonSkippableVideo);
		_ad.ShowNonSkippableVideo();
	}


	public void ShowBanner() {
		if (!EnableAds) return;
		// Debug.Log("Show Banner");
		_ad.ShowBanner();
	}


	public void ShowBottomBanner() {
		if (!EnableAds) return;
		Debug.Log("Show Bottom Banner");
		_ad.ShowBottomBanner();
	}


	public void HideBottomBanner() {
		// Debug.Log("Hide Bottom Banner");
		_ad.HideBottomBanner();
	}


	#region Advertising Order


	/// <summary>
	/// Как я понял, можно ли показывать рекламу.
	/// Нужен, чтобы не показывать рекламу каждый забег
	/// </summary>
	public bool CheckAdvertisingOrder() {
		if (PlayerPrefs.GetInt(EnvironmentController.TUTORIAL_KEY) == 1 || _gameSessionCount % 3 == 0) {
			ResetAdvertisingOrder();
			return false;
		}

		_gameSessionCount++;
		return true;
	}


	public void ResetAdvertisingOrder() {
		_gameSessionCount = 1;
	}


	public void SkipAdInOrder() {
		_gameSessionCount = 3;
	}

	#endregion
}