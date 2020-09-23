using System.Collections;
using UnityEngine;
using AEngine;

public class EveryDayTask : QuestTask
{
    [SerializeField] private float _delay;
    [SerializeField] private Transform _targetQuest;
    [SerializeField] private CanvasGroup _targetGroup;
    [SerializeField] private float _maxScale;
    [SerializeField] private float _minScale;
    [SerializeField] private float _upDuration;
    [SerializeField] private float _downDuration;

    [Header("Reward animation")]
    [SerializeField] private Transform _rewardPositionTarget;
    [SerializeField] private Transform _reward;
    [SerializeField] private CanvasGroup _rewardGroup;
    [SerializeField] private float _rewardDuration;
    [SerializeField] private float _rewardSpeed;

    private void OnEnable()
    {
        if (IsEnable && !QuestManager.Instance.CompletedQuests.Contains(_data.ID))
        {
            StartCoroutine(CompleteProcess());
        }
    }
        
    private void OnDisable()
    {
        _targetGroup.alpha = 1f;
        _targetQuest.localScale = Vector3.one;
        _reward.localPosition = Vector3.zero;
    }

    private IEnumerator CompleteProcess()
    {
        yield return new WaitForSeconds(_delay);

        float duration = _upDuration;
        float time = 0f;

        while (time <= duration)
        {
            _targetQuest.localScale = Vector3.Lerp(Vector3.one, Vector3.one * _maxScale, Mathf.Clamp01(time/duration));
            time += Time.deltaTime;

            yield return null;
        }

        _targetQuest.localScale = Vector3.one * _maxScale;

        yield return new WaitForSeconds(0.1f);

        duration = _downDuration;
        time = duration;

        while (time >= 0f)
        {
            var progress = time / duration;

            if (progress >= 0.5f) {
                _targetQuest.localScale = Vector3.Lerp(Vector3.one * _minScale, Vector3.one * _maxScale, Mathf.Clamp01((progress - 0.5f) * 2f));
            }
            else {
                _targetQuest.localScale = Vector3.Lerp(Vector3.one, Vector3.one * _minScale, Mathf.Clamp01(progress * 2f));
            }
            _targetGroup.alpha = Mathf.Lerp(0.3f, 1f, progress);
            _targetQuest.localScale = Vector3.one;
            time -= Time.deltaTime;

            yield return null;
        }

        _targetGroup.alpha = 0.3f;

        yield return new WaitForEndOfFrame();

        AudioManager.Instance.PlaySound(Sounds.WinQuest);

        duration = _rewardDuration;
        time = _rewardDuration;

        _reward.position = _rewardPositionTarget.position;
        _rewardGroup.alpha = 1f;

        while (time >= 0f)
        {
            _reward.localPosition += Vector3.up * _rewardSpeed * Time.deltaTime;

            if (time <= duration * 0.7f)
            {
                _rewardGroup.alpha = Mathf.Clamp01(time / duration);
            }

            time -= Time.deltaTime;

            yield return null;
        }

        _rewardGroup.alpha = 0f;
                        
        CompleteQuest(false);

        _targetQuest.localScale = Vector3.one;
        _targetQuest.gameObject.SetActive(true);
    }

    #region Events
    private void OnWatchAds()
    {
        CompleteQuest(true);
    }
    #endregion
}