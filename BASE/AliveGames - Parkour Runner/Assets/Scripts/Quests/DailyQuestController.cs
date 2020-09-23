using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DailyQuestController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private float _animationDuration;
    [SerializeField] private List<QuestItem> _quests;

    private QuestManager _manager => QuestManager.Instance;
    private List<QuestItem> _activeQuests;

    private void OnEnable()
    {
        _group.alpha = 1f;
        ShowActiveQuests();

        _manager.OnUpdateActiveQuestsEvent -= OnUpdateQuests;
        _manager.OnUpdateActiveQuestsEvent += OnUpdateQuests;        
    }

    private void OnDisable()
    {
        _manager.OnUpdateActiveQuestsEvent -= OnUpdateQuests;
    }

    private void ShowActiveQuests()
    {
        foreach (var item in _quests)
            item.gameObject.SetActive(false);


        foreach (var item in _manager.ActiveQuests)
        {
            var target = FindQuest(item.ID);

            if (target != null) {
                target.gameObject.SetActive(true);
                var canvasGroup = target.GetComponent<CanvasGroup>();
                print(canvasGroup);
                canvasGroup.alpha = _manager.CompletedQuests.Contains(item.ID) ? 0.3f : 1f;
            }
        }
    }

    private IEnumerator ReplaceActiveQuestsProcess()
    {
        float duration = _animationDuration / 2f;
        float time = duration;

        while (time >= 0f)
        {
            _group.alpha = Mathf.Clamp01(time / duration);
            time -= Time.deltaTime;

            yield return null;
        }

        _group.alpha = 0f;
        time = 0f;
        ShowActiveQuests();

        yield return new WaitForSeconds(0.1f);

        while (time <= duration)
        {
            _group.alpha = Mathf.Clamp01(time / duration);
            time += Time.deltaTime;

            yield return null;
        }

        _group.alpha = 1f;
    }

    private QuestItem FindQuest(int id)
    {
        for (int i = 0; i < _quests.Count; i++)
            if (_quests[i].Data.ID == id)
                return _quests[i];

        return null;
    }

    #region Events
    private void OnUpdateQuests()
    {
        StartCoroutine(ReplaceActiveQuestsProcess());
    }
    #endregion
}