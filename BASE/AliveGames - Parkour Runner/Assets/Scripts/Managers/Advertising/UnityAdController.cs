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

    public override bool IsAvailable()
    {
        /*
        //return Advertisement.isSupported && Advertisement.IsReady();
        return Advertisement.IsReady();
        */
        return true;
    }

    public override void ShowAdvertising()
    {
        /*
        Advertisement.Show(new ShowOptions() { resultCallback = HandleAdResult });
        */
    }
}