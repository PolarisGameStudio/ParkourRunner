using System;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class IosNativeDebug : IIosNativeDebug
    {
#if !UNITY_EDITOR && UNITY_IOS
        [DllImport("__Internal")]
        private static extern void AdvertyInitializeDebug();

        public void Initialize()
        {
            AdvertyInitializeDebug();
        }
#else
        public void Initialize()
        {
        }
#endif
    }
}
