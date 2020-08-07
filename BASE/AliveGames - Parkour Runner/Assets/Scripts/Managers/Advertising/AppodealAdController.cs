using System;
using System.Collections;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using ConsentManager.Api;
using ConsentManager.Common;
using Managers;
using Managers.Advertising;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppodealAdController : BaseAdController, IInterstitialAdListener, IRewardedVideoAdListener,
									INonSkippableVideoAdListener, IBannerAdListener, IConsentInfoUpdateListener,
									IConsentFormListener {
	private const int MAX_FRAMES_TO_INTERSTITIAL = 2;

	[SerializeField] private string _androidAppKey;
	[SerializeField] private string _iosAppKey;
	[SerializeField] private bool   _isTesting;

	private Coroutine   _bottomBanner;
	private ConsentForm _consentForm;

	private string AppKey {
		get {
			var appKey = _iosAppKey;
			if (Application.platform != RuntimePlatform.IPhonePlayer &&
				Application.platform != RuntimePlatform.OSXPlayer) {
				appKey = _androidAppKey;
			}
			return appKey;
		}
	}


	public override void Initialize() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		return;
#endif

		if (!AdManager.EnableAds) return;

		InitializeConsentManager();
	}


	private void InitializeAppodeal() {
		Appodeal.setTesting(_isTesting);
		Appodeal.setSmartBanners(true);
		Appodeal.setTabletBanners(true);

		// Appodeal.setLogLevel(Appodeal.LogLevel.Verbose);

		var consent = ConsentManager.Api.ConsentManager.getInstance().getConsent();
		print(consent);
		if (consent != null) {
			Appodeal.initialize(AppKey,
								Appodeal.INTERSTITIAL | Appodeal.BANNER_BOTTOM | Appodeal.REWARDED_VIDEO |
								Appodeal.NON_SKIPPABLE_VIDEO, consent);
		}
		else {
			Appodeal.initialize(AppKey,
								Appodeal.INTERSTITIAL | Appodeal.BANNER_BOTTOM | Appodeal.REWARDED_VIDEO |
								Appodeal.NON_SKIPPABLE_VIDEO);
		}
		Appodeal.cache(Appodeal.INTERSTITIAL | Appodeal.BANNER_BOTTOM | Appodeal.REWARDED_VIDEO |
						Appodeal.NON_SKIPPABLE_VIDEO);

		Appodeal.setInterstitialCallbacks(this);
		Appodeal.setRewardedVideoCallbacks(this);
		Appodeal.setNonSkippableVideoCallbacks(this);
		// Appodeal.setBannerCallbacks(this);
	}


	private void InitializeConsentManager() {
		var consentManager = ConsentManager.Api.ConsentManager.getInstance();

		var consent           = consentManager.getConsent();
		var consentZone       = consentManager.getConsentZone();
		var consentStatus     = consentManager.getConsentStatus();
		var consentShouldShow = consentManager.shouldShowConsentDialog();

		print($"consentShouldShow: {consentShouldShow}");
		if (true || consentShouldShow == Consent.ShouldShow.TRUE) {
			// show dialog
			ShowConsent();
		}
		consentManager.requestConsentInfoUpdate(AppKey, this);
	}


	private void ShowConsent() {
		try {
			_consentForm = new ConsentForm.Builder().withListener(this).build();
			_consentForm?.load();
		}
		catch (Exception e) {
			Debug.LogError(e);
			InitializeAppodeal();
		}
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
		if (_bottomBanner != null) return;
		_bottomBanner = StartCoroutine(ShowBottomBannerProcess());
	}


	public override void HideBottomBanner() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		return;
#endif
		if (_bottomBanner != null) StopCoroutine(_bottomBanner);
		Appodeal.hide(Appodeal.BANNER_BOTTOM);
		_bottomBanner = null;
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
		yield return new WaitUntil(() => Appodeal.isLoaded(Appodeal.INTERSTITIAL));
		Appodeal.show(Appodeal.INTERSTITIAL);
		AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.interstitial_shown);
	}


	private IEnumerator ShowBannerProcess() {
		yield return new WaitUntil(() => Appodeal.isLoaded(Appodeal.BANNER));
		Appodeal.show(Appodeal.BANNER);
		AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.banner_shown);
	}


	private IEnumerator ShowBottomBannerProcess() {
		yield return new WaitUntil(() => Appodeal.isLoaded(Appodeal.BANNER_BOTTOM));
		Appodeal.show(Appodeal.BANNER_BOTTOM);
		AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.banner_shown);
	}


	private IEnumerator ShowVideoProcess() {
		yield return new WaitUntil(() => Appodeal.isLoaded(Appodeal.REWARDED_VIDEO));
		Appodeal.show(Appodeal.REWARDED_VIDEO);
		AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.rewarded_video_shown);
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


	#region ConsentInfoUpdateListener

	public void onConsentInfoUpdated(Consent consent) {
		print("onConsentInfoUpdated");
	}


	public void onFailedToUpdateConsentInfo(ConsentManagerException error) {
		print($"onFailedToUpdateConsentInfo Reason: {error.getReason()}");
	}

	#endregion


	#region ConsentFormListener

	public void onConsentFormLoaded() {
		_consentForm.showAsDialog();
	}


	public void onConsentFormError(ConsentManagerException exception) {
		print($"ConsentFormListener - onConsentFormError, reason - {exception.getReason()}");
	}


	public void onConsentFormOpened() {
		print("ConsentFormListener - onConsentFormOpened");
	}


	public void onConsentFormClosed(Consent consent) {
		print($"ConsentFormListener - onConsentFormClosed, consentStatus - {consent.getStatus()}");
		InitializeAppodeal();
	}

	#endregion
}