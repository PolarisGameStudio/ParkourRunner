using System.Collections;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

public class AppodealAdController : BaseAdController, IInterstitialAdListener, IBannerAdListener
{
	private const int MAX_FRAMES_TO_INTERSTITIAL = 2;

	[SerializeField] private string _androidAppKey;
	[SerializeField] private string _iosAppKey;
	[SerializeField] private bool   _isTesting;

	private bool _failed, _success, _skipped;

    private bool EnableBanner { get; set; }


	private void Update()
    {
		if (_failed) HandleAdResult(AdResults.Failed);
		if (_success) HandleAdResult(AdResults.Finished);
		if (_skipped) HandleAdResult(AdResults.Skipped);

		_failed = _success = _skipped = false;
	}
    
	public override void Initialize()
    {
		Appodeal.setTesting(_isTesting);
        Appodeal.setSmartBanners(false);
        Appodeal.setTabletBanners(true);

#if UNITY_IPHONE || UNITY_IOS
        Appodeal.initialize(_iosAppKey, Appodeal.INTERSTITIAL | Appodeal.BANNER_BOTTOM);
#elif UNITY_ANDROID
		Appodeal.initialize(_androidAppKey, Appodeal.INTERSTITIAL | Appodeal.BANNER_BOTTOM);
#endif

        Appodeal.setInterstitialCallbacks(this);
	}
    
	public override bool IsAvailable()
    {
        return Appodeal.canShow(Appodeal.INTERSTITIAL);
	}
                
	public override void ShowAdvertising()
    {
		StartCoroutine(ShowAdsProcess());
	}
    
	private IEnumerator ShowAdsProcess()
    {
		yield return new WaitWhile(() => !Appodeal.isLoaded(Appodeal.INTERSTITIAL));
		Appodeal.show(Appodeal.INTERSTITIAL);
	}

    public void ShowBanner()
    {
        this.EnableBanner = true;
        StartCoroutine(ShowBannerProcess());
    }

    public void HideBanner()
    {
        StopCoroutine(ShowBannerProcess());
        this.EnableBanner = false;
        Appodeal.hide(Appodeal.BANNER_BOTTOM);
    }

    private IEnumerator ShowBannerProcess()
    {
        yield return new WaitWhile(() => !Appodeal.isLoaded(Appodeal.BANNER_BOTTOM) && !Appodeal.canShow(Appodeal.BANNER_BOTTOM));

        // Возможно, пока баннер загружался игрок уже перешел в другое меню где баннер не нужно показывать
        if (this.EnableBanner)
            Appodeal.show(Appodeal.BANNER_BOTTOM);
    }
    

	#region Interface
    public void onInterstitialFailedToLoad()
    {
		_failed = true;
	}


	public void onInterstitialShowFailed() {

	}


	public void onInterstitialExpired()
    {
		_failed = true;
	}
    
	public void onInterstitialLoaded(bool isPrecache) { }

	public void onInterstitialClicked() { }
    
	public void onInterstitialClosed()
    {
		_success = true;
	}
    
	public void onInterstitialShown() { }
    #endregion


    #region Banner Interface

	public void onBannerLoaded(int height, bool isPrecache) {

	}


	public void onBannerFailedToLoad()
    {
        _failed = true;
    }

    public void onBannerExpired()
    {
        _failed = true;
    }

    public void onBannerLoaded(bool isPrecache) { }

    public void onBannerShown() { }

    public void onBannerClicked() { }
    #endregion
}