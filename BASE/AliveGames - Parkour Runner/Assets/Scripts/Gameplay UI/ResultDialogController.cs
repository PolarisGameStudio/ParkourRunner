using UnityEngine;
using UnityEngine.UI;
using ParkourRunner.Scripts.Managers;

public class ResultDialogController : MonoBehaviour
{
    public enum ViewModes
    {
        FullMode,
        ShortMode
    }

    [Header("Result Data Settings")]
    [SerializeField] private LocalizationComponent _distanceLocalization;
    [SerializeField] private LocalizationComponent _recordLocalization;
    [SerializeField] private LocalizationComponent _coinsLocalization;
    [SerializeField] private Text _distanceLabel;
    [SerializeField] private Text _recordDistanceLabel;
    [SerializeField] private Text _coinsLabel;    

    [Header("View Mode Settings")]
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private HorizontalLayoutGroup _buttonsGroup;
    [SerializeField] private float _fullModeSpacing;
    [SerializeField] private float _shortModeSpacing;

    public void Show()
    {
        GameManager manager = GameManager.Instance;

        _distanceLabel.text = string.Format("{0}  {1} m", _distanceLocalization.Text, (int)manager.DistanceRun);
        _recordDistanceLabel.text = string.Format("{0}  {1} m", _recordLocalization.Text, (int)ProgressManager.DistanceRecord);
        _coinsLabel.text = string.Format("{0}  {1}", _coinsLocalization.Text, Wallet.Instance.InGameCoins);

        switch (EnvironmentController.CurrentMode)
        {
            case GameModes.Tutorial:
                SetMode(manager.IsLevelComplete ? ViewModes.FullMode : ViewModes.ShortMode);
                break;

            case GameModes.Endless:
                SetMode(ViewModes.ShortMode);
                break;

            case GameModes.Levels:
                SetMode(manager.IsLevelComplete ? ViewModes.FullMode : ViewModes.ShortMode);
                break;
        }

        this.gameObject.SetActive(true);
    }
        
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void SetMode(ViewModes mode)
    {
        switch (mode)
        {
            case ViewModes.FullMode:
                _nextButton.SetActive(true);
                _buttonsGroup.spacing = _fullModeSpacing;
                break;

            case ViewModes.ShortMode:
                _nextButton.SetActive(false);
                _buttonsGroup.spacing = _shortModeSpacing;
                break;
        }
    }
}