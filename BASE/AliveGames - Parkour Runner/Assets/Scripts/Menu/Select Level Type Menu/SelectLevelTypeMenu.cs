using System;
using UnityEngine;
using DG.Tweening;
using AEngine;
using Managers;

public class SelectLevelTypeMenu : Menu
{
    [SerializeField] private GameObject _gameLoader;

    [Header("Animation settings")]
    [SerializeField] private MovingAnimation _backButtonAnim;
    [SerializeField] private AlphaAnimation _levelsAnim;

    protected override void Show()
    {
        base.Show();
                
        var sequence = DOTween.Sequence();
        sequence.Append(_levelsAnim.Show());
        sequence.Insert(0.1f, _backButtonAnim.Show());
        sequence.OnComplete(() =>
        {
            AdManager.Instance.ShowBottomBanner();
        });
    }

    protected override void StartHide(Action callback)
    {
        base.StartHide(callback);

        var sequence = DOTween.Sequence();
        sequence.Append(_backButtonAnim.Hide());
        sequence.Insert(0.2f, _levelsAnim.Hide());

        sequence.OnComplete(() =>
        {
            FinishHide(callback);
        });
        AdManager.Instance.HideBottomBanner();
    }

    private void OpenGame()
    {
        MenuController.TransitionTarget = MenuKinds.None;
        _gameLoader.SetActive(true);
    }

    #region Events
    public void OnBackButtonClick()
    {
        _audio.PlaySound(Sounds.Tap);
        _menuController.OpenMenu(MenuKinds.MainMenu);
    }



    public void OnShopButtonClick()
    {
        _audio.PlaySound(Sounds.Tap);
        _menuController.OpenMenu(MenuKinds.Shop);
    }

    public void OnTutorialLevelClick()
    {
        _audio.PlaySound(Sounds.Tap);

        EnvironmentController.CurrentMode = GameModes.Tutorial;

        StartHide(OpenGame);
        AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.start_tutorial);
    }

    public void OnEnglessLevelClick()
    {
        _audio.PlaySound(Sounds.Tap);

        EnvironmentController.CheckKeys();
        PlayerPrefs.SetInt(EnvironmentController.MULTIPLAYER_KEY, 0);
        PlayerPrefs.SetInt(EnvironmentController.TUTORIAL_KEY, 0);
        PlayerPrefs.SetInt(EnvironmentController.ENDLESS_KEY, 1);
        PlayerPrefs.Save();

        StartHide(OpenGame);
        AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.start_endless);
    }

    public void OnSelectLevelClick()
    {
        _audio.PlaySound(Sounds.Tap);
        _menuController.OpenMenu(MenuKinds.SelectLevel);
    }

    public void OnMultiplayerClick()
    {
        _audio.PlaySound(Sounds.Tap);
        void Action() => _menuController.OpenMenu(MenuKinds.Multiplayer);
        Action();
        return;

        if (AdManager.Instance.InterstitialIsAvailable()) {
            AdManager.Instance.ShowInterstitial(Action, Action, Action);
        }
        else {
            Action();
        }
    }

    public void OnSelectGameModesClick()
    {
        _audio.PlaySound(Sounds.Tap);
        _menuController.OpenMenu(MenuKinds.SelectLevelType2);
    }
    #endregion
}