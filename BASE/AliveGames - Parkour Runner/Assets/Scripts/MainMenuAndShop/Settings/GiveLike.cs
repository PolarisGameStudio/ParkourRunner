using UnityEngine;
using ParkourRunner.Scripts.Managers;
using AEngine;

public class GiveLike : SettingsBase
{
    public override void OnClick()
    {
        AudioManager.Instance.PlaySound(Sounds.Tap);
        OpenStore();
    }

    public static void OpenStore()
    {
#if UNITY_IPHONE || UNITY_IOS
        Application.OpenURL(StaticConst.IOS_URL);
#elif UNITY_ANDROID
        Application.OpenURL(StaticConst.ANDROID_URL);
#elif UNITY_EDITOR
        Application.OpenURL(StaticConst.IOS_URL);
#endif
    }
}