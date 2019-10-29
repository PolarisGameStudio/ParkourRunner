using System.Collections;
using UnityEngine;
using ParkourRunner.Scripts.Managers;

public class LevelByTimeTask : QuestTask
{
    [SerializeField] private float _delay;
    [SerializeField] private float _duration;

    private bool _isActive;
    private float _time;

    private void OnEnable()
    {
        if (IsEnable)
        {
            EnvironmentController.CheckKeys();
            if (PlayerPrefs.GetInt(EnvironmentController.TUTORIAL_KEY) == 0 && PlayerPrefs.GetInt(EnvironmentController.ENDLESS_KEY) == 0)
            {
                FinishMessage.OnFinishLevelMessage -= OnFinishLevel;
                FinishMessage.OnFinishLevelMessage += OnFinishLevel;

                _time = _duration;
                _isActive = true;
            }
        }
    }

    private void OnDisable()
    {
        FinishMessage.OnFinishLevelMessage -= OnFinishLevel;
    }

    private void Update()
    {
        if (_isActive)
        {
            _time -= Time.deltaTime;

            if (_time <= 0f)
            {
                FinishMessage.OnFinishLevelMessage -= OnFinishLevel;
                _isActive = false;
            }
        }
    }
        
    private IEnumerator CompleteQuestProcess()
    {
        yield return new WaitForSeconds(_delay);

        HUDManager.Instance.ShowGreatMessage(HUDManager.Messages.QuestComplete);
        CompleteQuest(true);

        FinishMessage.OnFinishLevelMessage -= OnFinishLevel;
        _isActive = false;
    }

    #region Events
    private void OnFinishLevel()
    {
        if (_isActive)
            StartCoroutine(CompleteQuestProcess());
    }
    #endregion
}