using System;
using UnityEngine;

namespace Managers.Advertising.Admix {
	public class AdmixAdBundleController : MonoBehaviour {
		private static           AdmixAdBundleController Instance;
		[SerializeField] private AdmixAdBundle[]         Bundles;

		private int _lastUsedBundle = -1;


		private void Awake() {
			Instance = this;
		}


		public static void ActivateBundle(Transform banner1X2, Transform banner6X5, Transform banner32X5,
										Transform   video4X3,  Transform video16X9) {
			if (++Instance._lastUsedBundle >= Instance.Bundles.Length) Instance._lastUsedBundle = 0;
			print($"Set bundle {Instance._lastUsedBundle}");
			var bundle = Instance.Bundles[Instance._lastUsedBundle];

			if (banner1X2) {
				bundle.Banner1X2.SetPositionAndRotation(banner1X2.position, banner1X2.rotation);
				bundle.Banner1X2.localScale = banner1X2.localScale;
			}
			if (banner6X5) {
				bundle.Banner6X5.SetPositionAndRotation(banner6X5.position, banner6X5.rotation);
				bundle.Banner6X5.localScale = banner6X5.localScale;
			}
			if (banner32X5) {
				bundle.Banner32X5.SetPositionAndRotation(banner32X5.position, banner32X5.rotation);
				bundle.Banner32X5.localScale = banner32X5.localScale;
			}
			if (video4X3) {
				bundle.Video4X3.SetPositionAndRotation(video4X3.position, video4X3.rotation);
				bundle.Video4X3.localScale = video4X3.localScale;
			}
			if (video16X9) {
				bundle.Video16X9.SetPositionAndRotation(video16X9.position, video16X9.rotation);
				bundle.Video16X9.localScale = video16X9.localScale;
			}
		}
	}
}