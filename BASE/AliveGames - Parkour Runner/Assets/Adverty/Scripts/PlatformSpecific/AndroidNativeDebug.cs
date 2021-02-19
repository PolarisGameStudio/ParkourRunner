using Adverty.Native;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class AndroidNativeDebug : IAndroidNativeDebug
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        [DllImport("glbridge")]
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
