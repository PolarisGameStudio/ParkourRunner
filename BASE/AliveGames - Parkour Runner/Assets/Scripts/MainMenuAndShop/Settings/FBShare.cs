using Facebook.Unity;
using UnityEngine;

namespace MainMenuAndShop.Settings {
	public class FBShare : MonoBehaviour {
		public void OnShareBtn() {
			Managers.Facebook.Share();
		}
	}
}