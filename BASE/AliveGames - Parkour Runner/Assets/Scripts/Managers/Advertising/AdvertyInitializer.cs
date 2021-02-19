using Adverty;
using UnityEngine;

namespace Managers.Advertising {
	public class AdvertyInitializer : MonoBehaviour {
		private void Start() {
			AdvertySDK.Init(new UserData(AgeSegment.Unknown, Adverty.Gender.Unknown));
		}
	}
}