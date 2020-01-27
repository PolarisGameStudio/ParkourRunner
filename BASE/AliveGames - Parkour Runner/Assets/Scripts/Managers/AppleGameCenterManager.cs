using UnityEngine;

namespace Managers
{
    public class AppleGameCenterManager : MonoBehaviour
    {
#if UNITY_IPHONE || UNITY_IOS
        public static AppleGameCenterManager Instance;
        public const string LeaderBoardId = "com.ParkourRunner.Leaderboards.AppleGameCenter.v1";
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            SignIn();
        }
        
		private static void SignIn()
        {
			print("Apple Games Center Authenticate");
            Social.localUser.Authenticate(( b, s) => print(s));
		}
        

        #region Leaderboards
        public static void SetScoreToLeaderboard(long score)
        {
			print($"Trying set leaderboard score: {score}");
			Social.ReportScore(score, LeaderBoardId, delegate (bool status) { print($"Score is set: {status}"); });
		}
        
		public static void ShowLeaderboardsUI()
        {
			Social.ShowLeaderboardUI();
		}
        #endregion Leaderboards
#endif
    }
}