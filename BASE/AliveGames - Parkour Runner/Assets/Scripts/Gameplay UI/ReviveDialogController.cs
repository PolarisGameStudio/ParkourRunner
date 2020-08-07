using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using ParkourRunner.Scripts.Managers;
using AEngine;

public class ReviveDialogController : MonoBehaviour
{
    public enum Results
    {
        ShowAdvertising,
        ReviveByCoins,
        OpenShopMenu,
        TimeIsOut
    }
    

    [Header("Player Wait Progress")]
    [SerializeField] private Image _leftTimeProgress;
    [SerializeField] private float _activeStateDuration;

    [Header("UI Items")]
    [SerializeField] private Text _revivePriceLabel;
    [SerializeField] private GameObject _reviveCostButton;
    [SerializeField] private GameObject _reviveAdButtion;

    public bool IsPriceCondition => Wallet.Instance.AllCoins >= GameManager.Instance.ReviveCost;
    public bool IsAdCondition => AdManager.Instance.RewardedVideoIsAvailable();
    public bool CanShow => this.IsPriceCondition || this.IsAdCondition;

    private AudioManager _audio;
    private Action<Results> _callbackHandler;
    

    public void Show(Action<Results> callback)
    {
        if (_audio == null)
            _audio = AudioManager.Instance;

        if (this.CanShow)
        {
            _audio.PlaySound(Sounds.GameOver);

            _revivePriceLabel.text = (-GameManager.Instance.ReviveCost).ToString();
            _reviveCostButton.SetActive(this.IsPriceCondition);
            _reviveAdButtion.SetActive(this.IsAdCondition);

            this.gameObject.SetActive(true);
            
            StartCoroutine(WaitPlayerProcess());
            _callbackHandler = callback;
        }
        else
            callback.SafeInvoke(Results.TimeIsOut);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private IEnumerator WaitPlayerProcess()
    {
        float time = _activeStateDuration;

        while (time > 0f)
        {
            _leftTimeProgress.fillAmount = Mathf.Lerp(1f, 0f, Mathf.Clamp01(1f - time / _activeStateDuration));

            yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
        }

        _leftTimeProgress.fillAmount = 0f;

        _callbackHandler.SafeInvoke(Results.TimeIsOut);
        this.gameObject.SetActive(false);
    }


    #region Events
    public void OnShowAdButtonClick()
    {
        _audio.PlaySound(Sounds.Tap);
        _callbackHandler.SafeInvoke(Results.ShowAdvertising);

        this.gameObject.SetActive(false);
    }

    public void OnReviveByCoinsButtonClick()
    {
        _audio.PlaySound(Sounds.Tap);
        _callbackHandler.SafeInvoke(Results.ReviveByCoins);

        this.gameObject.SetActive(false);
    }

    public void OnOpenShopMenuButtonClick()
    {
        _audio.PlaySound(Sounds.Tap);
        _callbackHandler.SafeInvoke(Results.OpenShopMenu);

        this.gameObject.SetActive(false);
    }
    #endregion
}