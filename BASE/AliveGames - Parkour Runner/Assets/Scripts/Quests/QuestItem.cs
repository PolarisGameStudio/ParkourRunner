using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour
{
    [SerializeField] private QuestData _data;
    [SerializeField] private Text PriceText;

    public QuestData Data => _data;


    private void Start() {
        PriceText.text = _data.Reward.ToString();
    }
}