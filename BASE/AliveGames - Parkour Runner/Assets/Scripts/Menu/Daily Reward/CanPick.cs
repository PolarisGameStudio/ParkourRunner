using System;
using UnityEngine;

public class CanPick : MonoBehaviour {
	[SerializeField] private DailyReward DailyReward;
	[SerializeField] private GameObject  TurnOnObject;


	private void OnEnable() {
		DailyReward.Initialize();
		TurnOnObject.SetActive(DailyReward.CanPickReward);
	}
}