using System;
using UnityEngine;
using UnityEngine.UI;

namespace Quests {
	public class QuestsCounter : MonoBehaviour {
		[SerializeField] private Text Counter;


		private void OnEnable() {
			UpdateCounter();
		}


		private void UpdateCounter() {
			print("UpdateCounter");
			var count = QuestManager.Instance.ActiveQuests.Count - QuestManager.Instance.CompletedQuests.Count;
			gameObject.SetActive(count > 0);
			Counter.text = count.ToString();
		}
	}
}