using System.Collections;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

public class AppodealAdController : BaseAdController, IInterstitialAdListener
{
    private const int MAX_FRAMES_TO_INTERSTITIAL = 2;

    [SerializeField] private string _androidAppKey;
    [SerializeField] private string _iosAppKey;
    [SerializeField] private bool _isTesting;
    
    public override void Initialize()
    {
        Appodeal.setTesting(_isTesting);
        
#if UNITY_IPHONE || UNITY_IOS
        //Appodeal.initialize(_iosAppKey, Appodeal.NON_SKIPPABLE_VIDEO | Appodeal.INTERSTITIAL);
        Appodeal.initialize(_iosAppKey, Appodeal.INTERSTITIAL);
#elif UNITY_ANDROID
        Appodeal.initialize(_androidAppKey, Appodeal.INTERSTITIAL);
#endif

        Appodeal.setInterstitialCallbacks(this);
    }

    public override bool IsAvailable()
    {
        return true;
    }
        
    public override void ShowAdvertising()
    {
        StartCoroutine(ShowAdsProcess());

        //if (Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO))
        //    Appodeal.show(Appodeal.NON_SKIPPABLE_VIDEO);
        //else
        //    Appodeal.show(Appodeal.INTERSTITIAL);
    }

    private IEnumerator ShowAdsProcess()
    {
        yield return new WaitWhile(() => !Appodeal.isLoaded(Appodeal.INTERSTITIAL));

        Appodeal.show(Appodeal.INTERSTITIAL);
    }

    #region Interface
    // IInterstitialAdListener
    public void onInterstitialFailedToLoad()
    {
        //HandleAdResult(UnityEngine.Advertisements.ShowResult.Failed);
        HandleAdResult(AdResults.Failed);
    }

    public void onInterstitialExpired()
    {
        //HandleAdResult(UnityEngine.Advertisements.ShowResult.Failed);
        HandleAdResult(AdResults.Failed);
    }

    public void onInterstitialLoaded(bool isPrecache) { }

    public void onInterstitialClicked() { }

    public void onInterstitialClosed()
    {
        //HandleAdResult(UnityEngine.Advertisements.ShowResult.Finished);
        HandleAdResult(AdResults.Finished);
    }

    public void onInterstitialShown()
    {
        //HandleAdResult(UnityEngine.Advertisements.ShowResult.Finished);
        HandleAdResult(AdResults.Finished);
    }
    #endregion
}