using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

namespace Managers {
	public class Facebook : MonoBehaviour {
		private void Awake() {
			if (!FB.IsInitialized) {
				FB.Init(() => {
							if (FB.IsInitialized)
								FB.ActivateApp();
							else
								Debug.LogError("Couldn't initialize");
						},
						isGameShown => {
							if (!isGameShown)
								Time.timeScale = 0;
							else
								Time.timeScale = 1;
						});
			}
			else
				FB.ActivateApp();
		}


		#region Login / Logout

		public static void Login() {
			var permissions = new List<string>() { "public_profile" };
			FB.LogInWithReadPermissions(permissions);
		}


		public static void Logout() {
			FB.LogOut();
		}

		#endregion


		public static void Share(FacebookDelegate<IShareResult> onResult = null) {
			if (!FB.IsLoggedIn) Login();

			FB.ShareLink(new System.Uri("https://play.google.com/store/apps/details?id=com.activision.callofduty.shooter"),
						"Check it out!",
						"Good game development company!",
						new System.Uri("https://i.ytimg.com/vi/rstUeJuXQp4/hqdefault.jpg"), onResult);
		}


		#region Inviting

		public static void GameRequest() {
			if (!FB.IsLoggedIn) Login();
			FB.AppRequest("Hey! Come and play this awesome game!", title: "Reso Coder Tutorial");
		}

		#endregion
	}
}