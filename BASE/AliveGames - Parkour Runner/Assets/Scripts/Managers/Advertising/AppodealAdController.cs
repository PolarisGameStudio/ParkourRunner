using System.Collections;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

public class AppodealAdController : BaseAdController, IInterstitialAdListener {
	private const int MAX_FRAMES_TO_INTERSTITIAL = 2;

	[SerializeField] private string _androidAppKey;
	[SerializeField] private string _iosAppKey;
	[SerializeField] private bool   _isTesting;

	private bool _failed, _success, _skipped;


	private void Update() {
		if (_failed) HandleAdResult(AdResults.Failed);
		if (_success) HandleAdResult(AdResults.Finished);
		if (_skipped) HandleAdResult(AdResults.Skipped);

		_failed = _success = _skipped = false;
	}


	public override void Initialize() {
		Appodeal.setTesting(_isTesting);

#if UNITY_IPHONE || UNITY_IOS
        Appodeal.initialize(_iosAppKey, Appodeal.INTERSTITIAL);
#elif UNITY_ANDROID
		Appodeal.initialize(_androidAppKey, Appodeal.INTERSTITIAL);
#endif

		Appodeal.setInterstitialCallbacks(this);
	}


	public override bool IsAvailable() {
		return true;
	}


	public override void ShowAdvertising() {
		StartCoroutine(ShowAdsProcess());
	}


	private IEnumerator ShowAdsProcess() {
		yield return new WaitWhile(() => !Appodeal.isLoaded(Appodeal.INTERSTITIAL));

		Appodeal.show(Appodeal.INTERSTITIAL);
	}


	#region Interface

	public void onInterstitialFailedToLoad() {
		_failed = true;
	}


	public void onInterstitialExpired() {
		_failed = true;
	}


	public void onInterstitialLoaded(bool isPrecache) { }

	public void onInterstitialClicked() { }


	public void onInterstitialClosed() {
		_success = true;
	}


	public void onInterstitialShown() { }

	#endregion
}