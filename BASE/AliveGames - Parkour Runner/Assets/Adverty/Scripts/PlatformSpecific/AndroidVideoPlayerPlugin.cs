using System;
using System.Runtime.InteropServices;
using Adverty.Native;

namespace Adverty.PlatformSpecific
{
    public class AndroidVideoPlayerPlugin : IAndroidVideoPlayerPlugin 
    {
        #if !UNITY_EDITOR && UNITY_ANDROID
        [DllImport("glbridge")]
        private static extern IntPtr CreateVideoPlayer(IntPtr jniEnv, int listenerId, string url, IntPtr surfaceTexture, IntPtr surface);

        [DllImport("glbridge")]
        private static extern void VideoPlayerPrepare(IntPtr videoPlayer);

        [DllImport("glbridge")]
        private static extern void VideoPlayerPlay(IntPtr videoPlayer);

        [DllImport("glbridge")]
        private static extern void VideoPlayerPause(IntPtr videoPlayer);

        [DllImport("glbridge")]
        private static extern void VideoPlayerSetOnPreparedMethod(IntPtr onPreparedMethod);

        [DllImport("glbridge")]
        private static extern void VideoPlayerSetOnFrameAvailableMethod(IntPtr onFrameAvailableMethod);

        public IntPtr NativeCreateVideoPlayer(IntPtr jniEnv, int listenerId, string url, IntPtr surface, IntPtr surfaceTexture)
        {
            return CreateVideoPlayer(jniEnv, listenerId, url, surface, surfaceTexture);
        }

        public void NativeVideoPlayerPrepare(IntPtr videoPlayer)
        {
            VideoPlayerPrepare(videoPlayer);
        }

        public void NativeVideoPlayerPlay(IntPtr videoPlayer)
        {
            VideoPlayerPlay(videoPlayer);
        }

        public void NativeVideoPlayerPause(IntPtr videoPlayer)
        {
            VideoPlayerPause(videoPlayer);
        }

        public void NativeVideoPlayerSetOnPreparedMethod(IntPtr onPreparedMethod)
        {
            VideoPlayerSetOnPreparedMethod(onPreparedMethod);
        }

        public void NativeVideoPlayerSetOnFrameAvailableMethod(IntPtr onFrameAvailableMethod)
        {
            VideoPlayerSetOnFrameAvailableMethod(onFrameAvailableMethod);
        }
        #else
        public IntPtr NativeCreateVideoPlayer(IntPtr jniEnv, int listenerId, string url, IntPtr surface, IntPtr surfaceTexture)
        {
            return IntPtr.Zero;
        }

        public void NativeVideoPlayerPrepare(IntPtr videoPlayer)
        {
        }

        public void NativeVideoPlayerPlay(IntPtr videoPlayer)
        {
        }

        public void NativeVideoPlayerPause(IntPtr videoPlayer)
        {
        }

        public void NativeVideoPlayerSetOnPreparedMethod(IntPtr onPreparedMethod)
        {
        }

        public void NativeVideoPlayerSetOnFrameAvailableMethod(IntPtr onFrameAvailableMethod)
        {
        }
        #endif
    }
}