using System.Collections;
using System;
using UnityEngine;
using DG.Tweening;
using AEngine;
using Managers;

public class MainMenu : Menu
{
    public static bool IsPlayingOpenMenuAnimation;
    public static bool IsPlayingSettingsAnimation;
        
    [Header("Animation settings")]
    [SerializeField] private GameObject _gameLoader;
    [SerializeField] private SettingsTweening _settingsTweening;    
    [SerializeField] private RectTransform _buttonsShowSpringPoint;
    [SerializeField] private RectTransform _questShowSpringPoint;
    [SerializeField] private MovingAnimation _buttonsBlockAnim;
    [SerializeField] private MovingAnimation _settingsPanelAnim;
    [SerializeField] private MovingAnimation _playerStatusAnim;
    [SerializeField] private MovingAnimation _questBlock;
        
    protected override void Show()
    {
        base.Show();

        IsPlayingOpenMenuAnimation = false;
        IsPlayingSettingsAnimation = false;
        
        var settingsSecuance = DOTween.Sequence();
        settingsSecuance.Append(_settingsPanelAnim.Show());
                
        var secuance = DOTween.Sequence();
        secuance.Append(_buttonsBlockAnim.Show(_buttonsShowSpringPoint.anchoredPosition, _buttonsBlockAnim.duration * 0.9f));
        secuance.Insert(0f, _questBlock.Show(_questShowSpringPoint.anchoredPosition, _questBlock.duration * 0.9f));

        secuance.Insert(0, _playerStatusAnim.Show());
                        
        secuance.OnComplete(() =>
        {
            var finalSecuance = DOTween.Sequence();
            finalSecuance.Append(_buttonsBlockAnim.Show(_buttonsBlockAnim.showPoint.anchoredPosition, _buttonsBlockAnim.duration * 0.1f));
            finalSecuance.Append(_questBlock.Show(_questBlock.showPoint.anchoredPosition, _questBlock.duration * 0.1f));

            AdManager.Instance.ShowBottomBanner();
        });
    }
        
    protected override void StartHide(Action callback)
    {
        base.StartHide(callback);
        StartCoroutine(HideProcess(callback));

        AdManager.Instance.HideBottomBanner();
    }

    private IEnumerator HideProcess(Action callback)
    {
        while (_settingsTweening.IsInProcess)
        {
            yield return new WaitForEndOfFrame();
        }

        if (_settingsTweening.IsOpend)
        {
            _settingsTweening.CloseSettings();
        }

        var secuance = DOTween.Sequence();
        
        secuance.Append(_buttonsBlockAnim.Hide());
        secuance.Insert(0f, _questBlock.Hide());

        secuance.Insert(0f, _settingsPanelAnim.Hide());

        secuance.Insert(0f, _playerStatusAnim.Hide());
        
        secuance.OnComplete(() =>
        {
            FinishHide(callback);
        });
    }
    
    #region Events
    public void OnPlayButtonClick()
    {
        if (!IsPlayingOpenMenuAnimation && !IsPlayingSettingsAnimation)
        {
            IsPlayingOpenMenuAnimation = true;

            _audio.PlaySound(Sounds.Tap);
            _menuController.OpenMenu(MenuKinds.SelectLevelType);
        }
    }

    public void OnShopButtonClick()
    {
        if (!IsPlayingOpenMenuAnimation && !IsPlayingSettingsAnimation)
        {
            IsPlayingOpenMenuAnimation = true;

            _audio.PlaySound(Sounds.Tap);
            _menuController.OpenMenu(MenuKinds.Shop);
        }
    }

    public void OnQuestButtonClick()
    {
        if (!IsPlayingOpenMenuAnimation && !IsPlayingSettingsAnimation)
        {
            IsPlayingOpenMenuAnimation = true;

            _audio.PlaySound(Sounds.Tap);
            _menuController.OpenMenu(MenuKinds.Quests);
        }
    }

    public void OnLeaderboardButtonClick()
    {
        if (!IsPlayingOpenMenuAnimation && !IsPlayingSettingsAnimation)
        {
            _audio.PlaySound(Sounds.Tap);

#if UNITY_IPHONE || UNITY_IOS
            AppleGameCenterManager.ShowLeaderboardsUI();
#elif UNITY_ANDROID
            GooglePlayGamesManager.ShowLeaderboardsUI();
#endif
        }
    }

    public void OnMultiplayerButtonClick()
    {
        _audio.PlaySound(Sounds.Tap);
        _menuController.OpenMenu(MenuKinds.Multiplayer);
    }
    #endregion
}