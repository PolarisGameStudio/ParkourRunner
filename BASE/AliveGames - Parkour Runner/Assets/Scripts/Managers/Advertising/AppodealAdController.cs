using System.Collections;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using Managers.Advertising;
using UnityEngine;

public class AppodealAdController : BaseAdController, IInterstitialAdListener, IRewardedVideoAdListener,
									INonSkippableVideoAdListener, IBannerAdListener {
	private const int MAX_FRAMES_TO_INTERSTITIAL = 2;

	[SerializeField] private string _androidAppKey;
	[SerializeField] private string _iosAppKey;
	[SerializeField] private bool   _isTesting;

	private Coroutine _bottomBanner;


	public override void Initialize() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		return;
#endif

		if(!AdManager.EnableAds) return;

		// Appodeal.setLogLevel(Appodeal.LogLevel.Debug);
		Appodeal.setTesting(_isTesting);
		Appodeal.setSmartBanners(true);
		Appodeal.setTabletBanners(true);

		Appodeal.initialize(_androidAppKey,
							Appodeal.INTERSTITIAL | Appodeal.BANNER_BOTTOM | Appodeal.REWARDED_VIDEO |
							Appodeal.NON_SKIPPABLE_VIDEO);
		Appodeal.cache(Appodeal.INTERSTITIAL | Appodeal.BANNER_BOTTOM | Appodeal.REWARDED_VIDEO |
						Appodeal.NON_SKIPPABLE_VIDEO);

		Appodeal.setInterstitialCallbacks(this);
		Appodeal.setRewardedVideoCallbacks(this);
		Appodeal.setNonSkippableVideoCallbacks(this);
		// Appodeal.setBannerCallbacks(this);
	}


	public override bool InterstitialIsLoaded() {
		return Appodeal.isLoaded(Appodeal.INTERSTITIAL);
	}


	public override bool RewardedVideoLoaded() {
		return Appodeal.isLoaded(Appodeal.REWARDED_VIDEO);
	}


	public override bool NonSkippableVideoIsLoaded() {
		return Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO);
	}


	#region Show Ad

	public override void ShowInterstitial() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		HandleAdResult(AdResults.Finished, AdType.Interstitial);
		return;
#endif
		StartCoroutine(ShowInterstitialProcess());
	}


	public override void ShowBanner() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		return;
#endif
		// StartCoroutine(ShowBannerProcess());
	}


	public override void ShowBottomBanner() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		return;
#endif
		_bottomBanner = StartCoroutine(ShowBottomBannerProcess());
	}


	public override void HideBottomBanner() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		return;
#endif
		// print("Stopping 'ShowBottomBannerProcess'");
		if(_bottomBanner != null) StopCoroutine(_bottomBanner);
		Appodeal.hide(Appodeal.BANNER_BOTTOM);
	}


	public override void ShowRewardedVideo() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		HandleAdResult(AdResults.Finished, AdType.RewardedVideo);
		return;
#endif
		StartCoroutine(ShowVideoProcess());
	}


	public override void ShowNonSkippableVideo() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		HandleAdResult(AdResults.Finished, AdType.NonSkippableVideo);
		return;
