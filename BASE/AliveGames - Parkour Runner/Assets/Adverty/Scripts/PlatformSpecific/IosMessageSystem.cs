using Adverty.Native;
using System;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class IosMessageSystem : IIosMessageSystem
    {
#if !UNITY_EDITOR && UNITY_IOS

        [DllImport("__Internal")]
        private static extern void AdvertySetNativeEventsCallback(IntPtr callbackDelegate);

        [DllImport("__Internal")]
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