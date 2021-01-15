using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum CharacterKinds
{
    Base,
    Character1,
    Character1Fem,
    Character2,
    Character2Fem,
    Character3,
    Character3Fem,
    Character4,
    Character4Fem,
    CaptainAmerica,
    CaptainAmericaFem
}

[CreateAssetMenu(fileName = "Character Configuration", menuName = "Character Data", order = 50)]
public class CharactersData : ScriptableObject
{
    public const string CHARACTER_KEY = "Character";

    [SerializeField] private Data[] _characters;

    public static CharacterKinds CurrentCharacter
    {
        get
        {
            if (!PlayerPrefs.HasKey(CHARACTER_KEY))
            {
                PlayerPrefs.SetString(CHARACTER_KEY, CharacterKinds.Base.ToString());
                PlayerPrefs.Save();
            }

            return (CharacterKinds)Enum.Parse(typeof(CharacterKinds), PlayerPrefs.GetString(CHARACTER_KEY));
        }
    }


    #region Load From Resources

    private static CharactersData Instance {
        get {
            if (_instance != null) return _instance;

            _instance = Resources.Load("Character/Character Configuration") as CharactersData;
            if (_instance == null) {
                throw new FileNotFoundException("Can't found 'Jetpacks' file");
            }
            return _instance;
        }
    }

    private static CharactersData _instance;

    #endregion


    public static Data GetCharacterData(CharacterKinds kind)
    {
        if (Instance._characters == null || Instance._characters.Length <= 0) return null;

        foreach (var item in Instance._characters)
        {
            if (item.kind == kind) {
                return item;
            }
        }

        Debug.LogError("Couldn't find or not configured character = " + kind);
        return null;

    }


    public static List<CharacterKinds> GetFreeSkinList(List<CharacterKinds> usedSkins = null) {
        var skins = new List<CharacterKinds>();
        if (usedSkins == null) {
            usedSkins = Enum.GetValues(typeof(CharacterKinds)).OfType<object>().Select(c => (CharacterKinds) c).ToList();
        }

        foreach (var skin in usedSkins) {
            if (skin == CharacterKinds.Base) continue;

            var key = CHARACTER_KEY + " : " + skin;
            if (!PlayerPrefs.HasKey(key) || PlayerPrefs.GetInt(key) == 0) {
                skins.Add(skin);
            }
        }

        return skins;
    }


    [Serializable]
    public class Data
    {
        public CharacterKinds kind;
        public GameObject     targetPrefab;
        public int            price;

        public string Key => CHARACTER_KEY + " : " + kind;

        public bool Bought
        {
            get
            {
                if (PlayerPrefs.HasKey(Key))
                {
                    return PlayerPrefs.GetInt(Key) != 0 || price <= 0;
                }
                else
                {
                    bool bought = price <= 0;
                    PlayerPrefs.SetInt(Key, bought ? 1 : 0);
                    PlayerPrefs.Save();
                    return bought;
                }
            }

            set
            {
                PlayerPrefs.SetInt(Key, value || price <= 0 ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
    }
}
