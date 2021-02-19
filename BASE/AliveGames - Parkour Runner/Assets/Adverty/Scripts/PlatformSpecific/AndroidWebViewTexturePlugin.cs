using Adverty.Native;
using System;
using System.Runtime.InteropServices;

namespace Adverty.PlatformSpecific
{
    public class AndroidWebViewTextureNativeObject : IAndroidWebViewTextureNativeObject
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        [DllImport("glbridge")]
        private static extern IntPtr getCustomRenderEventFunc();

        [DllImport("glbridge")]
        private static extern bool setOnClicked(IntPtr ptr, IntPtr onClickedDelegate);

        [DllImport("glbridge")]
        private static extern void setOnPageStarted(IntPtr ptr, IntPtr onPageStartedDelegate);

        [DllImport("glbridge")]
        private static extern void setOnPageFinished(IntPtr ptr, IntPtr onPageFinishedDelegate);

        [DllImport("glbridge")]
        private static extern void setOnReceivedError(IntPtr ptr, IntPtr onReceivedErrorDelegate);

        [DllImport("glbridge")]
        private static extern void setOnCreated(IntPtr ptr, IntPtr onCreatedDelegate);

        [DllImport("glbridge")]
        private static extern void setOnTextureCheckPassed(IntPtr ptr, IntPtr onTextureCheckPassedDelegate);

        [DllImport("glbridge")]
        private static extern void setOnHtmlLoadTimeout(IntPtr ptr, IntPtr onHtmlLoadTimeoutDelegate);

        [DllImport("glbridge")]
        private static extern void loadData(IntPtr ptr, string data);

        [DllImport("glbridge")]
        private static extern void loadUrl(IntPtr ptr, string url);

        [DllImport("glbridge")]
        private static extern void setRenderingActive(IntPtr ptr, bool active);

        [DllImport("glbridge")]
        private static extern void destroy(IntPtr ptr);

        [DllImport("glbridge")]
        private static extern IntPtr create(int cbo, int width, int height, int loadTimeout);

        [DllImport("glbridge")]
        private static extern void touch(IntPtr ptr, float x, float y);


        public IntPtr GetCustomRenderEventFunc()
        {
            return getCustomRenderEventFunc();
        }

        public IntPtr Create(int cbo, int width, int height, int loadTimeout)
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

        public void SetOnTextureCheckPassed(IntPtr ptr, IntPtr onTextureCheckPassedDelegate)
        {
            setOnTextureCheckPassed(ptr, onTextureCheckPassedDelegate);
        }

        public void SetOnHtmlLoadTimeout(IntPtr ptr, IntPtr onHtmlLoadTimeoutDelegate)
        {
            setOnHtmlLoadTimeout(ptr, onHtmlLoadTimeoutDelegate);
        }

        public void SetOnCreate(IntPtr ptr, IntPtr onCreateDelegate)
        {
            setOnCreated(ptr, onCreateDelegate);
        }

        public void SetOnPageFinished(IntPtr ptr, IntPtr onPageFinishedDelegate)
        {
            setOnPageFinished(ptr, onPageFinishedDelegate);
        }

        public void SetOnPageStarted(IntPtr ptr, IntPtr onPageStartedDelegate)
        {
            setOnPageStarted(ptr, onPageStartedDelegate);
        }

        public void SetOnReceiveError(IntPtr ptr, IntPtr onReceivedErrorDelegate)
        {
            setOnReceivedError(ptr, onReceivedErrorDelegate);
        }

        public void SetRenderActive(IntPtr ptr, bool active)
        {
            setRenderingActive(ptr, active);
        }

        public void SetOnClicked(IntPtr ptr, IntPtr onClickedDelegate)
        {
            setOnClicked(ptr, onClickedDelegate);
        }

        public void Touch(IntPtr ptr, float x, float y)
        {
            touch(ptr, x, y);
        }


#else
       public IntPtr Create(int cbo, int width, int height, int loadTimeout)
       {
            return IntPtr.Zero;
       }

       public void Destroy(IntPtr ptr)
       {
       }

       public IntPtr GetCustomRenderEventFunc()
       {
            return IntPtr.Zero;
       }

       public void LoadData(IntPtr ptr, string data)
       {
       }

       public void LoadURL(IntPtr ptr, string url)
       {
       }

       public void SetDrawAfterScreenUpdates(IntPtr ptr, bool afterScreenUpdates)
       {
       }

       public void SetOnCreate(IntPtr ptr, IntPtr onCreateDelegate)
       {
       }

       public void SetOnPageFinished(IntPtr ptr, IntPtr onPageFinishedDelegate)
       {
       }

       public void SetOnPageStarted(IntPtr ptr, IntPtr onPageStartedDelegate)
       {
       }

       public void SetOnReceiveError(IntPtr ptr, IntPtr onReceivedErrorDelegate)
       {
       }

       public void SetRenderActive(IntPtr ptr, bool active)
       {
       }

       public void SetOnClicked(IntPtr ptr, IntPtr onClickedDelegate)
       {
       }

       public void Touch(IntPtr ptr, float x, float y)
       {
       }

        public void SetOnTextureCheckPassed(IntPtr ptr, IntPtr onTextureCheckPassedDelegate)
        {
        }

        public void SetOnHtmlLoadTimeout(IntPtr ptr, IntPtr onHtmlLoadTimeoutDelegate)
        {
        }
#endif

    }
}
