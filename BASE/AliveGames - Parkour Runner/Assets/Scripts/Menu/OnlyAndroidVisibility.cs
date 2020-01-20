using UnityEngine;

public class OnlyAndroidVisibility : MonoBehaviour
{
    void Start()
    {
#if !UNITY_ANDROID
        this.gameObject.SetActive(false);
#endif
    }
}