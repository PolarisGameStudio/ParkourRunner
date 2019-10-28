using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Data", menuName = "Quest Data", order = 54)]
public class QuestData : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private bool _enable;
    [SerializeField] private int _reward;

    public int ID { get { return _id; } }

    public bool Enable { get { return _enable; } }

    public int Reward { get { return _reward; } }
}