using System;
using UnityEngine;
using UnityEngine.Advertisements;

public enum AdResults
{
    Finished,
    Skipped,
    Failed
}

public abstract class BaseAdController : MonoBehaviour
{
    public static event Action OnShowAdsEvent;

    protected Action _finishedCallback;
    protected Action _skippedCallback;
    protected Action _failedCallback;

    public abstract void Initialize();

    public abstract bool IsAvailable();

    public abstract void ShowAdvertising();

    public void InitCallbackHandlers(Action finishedCallback, Action skippedCallback, Action failedCallback)
    {
        _finishedCallback = finishedCallback;
        _skippedCallback = skippedCallback;
        _failedCallback = failedCallback;
    }

    //public void HandleAdResult(ShowResult result)
    public void HandleAdResult(AdResults result)
    {
        switch (result)
        {
            //case ShowResult.Finished:
            case AdResults.Finished:
                _finishedCallback.SafeInvoke();
                OnShowAdsEvent.SafeInvoke();
                break;

            case AdResults.Skipped:
            //case ShowResult.Skipped:
                _skippedCallback.SafeInvoke();
                break;

            //case ShowResult.Failed:
            case AdResults.Failed:
                _finishedCallback.SafeInvoke();
                break;
        }
    }
}