using UnityEngine;

public class QuestItem : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private bool _isEnable;

    public int ID { get { return _id; } }

    public bool IsEnable { get { return _isEnable; } }
}