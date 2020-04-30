using System;
using Assets.ParkourRunner.Scripts.Track.Generator;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Managers.Advertising {
	public abstract class BaseAdController : MonoBehaviour {
		public static event Action OnShowAdsEvent;

		private AdResultCallback InterstitialResultCallback      = new AdResultCallback();
		private AdResultCallback RewardedResultCallback          = new AdResultCallback();
		private AdResultCallback NonSkippableVideoResultCallback = new AdResultCallback();

		public abstract void Initialize();

		public abstract bool InterstitialIsLoaded();
		public abstract bool RewardedVideoLoaded();
		public abstract bool NonSkippableVideoIsLoaded();

		public abstract void ShowInterstitial();
		public abstract void ShowBanner();
		public abstract void ShowBottomBanner();
		public abstract void HideBottomBanner();
		public abstract void ShowRewardedVideo();
		public abstract void ShowNonSkippableVideo();


		public void InitCallbackHandlers(Action finishedCallback, Action skippedCallback, Action failedCallback,
										AdType  adType) {
			var result = GetCallbacks(adType);

			result.FinishedCallback = finishedCallback;
			result.SkippedCallback  = skippedCallback;
			result.FailedCallback   = failedCallback;
		}


		private AdResultCallback GetCallbacks(AdType adType) {
			switch (adType) {
				case AdType.Interstitial:      return InterstitialResultCallback;
				case AdType.RewardedVideo:     return RewardedResultCallback;
				case AdType.NonSkippableVideo: return NonSkippableVideoResultCallback;
				default:
					throw new ArgumentOutOfRangeException(nameof(adType), adType, null);
			}
		}


		public void HandleAdResult(AdResults result, AdType adType) {
			var callbacks = GetCallbacks(adType);
			switch (result) {
				case AdResults.Finished:
					callbacks.FinishedCallback.SafeInvoke();
					OnShowAdsEvent.SafeInvoke();
					break;
				case AdResults.Skipped:
					callbacks.SkippedCallback.SafeInvoke();
					break;
				case AdResults.Failed:
					callbacks.FailedCallback.SafeInvoke();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(result), result, null);
			}
			callbacks.Reset();
		}
	}

	public class AdResultCallback {
		public Action FinishedCallback;
		public Action FailedCallback;
		public Action SkippedCallback;


		public void Reset() {
			FinishedCallback = null;
			FailedCallback   = null;
			SkippedCallback  = null;
		}
	}

	public enum AdType {
		Interstitial,
		RewardedVideo,
		NonSkippableVideo,
		Banner,
		BannerBottom,
	}

	public enum AdResults {
		Finished,
		Skipped,
		Failed
	}
}