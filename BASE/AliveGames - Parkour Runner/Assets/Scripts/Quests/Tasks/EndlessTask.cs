using System.Collections;
using UnityEngine;
using ParkourRunner.Scripts.Managers;

public class EndlessTask : QuestTask
{
    [SerializeField] private float _duration;

    private void OnEnable()
    {
        if (IsEnable)
        {
            EnvironmentController.CheckKeys();
            if (PlayerPrefs.GetInt(EnvironmentController.TUTORIAL_KEY) == 0 && PlayerPrefs.GetInt(EnvironmentController.ENDLESS_KEY) == 1)
            {
                StartCoroutine(TaskProcess());
            }
        }
    }

    private IEnumerator TaskProcess()
    {
        yield return new WaitForSeconds(_duration);

        HUDManager.Instance.ShowGreatMessage(HUDManager.Messages.QuestComplete);
        CompleteQuest();
    }
}