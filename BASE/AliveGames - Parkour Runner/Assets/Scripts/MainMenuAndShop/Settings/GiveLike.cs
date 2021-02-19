using UnityEngine;
using ParkourRunner.Scripts.Managers;
using AEngine;

public class GiveLike : SettingsBase {
	private const string Key = "Liked";
	public static bool Liked => PlayerPrefs.GetInt(Key, 0) == 1;


    public override void OnClick()
    {
        AudioManager.Instance.PlaySound(Sounds.Tap);
		PlayerPrefs.SetInt(Key, 1);
		PlayerPrefs.Save();
        OpenStore();
		gameObject.SetActive(false);
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