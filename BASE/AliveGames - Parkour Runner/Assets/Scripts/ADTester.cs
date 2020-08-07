using UnityEngine;

namespace DefaultNamespace {
	public class ADTester : MonoBehaviour {
		public void OnClick() {
			AdManager.Instance.ShowRewardedVideo(() => print($"Rewarded video is finished"),
												() => print($"Rewarded video is skipped"),
												() => print($"Rewarded video is failed"));
		}
	}
}