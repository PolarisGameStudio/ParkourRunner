using UnityEngine;
using UnityEngine.UI;

public class PickRewardPanel : MonoBehaviour {
	[SerializeField] private Image Icon;
	[SerializeField] private Text  Description;

	private float _smallIconHeight = 342f;
	private float _bigIconHeight   = 484f;


	public void SetData(DailyReward.Reward reward) {
		Icon.sprite      = reward.Icon;
		Description.text = reward.Value.ToString();

		var iconRT   = Icon.GetComponent<RectTransform>();
		var iconSize = iconRT.sizeDelta;
		iconSize.y = _smallIconHeight;

		Description.gameObject.SetActive(true);

		if (reward.RewardType == DailyReward.RewardType.FreeSkin) {
			Description.gameObject.SetActive(false);
			iconSize.y = _bigIconHeight;
		}

		iconRT.sizeDelta = iconSize;
	}
}