using UnityEngine;
using UnityEngine.UI;

public class RewardCard : MonoBehaviour {
	[SerializeField] private Text       Day;
	[SerializeField] private Image      Icon;
	[SerializeField] private Text       Description;
	[SerializeField] private GameObject Completed;
	[SerializeField] private Color      CanPickColor;


	public void SetData(int day, DailyReward.Reward reward) {
		Day.text         = day.ToString();
		Icon.sprite      = reward.Icon;
		Description.text = reward.Value.ToString();

		if (reward.RewardType == DailyReward.RewardType.FreeSkin) {
			Description.gameObject.SetActive(false);
			var iconRT = Icon.GetComponent<RectTransform>();
			var iconSize = iconRT.sizeDelta;
			iconSize.y = 315f;
			iconRT.sizeDelta = iconSize;
		}
	}


	public void CompleteDay() {
		Completed.SetActive(true);
	}


	public void CanBePicked(bool state) {
		var image = GetComponent<Image>();
		image.color = state ? CanPickColor : Color.white;
	}
}