using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

namespace Managers {
	public class Facebook : MonoBehaviour {
#if !UNITY_EDITOR && !UNITY_STANDALONE_WIN
		private void Awake() {
			print("Check FB Init");
			if (!FB.IsInitialized) {
				print("Start FB init...");
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
#endif


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
#if !UNITY_EDITOR && !UNITY_STANDALONE_WIN

			if (!FB.IsLoggedIn) Login();

			FB.ShareLink(new System.Uri("https://play.google.com/store/apps/details?id=com.play.game.cyber.parkour.endless.runner.robot.bot.subway.tomb.runbot"),
						"Check it out!",
						"Beat my record!",
						null, onResult);
#endif
		}


		#region Inviting

		public static void GameRequest() {
			if (!FB.IsLoggedIn) Login();
			FB.AppRequest("Hey! Come and play this awesome game!", title: "Reso Coder Tutorial");
		}

		#endregion
	}
}