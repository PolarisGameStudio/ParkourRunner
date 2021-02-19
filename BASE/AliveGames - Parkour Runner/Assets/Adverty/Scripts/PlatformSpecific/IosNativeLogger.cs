using Adverty.Native;
using System;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class IosNativeLogger : IIosLogger
    {
#if UNITY_IOS && !UNITY_EDITOR

        [DllImport("__Internal")]
        private static extern void AdvertySetNativeDebugCallback(IntPtr callbackDelegate);

        [DllImport("__Internal")]
        private static extern void AdvertySetNativeDebugWithStackTraceCallback(IntPtr callbackDelegate);

        public void SetNativeDebugCallback(IntPtr callbackDelegate)
        {
            AdvertySetNativeDebugCallback(callbackDelegate);
        }

        public void SetNativeDebugWithStackTraceCallback(IntPtr callbackDelegate)
        {
            AdvertySetNativeDebugWithStackTraceCallback(callbackDelegate);
        }
#else
        public void SetNativeDebugCallback(IntPtr callbackDelegate)
        {
        }

        public void SetNativeDebugWithStackTraceCallback(IntPtr callbackDelegate)
        {
        }
#endif
    }
}