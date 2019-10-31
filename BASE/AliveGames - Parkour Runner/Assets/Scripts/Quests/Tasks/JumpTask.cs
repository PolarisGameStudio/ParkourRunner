using UnityEngine;
using ParkourRunner.Scripts.Player;
using ParkourRunner.Scripts.Managers;

public class JumpTask : QuestTask
{
    // Смотреть также JumpProgressUI
    [SerializeField] private int _maxJumps;

    private bool _isActive;

    private void OnEnable()
    {
        if (IsEnable)
        {
            _isActive = true;

            RandomTricks.OnJumpOverObstacle -= OnPlayerJump;
            RandomTricks.OnJumpOverObstacle += OnPlayerJump;
        }
    }

    private void OnDisable()
    {
        _isActive = false;

        RandomTricks.OnJumpOverObstacle -= OnPlayerJump;
    }

    #region Events
    private void OnPlayerJump()
    {
        if (_isActive)
        {
            QuestManager.Instance.AddJumpProgressTick();

            if (QuestManager.Instance.GetJumpProgress() >= _maxJumps)
            {
                RandomTricks.OnJumpOverObstacle -= OnPlayerJump;
                _isActive = false;

                HUDManager.Instance.ShowGreatMessage(HUDManager.Messages.QuestComplete);
                CompleteQuest(true);
            }
        }        
    }
    #endregion
}