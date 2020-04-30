//using UnityEngine;

using System.Collections;
using GoogleMobileAds.Api;
using Managers.Advertising;
using UnityEngine;

public class AdMobController : BaseAdController
{
    private const string GAME_ID_IOS = "ca-app-pub-9017460316126624~2209772973";
    private const string INTERSTITIAL_IOS_ID = "ca-app-pub-9017460316126624/9208573399";
    private const string GAME_ID_ANDROID = "ca-app-pub-9017460316126624~4331148594";
    private const string INTERSTITIAL_ANDROID_ID = "ca-app-pub-9017460316126624/3516760372";

    private InterstitialAd _ad;

    public override void Initialize()
    {

#if UNITY_IPHONE || UNITY_IOS
        MobileAds.Initialize(GAME_ID_IOS);
        _ad = new InterstitialAd(INTERSTITIAL_IOS_ID);
#elif UNITY_ANDROID
        MobileAds.Initialize(GAME_ID_ANDROID);
        _ad = new InterstitialAd(INTERSTITIAL_ANDROID_ID);
#elif UNITY_EDITOR
        MobileAds.Initialize(GAME_ID_IOS);
        _ad = new InterstitialAd(INTERSTITIAL_IOS_ID);
#endif

        AdRequest request = new AdRequest.Builder().Build();
        _ad.LoadAd(request);

        // Без награды. Это не Rewarded, по идее видео не должен просто так закрыть пока не закончится, так что любой показ - как награда.
        _ad.OnAdClosed -= OnInterstitialClosed;
        _ad.OnAdFailedToLoad -= OnInterstitialFailed;
    }


    public override bool InterstitialIsLoaded() {
        return _ad.IsLoaded();
    }


    public override bool RewardedVideoLoaded() {
        throw new System.NotImplementedException();
    }


    public override bool NonSkippableVideoIsLoaded() {
        throw new System.NotImplementedException();
    }


    public override void ShowInterstitial() {
        StartCoroutine(ShowInterstitialProcess());
    }


    public override void ShowBanner() {
        throw new System.NotImplementedException();
    }


    public override void ShowBottomBanner() {
        throw new System.NotImplementedException();
    }


    public override void HideBottomBanner() {
        throw new System.NotImplementedException();
    }


    public override void ShowRewardedVideo() {
        throw new System.NotImplementedException();
    }


    public override void ShowNonSkippableVideo() {
        throw new System.NotImplementedException();
    }


    private IEnumerator ShowInterstitialProcess() {
        yield return new WaitUntil(() => _ad.IsLoaded());
        _ad.Show();
    }

    #region Events
    private void OnInterstitialClosed(object sender, System.EventArgs args)
    {
        HandleAdResult(AdResults.Finished, AdType.Interstitial);
    }

    private void OnInterstitialFailed(object sender, System.EventArgs args)
    {
        HandleAdResult(AdResults.Failed, AdType.Interstitial);
    }
    #endregion
}