using Adverty.Native;
using System;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class AndroidNativeLogger : IAndroidLogger
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        [DllImport("glbridge")]
        private static extern void AdvertySetNativeDebugCallback(IntPtr callbackDelegate);

        [DllImport("glbridge")]
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
