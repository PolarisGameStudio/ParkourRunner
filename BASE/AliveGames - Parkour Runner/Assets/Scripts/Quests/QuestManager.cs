using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private const string ACTIVE_QUESTS_KEY = "QUEST";
    private const string DATE_YEAR_KEY = "DATE_YEAR";
    private const string DATE_MONTH_KEY = "DATE_MONTH";
    private const string DATE_DAY_KEY = "DATE_DAY";
    private const string DATE_HOUR_KEY = "DATE_HOUR";
    private const string DATE_MINUTE_KEY = "DATE_MINUTE";
    private const string DATE_SECOND_KEY = "DATE_SECOND";

    public static QuestManager Instance;

    public event Action OnUpdateActiveQuestsEvent;

    [SerializeField] private int _questsHoursPeriod;
    [SerializeField] private int _activeQuestsCount;
    [SerializeField] private List<QuestData> _quests;

    public List<QuestData> ActiveQuests { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            this.ActiveQuests = new List<QuestData>();

            StartCoroutine(UpdateStateProcess());

            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);
    }

    public void CompleteQuest(int id)
    {
        for (int i = 0; i < this.ActiveQuests.Count; i++)
        {
            if (this.ActiveQuests[i].ID == id)
            {
                this.ActiveQuests.RemoveAt(i);
                SaveActiveQuests();
                return;
            }
        }
    }

    private IEnumerator UpdateStateProcess()
    {
        while (true)
        {
            DateTime date = DateTime.Now;

            // Check on first launch
            if (!PlayerPrefs.HasKey(DATE_YEAR_KEY))
            {
                SelectRandomQuests();
                SaveActiveQuests();
                SaveDate(date);

                OnUpdateActiveQuestsEvent.SafeInvoke();
            }
            else
            {
                DateTime savedDate = new DateTime(PlayerPrefs.GetInt(DATE_YEAR_KEY), PlayerPrefs.GetInt(DATE_MONTH_KEY), PlayerPrefs.GetInt(DATE_DAY_KEY), PlayerPrefs.GetInt(DATE_HOUR_KEY), PlayerPrefs.GetInt(DATE_MINUTE_KEY), PlayerPrefs.GetInt(DATE_SECOND_KEY));

                double deltaTime = (date - savedDate).TotalHours;
                
                if (deltaTime >= _questsHoursPeriod)
                {
                    SelectRandomQuests();
                    SaveActiveQuests();
                    SaveDate(date);

                    OnUpdateActiveQuestsEvent.SafeInvoke();
                }
                else if (this.ActiveQuests.Count == 0)
                {
                    LoadActiveQuests();

                    OnUpdateActiveQuestsEvent.SafeInvoke();
                }
            }

            yield return new WaitForSeconds(15f);
        }
    }

    private void SaveDate(DateTime date)
    {
        PlayerPrefs.SetInt(DATE_YEAR_KEY, date.Year);
        PlayerPrefs.SetInt(DATE_MONTH_KEY, date.Month);
        PlayerPrefs.SetInt(DATE_DAY_KEY, date.Day);
        PlayerPrefs.SetInt(DATE_HOUR_KEY, date.Hour);
        PlayerPrefs.SetInt(DATE_MINUTE_KEY, date.Minute);
        PlayerPrefs.SetInt(DATE_SECOND_KEY, date.Second);

        PlayerPrefs.Save();
    }

    private void SelectRandomQuests()
    {
        this.ActiveQuests.Clear();

        //List<QuestData> list = new List<QuestData>(_quests);
        List<QuestData> list = _quests.Where(x => x.Enable).ToList();

        for (int i = 0; i < _activeQuestsCount; i++)
        {
            QuestData item = list[UnityEngine.Random.Range(0, list.Count)];
            ActiveQuests.Add(item);

            list = list.Where(x => x.Enable && x.ID != item.ID).ToList();
        }
    }

    private void SaveActiveQuests()
    {
        if (this.ActiveQuests == null)
        {
            Debug.LogError("Quests list was not initialized correctly");
        }
        else
        {
            for (int i = 0; i < _quests.Count; i++)
            {
                string key = ACTIVE_QUESTS_KEY + _quests[i].ID;
                PlayerPrefs.SetInt(key, this.ActiveQuests.Contains(_quests[i]) ? 1 : 0);
            }

            PlayerPrefs.Save();
        }
    }

    private void LoadActiveQuests()
    {
        this.ActiveQuests.Clear();

        for (int i = 0; i < _quests.Count; i++)
        {
            string key = ACTIVE_QUESTS_KEY + _quests[i].ID;

            if (!PlayerPrefs.HasKey(key))
            {
                Debug.LogError("Couldn't find quest key. Select random items.");
                SelectRandomQuests();
                SaveActiveQuests();
                SaveDate(DateTime.Now);
                break;
            }
            else
            {
                int isActive = PlayerPrefs.GetInt(key);
                if (isActive == 1)
                    this.ActiveQuests.Add(_quests[i]);
            }
        }
    }
}