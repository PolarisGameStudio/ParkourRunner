using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Managers {
	public class GooglePlayGamesManager : MonoBehaviour {
		public const string LeaderBoardId = "CgkIi6r778MEEAIQAQ";
		private void Start () {
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
			PlayGamesPlatform.DebugLogEnabled = true;
			PlayGamesPlatform.InitializeInstance(config);
			PlayGamesPlatform.Activate();

			SignIn();
		}


		private static void SignIn() {
			print("GP Games Authenticate");
			PlayGamesPlatform.Instance.Authenticate(delegate (bool b, string s) {
				print(b);
				print(s);
			}, false);
			Social.localUser.Authenticate(( b, s) => print(s));
		}


		/*#region Achievements

		public static void UnlockAchievement(string id) {
			Social.ReportProgress(id, 100, success => { });
		}


		public static void IncrementAchievement(string id, int stepsToIncrement) {
			PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
		}


		public static void ShowAchievementsUI() {
			Social.ShowAchievementsUI();
		}

		#endregion /Achievements*/


		#region Leaderboards

		public static void SetScoreToLeaderboard(long score) {
			print($"Trying set leaderboard score: {score}");
			Social.ReportScore(score, LeaderBoardId, delegate (bool status) { print($"Score is set: {status}"); });
		}


		public static void ShowLeaderboardsUI() {
			if (!PlayGamesPlatform.Instance.IsAuthenticated()) {
				SignIn();
				return;
			}
			Social.ShowLeaderboardUI();
		}

		#endregion /Leaderboards
	}
}