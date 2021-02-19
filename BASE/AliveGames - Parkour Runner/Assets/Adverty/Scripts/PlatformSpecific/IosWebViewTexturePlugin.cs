using Adverty.Native;
using System;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class IosWebViewTexturePlugin : IIosWebViewTextureNativeObject
    {
#if UNITY_IOS && !UNITY_EDITOR

        [DllImport("__Internal")]
        private static extern IntPtr create(IntPtr cbo, int width, int height, int timeout);

        [DllImport("__Internal")]
        private static extern IntPtr destroy(IntPtr ptr);

        [DllImport("__Internal")]
        private static extern void loadData(IntPtr ptr, string data);

        [DllImport("__Internal")]
        private static extern void loadUrl(IntPtr ptr, string url);

        [DllImport("__Internal")]
        private static extern bool setOnClickAction(IntPtr ptr, IntPtr onClickDelegate);

        [DllImport("__Internal")]
        private static extern void setOnPageFinished(IntPtr ptr, IntPtr onPageFinishedDelegate);

        [DllImport("__Internal")]
        private static extern void setOnReceivedError(IntPtr ptr, IntPtr onReceivedErrorDelegate);

        [DllImport("__Internal")]
        private static extern void setOnTextureCheckComplete(IntPtr ptr, IntPtr onTextureCheckCompleteDelegate);

        [DllImport("__Internal")]
        private static extern void setRenderingActive(IntPtr ptr, bool active);

        [DllImport("__Internal")]
        private static extern void setFramerate(IntPtr ptr, int framesPerSecond);

        [DllImport("__Internal")]
        private static extern void setOnHtmlLoadTimeout(IntPtr ptr, IntPtr onHtmlLoadTimeoutDelegate);

        [DllImport("__Internal")]
        private static extern void setDrawAfterScreenUpdates(IntPtr ptr, bool afterScreenUpdates);
        
        [DllImport("__Internal")]
        private static extern void touch(IntPtr ptr, float x, float y);

        [DllImport("__Internal")]
        private static extern void viewabilityCheck(IntPtr ptr, IntPtr checkPoints, IntPtr viewabilityCallback);


        public IntPtr Create(IntPtr cbo, int width, int height, int loadTimeout)
        {
            return create(cbo, width, height, loadTimeout);
        }

        public void Destroy(IntPtr ptr)
        {
            destroy(ptr);
        }

        public void LoadData(IntPtr ptr, string data)
        {
            loadData(ptr, data);
        }

        public void LoadURL(IntPtr ptr, string url)
        {
            loadUrl(ptr, url);
        }

        public void SetDrawScreenAfterUpdate(IntPtr ptr, bool afterScreenUpdate)
        {
            setDrawAfterScreenUpdates(ptr, afterScreenUpdate);
        }

        public void SetFramerate(IntPtr ptr, int framesPerSecond)
        {
            setFramerate(ptr, framesPerSecond);
        }

        public void SetOnHtmlLoadTimeout(IntPtr ptr, IntPtr onHtmlLoadTimeoutDelegate)
        {
            setOnHtmlLoadTimeout(ptr, onHtmlLoadTimeoutDelegate);
        }

        public void SetOnPageFinished(IntPtr ptr, IntPtr onPageFinishedDelegate)
        {
            setOnPageFinished(ptr, onPageFinishedDelegate);
        }

        public void SetOnReceiveError(IntPtr ptr, IntPtr onReceivedErrorDelegate)
        {
            setOnReceivedError(ptr, onReceivedErrorDelegate);
        }

        public void SetOnTextureCheckPassed(IntPtr ptr, IntPtr onTextureCheckPassedDelegate)
        {
            setOnTextureCheckComplete(ptr, onTextureCheckPassedDelegate);
        }

        public void SetOnClicked(IntPtr ptr, IntPtr onClickedDelegate)
        {
            setOnClickAction(ptr, onClickedDelegate);
        }

        public void SetRenderActive(IntPtr ptr, bool active)
        {
            setRenderingActive(ptr, active);
        }

        public void Touch(IntPtr ptr, float x, float y)
        {
            touch(ptr, x, y);
        }

        void IIosWebViewTextureNativeObject.ViewabilityCheck(IntPtr ptr, IntPtr checkPoints, IntPtr viewabilityCallback)
        {
            viewabilityCheck(ptr, checkPoints, viewabilityCallback);
        }
#else
        public static bool IsVersionSuitable()
        {
            return false;
        }       
        
        public IntPtr Create(IntPtr cbo, int width, int height, int loadTimeout)
        {
            return IntPtr.Zero;
        }

        public void Destroy(IntPtr ptr)
        {
        }

        public void LoadData(IntPtr ptr, string data)
        {
        }

        public void LoadURL(IntPtr ptr, string url)
        {
        }

        public void SetDrawScreenAfterUpdate(IntPtr ptr, bool afterScreenUpdate)
        {
        }

        public void SetOnClicked(IntPtr ptr, IntPtr onClickedDelegate)
        {
        }

        public void SetOnHtmlLoadTimeout(IntPtr ptr, IntPtr onHtmlLoadTimeoutDelegate)
        {
        }

        public void SetOnPageFinished(IntPtr ptr, IntPtr onPageFinishedDelegate)
        {
        }

        public void SetOnReceiveError(IntPtr ptr, IntPtr onReceivedErrorDelegate)
        {
        }

        public void SetOnTextureCheckPassed(IntPtr ptr, IntPtr onTextureCheckPassedDelegate)
        {
        }

        public void SetRenderActive(IntPtr ptr, bool active)
        {
        }

        public void SetFramerate(IntPtr ptr, int framesPerSecond)
        {
        }

        public void Touch(IntPtr ptr, float x, float y)
        {
        }

        void IIosWebViewTextureNativeObject.ViewabilityCheck(IntPtr ptr, IntPtr checkPoints, IntPtr viewabilityCallback)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
