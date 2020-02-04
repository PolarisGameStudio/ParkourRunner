using System;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    #region Singleton
    public static AdManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            ResetAdvertisingOrder();

            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    [SerializeField] private AppodealAdController _ad;
    
    private int _gameSessionCount;

    public bool EnableAds => true; // PlayerPrefs.GetInt("NoAds") != 1; } }


    public void Start()
    {
        _ad.Initialize();
    }

    public bool IsAvailable()
    {
        return _ad != null ? _ad.IsAvailable() : false;
    }

    public void ShowAdvertising(Action finishedCallback, Action skippedCallback, Action failedCallback)
    {
        bool isShowingAd = false;
        
        if (_ad.IsAvailable())
        {
            Debug.Log("Show " + _ad.gameObject.name);
            _ad.InitCallbackHandlers(finishedCallback, skippedCallback, failedCallback);

            if (this.EnableAds)
                _ad.ShowAdvertising();
            else
                _ad.HandleAdResult(AdResults.Finished);

            isShowingAd = true;
        }

        if (!isShowingAd)
        {
            Debug.Log("Ad not show");
            finishedCallback.SafeInvoke();
        }
    }

    public void ShowBanner()
    {
        //_ad.ShowBanner();
    }

    public void HideBanner()
    {
        //_ad.HideBanner();
    }

    #region Advertising Order
    public bool CheckAdvertisingOrder()
    {
        if (PlayerPrefs.GetInt(EnvironmentController.TUTORIAL_KEY) == 1 || _gameSessionCount % 3 == 0 || !AdManager.Instance.IsAvailable())
        {
            ResetAdvertisingOrder();
            return false;
        }

        _gameSessionCount++;

        return true;
    }

    public void ResetAdvertisingOrder()
    {
        _gameSessionCount = 1;
    }

    public void SkipAdInOrder()
    {
        _gameSessionCount = 3;
    }
    #endregion
}