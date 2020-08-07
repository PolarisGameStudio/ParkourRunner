using UnityEngine;
using UnityEngine.SceneManagement;
using ParkourRunner.Scripts.Managers;
using AEngine;

public class NextGameTransition : MonoBehaviour
{
    [SerializeField] private MenuTransition _baseTransition;
    [SerializeField] private int _gameSceneID;

    private void LoadScene(bool reloadScene)
    {
        if (reloadScene)
        {
            AudioManager.Instance.PlaySound(Sounds.Tap);
            SceneManager.LoadScene(_gameSceneID);
        }
        else {
            _baseTransition.OnTransitionButtonClick();
        }

        AdManager.Instance.HideBottomBanner();
    }

    #region Events
    public void OnTransitionButtonClick()
    {
        switch (EnvironmentController.CurrentMode)
        {
            case GameModes.Tutorial:
                LoadScene(!GameManager.Instance.IsLevelComplete);
                break;

            case GameModes.Endless:
                LoadScene(true);
                break;

            case GameModes.Levels:
                if (GameManager.Instance.IsLevelComplete)
                {
                    int maxLevel = PlayerPrefs.GetInt(EnvironmentController.MAX_LEVEL);
                    int level = PlayerPrefs.GetInt(EnvironmentController.LEVEL_KEY);

                    level = Mathf.Clamp(level + 1, 1, maxLevel);
                    PlayerPrefs.SetInt(EnvironmentController.LEVEL_KEY, level);
                    PlayerPrefs.Save();
                }

                LoadScene(true);
                break;
        }
    }
    #endregion
}
