using UnityEngine;
using UnityEngine.UI;

namespace Quests {
	public class QuestsCounter : MonoBehaviour {
		[SerializeField] private Text Counter;


		private void Start() {
			QuestManager.Instance.OnUpdateActiveQuestsEvent += UpdateCounter;
			UpdateCounter();
		}


		private void UpdateCounter() {
			var count = QuestManager.Instance.ActiveQuests.Count;
			gameObject.SetActive(count > 0);
			Counter.text = count.ToString();
		}
	}
}