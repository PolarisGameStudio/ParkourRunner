using System;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class IosWebViewTexture : IIosWebViewTexture
    {
#if !UNITY_EDITOR && UNITY_IOS
        [DllImport("__Internal")]
        private static extern void AdvertyWebViewInitialize(int loadHtmlTimeout);

        [DllImport("__Internal")]
        private static extern IntPtr AdvertyWebViewTextureCreate(IntPtr textureId, int width, int height, int eventListenerId);

        [DllImport("__Internal")]
        private static extern void AdvertyWebViewTextureUpdateTexture(IntPtr instance);

        [DllImport("__Internal")]
        private static extern void AdvertyWebViewTextureLoadUrl(IntPtr instance, string url);

        [DllImport("__Internal")]
        private static extern void AdvertyWebViewTextureLoadHtml(IntPtr instance, string html);

        [DllImport("__Internal")]
        private static extern void AdvertyWebViewTextureDestroy(IntPtr instance);

        [DllImport("__Internal")]
        private static extern void AdvertyWebViewTextureRunJavaScript(IntPtr instance, string javaScriptString);

        public void Initialize(int loadHtmlTimeout)
        {
            AdvertyWebViewInitialize(loadHtmlTimeout);
        }

        public IntPtr NativeAdvertyWebViewTextureCreate(IntPtr textureId, int width, int height, int eventListenerId)
        {
            return AdvertyWebViewTextureCreate(textureId, width, height, eventListenerId);
        }

        public void NativeAdvertyWebViewTextureUpdateTexture(IntPtr instance)
        {
           AdvertyWebViewTextureUpdateTexture(instance);
        }

        public void NativeAdvertyWebViewTextureLoadUrl(IntPtr instance, string url)
        {
            AdvertyWebViewTextureLoadUrl(instance, url);
        }

        public void NativeAdvertyWebViewTextureLoadHtml(IntPtr instance, string html)
        {
            AdvertyWebViewTextureLoadHtml(instance, html);
        }

        public void NativeAdvertyWebViewTextureDestroy(IntPtr instance)
        {
            AdvertyWebViewTextureDestroy(instance);
        }

        public void NativeAdvertyWebViewTextureRunJavaScript(IntPtr instance, string javaScriptString)
        {
            AdvertyWebViewTextureRunJavaScript(instance, javaScriptString);
        }
#else
        public void Initialize(int loadHtmlTimeout)
        {
        }

        public IntPtr NativeAdvertyWebViewTextureCreate(IntPtr textureId, int width, int height, int eventListenerId)
        {
            return IntPtr.Zero;
        }

        public void NativeAdvertyWebViewTextureUpdateTexture(IntPtr instance)
        {
        }

        public void NativeAdvertyWebViewTextureLoadUrl(IntPtr instance, string url)
        {
        }

        public void NativeAdvertyWebViewTextureLoadHtml(IntPtr instance, string html)
        {
        }

        public void NativeAdvertyWebViewTextureRunJavaScript(IntPtr instance, string javaScriptString)
        {
        }

        public void NativeAdvertyWebViewTextureDestroy(IntPtr instance)
        {
        }
#endif
    }
}

