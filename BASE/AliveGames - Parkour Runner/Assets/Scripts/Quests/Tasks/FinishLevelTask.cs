using System.Collections;
using UnityEngine;
using ParkourRunner.Scripts.Managers;

public class FinishLevelTask : QuestTask
{
    [SerializeField] private float _delay;

    private void OnEnable()
    {
        if (IsEnable)
        {
            FinishMessage.OnFinishLevelMessage -= OnFinishLevel;
            FinishMessage.OnFinishLevelMessage += OnFinishLevel;
        }            
    }

    private void OnDisable()
    {
        FinishMessage.OnFinishLevelMessage -= OnFinishLevel;
    }

    private IEnumerator CompleteQuestProcess()
    {
        yield return new WaitForSeconds(_delay);

        HUDManager.Instance.ShowGreatMessage(HUDManager.Messages.QuestComplete);
        CompleteQuest(true);

        FinishMessage.OnFinishLevelMessage -= OnFinishLevel;
    }

    #region Events
    private void OnFinishLevel()
    {
        StartCoroutine(CompleteQuestProcess());
    }
    #endregion
}