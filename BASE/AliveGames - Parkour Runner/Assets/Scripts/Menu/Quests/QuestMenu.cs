using System;
using UnityEngine;
using DG.Tweening;
using AEngine;

public class QuestMenu : Menu
{
    [Header("Animation settings")]
    [SerializeField] private MovingAnimation _backButtonAnim;
    [SerializeField] private MovingAnimation _captionAnim;
    [SerializeField] private AlphaAnimation _levelsAnim;

    protected override void Show()
    {
        base.Show();

        var secuance = DOTween.Sequence();
        secuance.Append(_levelsAnim.Show());
        secuance.Insert(0.1f, _backButtonAnim.Show());
        secuance.Insert(0.1f, _captionAnim.Show());
    }

    protected override void StartHide(Action callback)
    {
        base.StartHide(callback);

        var secuance = DOTween.Sequence();
        secuance.Append(_backButtonAnim.Hide());
        secuance.Insert(0f, _captionAnim.Hide());
        secuance.Insert(0.2f, _levelsAnim.Hide());

        secuance.OnComplete(() =>
        {
            FinishHide(callback);
        });
    }
        
    #region Events
    public void OnBackButtonClick()
    {
        _audio.PlaySound(Sounds.Tap);
        _menuController.OpenMenu(MenuKinds.MainMenu);
    }
    #endregion
}