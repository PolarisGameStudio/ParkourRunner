using Managers.Advertising;

public class WatchAdsTask : QuestTask
{
    public void WatchAdsClick() {
        AdManager.Instance.ShowRewardedVideo(null, null, null);
    }


    private void OnEnable()
    {
        if (IsEnable)
        {
            BaseAdController.OnShowAdsEvent -= OnWatchAds;
            BaseAdController.OnShowAdsEvent += OnWatchAds;
        }
    }

    private void OnDisable()
    {
        BaseAdController.OnShowAdsEvent -= OnWatchAds;
    }
        
    #region Events
    private void OnWatchAds()
    {
        CompleteQuest(false);
        BaseAdController.OnShowAdsEvent -= OnWatchAds;
    }
    #endregion
}