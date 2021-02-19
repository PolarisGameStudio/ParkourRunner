using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Adverty.Native;

namespace Adverty.PlatformSpecific
{
    public class AndroidRenderingPlugin : IAndroidRenderingPlugin
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        [DllImport("glbridge")]
        private static extern void SetGlIssuePluginEventMethod(IntPtr glIssuePluginMethod, int renderMode);

        [DllImport("glbridge")]
        private static extern void IssuePluginEvent(int eventId);

        [DllImport("glbridge")]
        private static extern void CustomRenderEvent(int eventId);

        [DllImport("glbridge")]
        private static extern void SetNativeTexture(IntPtr handler, int width, int height, int id);

        [DllImport("glbridge")]
        private static extern void DestroyNativeTexture(int textureId);

        public void NativeSetGlIssuePluginEventMethod(IntPtr glIssuePluginMethod, int renderMode)
        {
            SetGlIssuePluginEventMethod(glIssuePluginMethod, renderMode);
        }

        public void NativeIssuePluginEvent(int eventId)
        {
            IssuePluginEvent(eventId);
        }

        public void Render(int eventId)
        {
            CustomRenderEvent(eventId);
        }

        public void SendTexture(IntPtr handler, int width, int height, int id)
        {
            SetNativeTexture(handler, width, height, id);
        }

        public void DestroyTexture(int id)
        {
            DestroyNativeTexture(id);
        }
#else
        public void NativeSetGlIssuePluginEventMethod(IntPtr glIssuePluginMethod, int renderMode)
        {
        }

        public void NativeIssuePluginEvent(int eventId)
        {
        }

        public void Render(int eventId)
        {
        }

        public void SendTexture(IntPtr handler, int width, int height, int id)
        {
        }

        public void DestroyTexture(int id)
        {
        }
#endif
    }
}