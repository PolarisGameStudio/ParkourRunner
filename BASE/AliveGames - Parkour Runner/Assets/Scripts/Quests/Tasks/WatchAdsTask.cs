using Managers.Advertising;

public class WatchAdsTask : QuestTask
{
    public void WatchAdsClick() {
        print("Watch ad");
        AdManager.Instance.ShowRewardedVideo(OnWatchAds, null, null);
    }
        
    #region Events
    private void OnWatchAds()
    {
        print("OnWatch");
        CompleteQuest(false);
    }
    #endregion
}