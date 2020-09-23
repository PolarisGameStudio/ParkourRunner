using UnityEngine;
using AEngine;

public class QuestTask : MonoBehaviour
{
    [SerializeField] protected QuestData _data;

    protected bool IsEnable { get { return QuestManager.Instance.ActiveQuests.Contains(_data); } }

    protected void CompleteQuest(bool playSound)
    {
        if (playSound)
            AudioManager.Instance.PlaySound(Sounds.WinQuest);

        QuestManager.Instance.CompleteQuest(_data.ID);
        Wallet.Instance.AddCoins(_data.Reward, Wallet.WalletMode.Global);
    }
}