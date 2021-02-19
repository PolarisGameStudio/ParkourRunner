using System;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class IosWebView : IIosWebView
    {
#if !UNITY_EDITOR && UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr AdvertyWebViewOpen(int id, string url, string closeButtonPath, string backButtonPath, string forwardButtonPath, string backgroundButtonPath, bool shouldCloseWhenDone);

        [DllImport("__Internal")]
		private static extern void AdvertyWebViewClose(IntPtr instance);

        [DllImport("__Internal")]
        private static extern IntPtr AdvertyWebViewSetVisibility(IntPtr instance, bool visibility);

        [DllImport("__Internal")]
        private static extern void AdvertyWebViewLoadUrl(IntPtr instance, string url);

        [DllImport("__Internal")]
        private static extern void AdvertyWebViewMute(IntPtr instance);

        [DllImport("__Internal")]
        private static extern void AdvertyWebViewSetScrollEnabled(IntPtr instance, bool isScrollEnabled);

        [DllImport("__Internal")]
        private static extern void AdvertyWebViewSetPageNavigationEnabled(IntPtr instance, bool isNavigationEnabled);

        public IntPtr NativeAdvertyWebViewOpen(int id, string url, string closeButtonPath, string backButtonPath, string forwardButtonPath, string backgroundButtonPath, bool shouldCloseWhenDone)
        {
            return AdvertyWebViewOpen(id, url, closeButtonPath, backButtonPath, forwardButtonPath, backgroundButtonPath, shouldCloseWhenDone);
        }

        public void NativeAdvertyWebViewClose(IntPtr instance)
        {
            AdvertyWebViewClose(instance);
        }

        public void NativeAdvertyWebViewSetVisibility(IntPtr instance, bool visibility)
        {
            AdvertyWebViewSetVisibility(instance, visibility);
        }

        public void NativeAdvertyWebViewLoadUrl(IntPtr instance, string url)
        {
            AdvertyWebViewLoadUrl(instance, url);
        }

        public void NativeAdvertyWebViewMute(IntPtr instance)
        {
            AdvertyWebViewMute(instance);
        }

        public void NativeAdvertyWebViewSetScrollEnabled(IntPtr instance, bool isScrollEnabled)
        {
            AdvertyWebViewSetScrollEnabled(instance, isScrollEnabled);
        }

        public void NativeAdvertyWebViewSetPageNavigationEnabled(IntPtr instance, bool isNavigationEnabled)
        {
            AdvertyWebViewSetPageNavigationEnabled(instance, isNavigationEnabled);
        }
#else
        public IntPtr NativeAdvertyWebViewOpen(int id, string url, string closeButtonPath, string backButtonPath, string forwardButtonPath, string backgroundButtonPath, bool shouldCloseWhenDone)
        {
            return IntPtr.Zero;
        }
		       
        public void NativeAdvertyWebViewClose(IntPtr instance)
        {
        }
			
        public void NativeAdvertyWebViewSetVisibility(IntPtr instance, bool visibility)
        {
        }

        public void NativeAdvertyWebViewLoadUrl(IntPtr instance, string url)
        {
        }

        public void NativeAdvertyWebViewMute(IntPtr instance)
        {
        }

        public void NativeAdvertyWebViewSetScrollEnabled(IntPtr instance, bool isScrollEnabled)
        {
        }

        public void NativeAdvertyWebViewSetPageNavigationEnabled(IntPtr instance, bool isNavigationEnabled)
        {
        }
        #endif
    }
}

