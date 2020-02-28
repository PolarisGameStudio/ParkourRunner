using System.Collections;
using UnityEngine;
using AEngine;

public class AppManager : MonoSingleton<AppManager>
{
    private const string VERSION_KEY = "Application Version";
    private const int DEFAULT_VERSION_VALUE = 1;

    [SerializeField] private int _appVersion;
    [SerializeField] private CharactersData _characters;

    public int AppVersion { get { return _appVersion; } }
    
    protected override void Init()
    {
        base.Init();

        StartCoroutine(CheckApplicationVersionProcess());

        DontDestroyOnLoad(this.gameObject);
    }

    private IEnumerator CheckApplicationVersionProcess()
    {
        yield return new WaitForEndOfFrame();

        int version = PlayerPrefs.GetInt(VERSION_KEY, DEFAULT_VERSION_VALUE);

        if (version < DEFAULT_VERSION_VALUE)
        {
            version = DEFAULT_VERSION_VALUE;
            PlayerPrefs.Save();
        }

        if (_appVersion > version)
        {
            while (version < _appVersion)
            {
                NextStepUpdate(version);
                version++;

                yield return null;
            }
        }
    }

    private void NextStepUpdate(int currentSavedVersion)
    {
        switch (currentSavedVersion)
        {
            case 1:
                UpdateToVersion2();
                break;
        }
    }

    private void UpdateToVersion2()
    {
        Debug.Log("Update game settings to version 2");
        
        CharactersData.Data oldBaseChar = _characters.GetCharacterData(CharacterKinds.Character1);
        oldBaseChar.Bought = false;

        PlayerPrefs.SetInt(VERSION_KEY, 2);
        PlayerPrefs.Save();
    }
}