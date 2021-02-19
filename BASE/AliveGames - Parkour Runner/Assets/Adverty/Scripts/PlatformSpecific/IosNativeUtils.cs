using System;
using System.Runtime.InteropServices;
using Adverty.Extensions;

namespace Adverty.PlatformSpecific
{
    public class IosNativeUtils : IIosNativeUtils
    {
#if !UNITY_EDITOR && UNITY_IOS
        [DllImport("__Internal")]
        private static extern void AdvertyUtilsSetUnityEventListenerId(int listenerId);

        [DllImport("__Internal")]
        private static extern string AdvertyUtilsGetSystemLocale();

        [DllImport("__Internal")]
        private static extern void AdvertyUtilsGetUserAgent();

        public string GetSystemLocale()
        {
            return AdvertyUtilsGetSystemLocale();
        }

        public void SetUnityEventListenerId(int listenerId)
        {
            AdvertyUtilsSetUnityEventListenerId(listenerId);
        }

        public void GetUserAgent()
        {
            AdvertyUtilsGetUserAgent();
        }
#else

        public string GetSystemLocale()
        {
            return null;
        }

        public void GetUserAgent()
        {
        }

        public void SetUnityEventListenerId(int listenerId)
        {
        }
#endif
    }
}

