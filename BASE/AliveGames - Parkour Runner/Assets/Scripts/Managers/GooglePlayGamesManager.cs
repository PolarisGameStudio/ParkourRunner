using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Managers {
	public class GooglePlayGamesManager : MonoBehaviour {
		public const string LeaderBoardId = "CgkIi6r778MEEAIQAQ";


		private void Start () {
#if UNITY_ANDROID
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
			PlayGamesPlatform.DebugLogEnabled = true;
			PlayGamesPlatform.InitializeInstance(config);
			PlayGamesPlatform.Activate();

			SignIn();
#endif
		}


		private static void SignIn() {
#if UNITY_ANDROID
			// print("GP Games Authenticate");
			PlayGamesPlatform.Instance.Authenticate(delegate (bool b, string s) { }, false);
			Social.localUser.Authenticate(( b, s) => print(s));
#endif
		}


		#region Leaderboards

		public static void SetScoreToLeaderboard(long score) {
			// print($"Trying set leaderboard score: {score}");
			Social.ReportScore(score, LeaderBoardId, delegate (bool status) { print($"Score is set: {status}"); });
		}


		public static void ShowLeaderboardsUI() {
#if UNITY_ANDROID
			if (!PlayGamesPlatform.Instance.IsAuthenticated()) {
				SignIn();
				return;
			}
			Social.ShowLeaderboardUI();
#endif
		}

		#endregion /Leaderboards
	}
}