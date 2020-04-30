using Managers.Advertising;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdController : BaseAdController
{
    private const string IOS_AD_ID = "3215465";
    private const string ANDROID_AD_ID = "3215464";

    public override void Initialize()
    {
        /*
        //if (Advertisement.isSupported)
        //{
#if UNITY_IPHONE || UNITY_IOS
            Advertisement.Initialize(IOS_AD_ID);
#elif UNITY_ANDROID
            Advertisement.Initialize(ANDROID_AD_ID);
#endif
        //}
        //else
        //    Debug.Log("Advertising platform is not suported");
        */
    }


    public override bool InterstitialIsLoaded() {
        throw new System.NotImplementedException();
    }


    public override bool RewardedVideoLoaded() {
        throw new System.NotImplementedException();
    }


    public override bool NonSkippableVideoIsLoaded() {
        throw new System.NotImplementedException();
    }


    public override void ShowInterstitial() {
        // Advertisement.Show(new ShowOptions() { resultCallback = HandleAdResult });
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
}