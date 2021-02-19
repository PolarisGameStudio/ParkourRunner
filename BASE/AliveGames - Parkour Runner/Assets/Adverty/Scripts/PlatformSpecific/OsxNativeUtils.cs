using System;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class OsxUtils : IOsxNativeUtils
    {
#if UNITY_EDITOR_OSX || !UNITY_EDITOR && UNITY_STANDALONE_OSX
        [DllImport("utils")]
        private static extern string _Utils_GetSystemLocale();

        public string GetSystemLocale()
        {
            return _Utils_GetSystemLocale();
        }
#else
        public string GetSystemLocale()
        {
            return null;
        }
#endif
    }
}
