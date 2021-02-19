using System;
using Adverty.Native;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class AndroidMessageSystem : IAndroidMessageSystem
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        [DllImport("glbridge")]
        private static extern void AdvertySetNativeEventsCallback(IntPtr callbackDelegate);

        [DllImport("glbridge")]
        private static extern void AdvertySetNativeEventsCallbackWithData(IntPtr callbackDelegate);

        public void SetNativeEventsCallback(IntPtr callbackDelegate)
        {
            AdvertySetNativeEventsCallback(callbackDelegate);
        }

        public void SetNativeEventsCallbackWithData(IntPtr callbackDelegate)
        {
            AdvertySetNativeEventsCallbackWithData(callbackDelegate);
        }
#else
        public void SetNativeEventsCallback(IntPtr callbackDelegate)
        {
        }

        public void SetNativeEventsCallbackWithData(IntPtr callbackDelegate)
        {
        }
#endif
    }
}

