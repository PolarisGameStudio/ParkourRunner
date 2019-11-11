using UnityEngine;
using UnityEngine.UI;

public class JumpProgressUI : MonoBehaviour
{
    // Смотреть также JumpTask
    [SerializeField] private int _maxJumps;
    [SerializeField] private Text _label;

    private void OnEnable()
    {
        int progress = QuestManager.Instance.GetJumpProgress();
        _label.text = progress.ToString() + "/" + _maxJumps.ToString();
    }
}