using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class DailyQuestController : MonoBehaviour
{
    [SerializeField] private List<QuestItem> _quests;
    [SerializeField] private int _activeQuestsCount;

    private List<QuestItem> _activeQuests;

    private void OnEnable()
    {
        SelectRandomQuests();
        ShowActiveQuests();
    }

    private void CheckTime()
    {
        var date = DateTime.Now;
        
        
    }

    private void SelectRandomQuests()
    {
        if (_activeQuests != null)
            _activeQuests.Clear();
        else
            _activeQuests = new List<QuestItem>();

        List<QuestItem> list = new List<QuestItem>(_quests);

        for (int i = 0; i < _activeQuestsCount; i++)
        {
            QuestItem item = list[UnityEngine.Random.Range(0, list.Count)];
            _activeQuests.Add(item);

            list = list.Where(x => x.IsEnable && x.ID != item.ID).ToList();
        }
    }

    private void ShowActiveQuests()
    {
        foreach (var item in _quests)
            item.gameObject.SetActive(false);

        foreach (var item in _activeQuests)
            item.gameObject.SetActive(true);
    }
}