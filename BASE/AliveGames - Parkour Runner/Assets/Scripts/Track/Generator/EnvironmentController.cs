using UnityEngine;
using ParkourRunnerEnvironment;

public enum GameModes
{
    Tutorial,
    Endless,
    Levels,
    Multiplayer
}

[CreateAssetMenu(fileName = "Environment Controller", menuName = "ParkouRunner/Environment Controller")]
public class EnvironmentController : ScriptableObject
{
    public const string TUTORIAL_KEY = "Tutorial";
    public const string ENDLESS_KEY = "Endless";
    public const string LEVEL_KEY = "Level";
    public const string MULTIPLAYER_KEY = "Multiplayer";
    public const string MAX_LEVEL = "Max Level";

    [SerializeField] private Environment _tutorial;
    [SerializeField] private Environment[] _levels;
    [SerializeField] private Environment[] _multiplayerLevels;

    [Header("Debug mode")]
    [SerializeField] private bool _debug;
    [SerializeField] private Environment _target;

    public static GameModes CurrentMode
    {
        get
        {
            CheckKeys();
            return (PlayerPrefs.GetInt(TUTORIAL_KEY) == 1) ? (GameModes.Tutorial) : (PlayerPrefs.GetInt(ENDLESS_KEY) == 1 ? GameModes.Endless : (PlayerPrefs.GetInt(MULTIPLAYER_KEY)) == 1 ? GameModes.Endless : GameModes.Levels);
        }

        set {
            CheckKeys();
            switch (value) {
                case GameModes.Tutorial:
                    PlayerPrefs.SetInt(TUTORIAL_KEY,    1);
                    PlayerPrefs.SetInt(ENDLESS_KEY,     0);
                    PlayerPrefs.SetInt(MULTIPLAYER_KEY, 0);
                    break;

                case GameModes.Endless:
                    PlayerPrefs.SetInt(TUTORIAL_KEY,    0);
                    PlayerPrefs.SetInt(ENDLESS_KEY,     1);
                    PlayerPrefs.SetInt(MULTIPLAYER_KEY, 0);
                    break;

                case GameModes.Levels:
                    PlayerPrefs.SetInt(TUTORIAL_KEY,    0);
                    PlayerPrefs.SetInt(ENDLESS_KEY,     0);
                    PlayerPrefs.SetInt(MULTIPLAYER_KEY, 0);
                    break;

                case GameModes.Multiplayer:
                    PlayerPrefs.SetInt(TUTORIAL_KEY,    0);
                    PlayerPrefs.SetInt(ENDLESS_KEY,     0);
                    PlayerPrefs.SetInt(MULTIPLAYER_KEY, 1);
                    break;
            }

            PlayerPrefs.Save();
        }
    }

    public static void CheckKeys()
    {
        if (!PlayerPrefs.HasKey(TUTORIAL_KEY) || !PlayerPrefs.HasKey(LEVEL_KEY) || !PlayerPrefs.HasKey(ENDLESS_KEY) || !PlayerPrefs.HasKey(MAX_LEVEL) || !PlayerPrefs.HasKey(MULTIPLAYER_KEY))
        {
            PlayerPrefs.SetInt(TUTORIAL_KEY, 1);
            PlayerPrefs.SetInt(ENDLESS_KEY, 0);
            PlayerPrefs.SetInt(LEVEL_KEY, 1);
            PlayerPrefs.SetInt(MAX_LEVEL, 1);
            PlayerPrefs.SetInt(MULTIPLAYER_KEY, 0);
            PlayerPrefs.Save();
        }
    }

    public Environment GetActualEnvironment()
    {
        CheckKeys();

        if (_debug && _target != null)
            return _target;

        if (PlayerPrefs.GetInt(TUTORIAL_KEY) == 1) {
            return _tutorial;
        }
        if (PlayerPrefs.GetInt(ENDLESS_KEY) == 1) {
            return GetEndless();
        }
        if (PlayerPrefs.GetInt(MULTIPLAYER_KEY) > 0) {
            return GetMultiplayerLevel(PlayerPrefs.GetInt(MULTIPLAYER_KEY) - 1);
        }

        return GetLevelByIndex(PlayerPrefs.GetInt(LEVEL_KEY));
    }

    private Environment GetEndless()
    {
        foreach (Environment item in _levels)
            if (item.EndlessLevel)
                return item;

        Debug.LogError("Couldn't find endless level");

        return null;
    }

    private Environment GetLevelByIndex(int index)
    {
        foreach (Environment item in _levels)
            if (item.LevelIndex == index)
                return item;

        Debug.LogError("Couldn't find level by index " + index);

        return null;
    }

    private Environment GetMultiplayerLevel(int index)
    {
        return _multiplayerLevels[index];
    }
}