//using UnityEngine;
using GoogleMobileAds.Api;

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
        _ad.OnAdClosed -= OnAdClosed;
        _ad.OnAdFailedToLoad -= OnAdFailed;
        _ad.OnAdClosed += OnAdClosed;
        _ad.OnAdFailedToLoad += OnAdFailed;
    }

    public override bool IsAvailable()
    {
        return _ad.IsLoaded();
    }

    public override void ShowAdvertising()
    {
        _ad.Show();
    }

    #region Events
    private void OnAdClosed(object sender, System.EventArgs args)
    {
        HandleAdResult(UnityEngine.Advertisements.ShowResult.Finished);
    }

    private void OnAdFailed(object sender, System.EventArgs args)
    {
        HandleAdResult(UnityEngine.Advertisements.ShowResult.Finished);
    }
    #endregion
}