#endif
		StartCoroutine(ShowNonSkipVideoProcess());
	}

	#endregion


	#region Process Ad

	private IEnumerator ShowInterstitialProcess() {
		if (_isTesting) print("ShowInterstitialProcess");
		yield return new WaitUntil(() => Appodeal.isLoaded(Appodeal.INTERSTITIAL));
		Appodeal.show(Appodeal.INTERSTITIAL);
	}


	private IEnumerator ShowBannerProcess() {
		yield return new WaitUntil(() => Appodeal.isLoaded(Appodeal.BANNER));
		Appodeal.show(Appodeal.BANNER);
	}


	private IEnumerator ShowBottomBannerProcess() {
		yield return new WaitUntil(() => Appodeal.isLoaded(Appodeal.BANNER_BOTTOM));
		if(MenuController.LastTransition != MenuKinds.MainMenu) yield break;

		Appodeal.show(Appodeal.BANNER_BOTTOM);
	}


	private IEnumerator ShowVideoProcess() {
		yield return new WaitUntil(() => Appodeal.isLoaded(Appodeal.REWARDED_VIDEO));
		Appodeal.show(Appodeal.REWARDED_VIDEO);
	}


	private IEnumerator ShowNonSkipVideoProcess() {
		yield return new WaitUntil(() => Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO));
		Appodeal.show(Appodeal.NON_SKIPPABLE_VIDEO);
	}

	#endregion


	#region Interstitial Callbacks

	public void onInterstitialFailedToLoad() {
		if (_isTesting) Debug.Log("Interstitial load failed");
		HandleAdResult(AdResults.Failed, AdType.Interstitial);
	}


	public void onInterstitialShowFailed() {
		if (_isTesting) Debug.Log("Interstitial show failed");
		HandleAdResult(AdResults.Failed, AdType.Interstitial);
	}


	public void onInterstitialExpired() {
		HandleAdResult(AdResults.Failed, AdType.Interstitial);
	}


	public void onInterstitialLoaded(bool isPrecache) {
		if (_isTesting) Debug.Log("Interstitial is loaded");
	}


	public void onInterstitialClicked() { }


	public void onInterstitialClosed() {
		HandleAdResult(AdResults.Finished, AdType.Interstitial);
	}


	public void onInterstitialShown() { }

	#endregion


	#region Rewarded Video Callbacks

	public void onRewardedVideoLoaded(bool precache) {
		if (_isTesting) Debug.Log("Rewarded video is loaded");
	}


	public void onRewardedVideoFailedToLoad() {
		if (_isTesting) Debug.Log("Rewarded video load failed");
		HandleAdResult(AdResults.Failed, AdType.RewardedVideo);
	}


	public void onRewardedVideoShowFailed() {
		if (_isTesting) Debug.Log("Rewarded video show failed");
		HandleAdResult(AdResults.Failed, AdType.RewardedVideo);
	}


	public void onRewardedVideoShown() { }


	public void onRewardedVideoFinished(double amount, string name) {
		HandleAdResult(AdResults.Finished, AdType.RewardedVideo);
	}


	public void onRewardedVideoClosed(bool finished) {
		HandleAdResult(finished ? AdResults.Finished : AdResults.Skipped, AdType.RewardedVideo);
	}


	public void onRewardedVideoExpired() {
		HandleAdResult(AdResults.Failed, AdType.RewardedVideo);
	}


	public void onRewardedVideoClicked() { }

	#endregion


	#region Non Skippable Video Callbacks

	public void onNonSkippableVideoLoaded(bool isPrecache) { }


	public void onNonSkippableVideoFailedToLoad() {
		HandleAdResult(AdResults.Failed, AdType.NonSkippableVideo);
	}


	public void onNonSkippableVideoShowFailed() {
		HandleAdResult(AdResults.Failed, AdType.NonSkippableVideo);
	}


	public void onNonSkippableVideoShown() { }


	public void onNonSkippableVideoFinished() {
		HandleAdResult(AdResults.Finished, AdType.NonSkippableVideo);
	}


	public void onNonSkippableVideoClosed(bool finished) {
		HandleAdResult(finished ? AdResults.Finished : AdResults.Skipped, AdType.NonSkippableVideo);
	}


	public void onNonSkippableVideoExpired() {
		HandleAdResult(AdResults.Failed, AdType.NonSkippableVideo);
	}

	#endregion


	#region Banner Callbacks

	public void onBannerLoaded(int height, bool isPrecache) {
		if (_isTesting) print($"Banner Is Loaded ({height})");
	}


	public void onBannerFailedToLoad() {
		if (_isTesting) print("Failed to load Banner");
	}


	public void onBannerShown() { }

	public void onBannerClicked() { }

	public void onBannerExpired() { }

	#endregion
}