using System;
using ParkourRunner.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class CollectedBonusActivator : MonoBehaviour {
	[SerializeField] private BonusCell[] Bonuses;


	private void Start() {
		if (PlayerPrefs.GetInt(EnvironmentController.MULTIPLAYER_KEY) == 1 ||
			PlayerPrefs.GetInt(EnvironmentController.TUTORIAL_KEY)    == 1) {
			foreach (var bonus in Bonuses) {
				bonus.CanvasGroup.gameObject.SetActive(false);
			}
			return;
		}

		UpdateBonuses();
	}


	/*private void Update() {
		var index                                    = -1;
		if (Input.GetKeyDown(KeyCode.Keypad0)) index = 0;
		if (Input.GetKeyDown(KeyCode.Keypad1)) index = 1;
		if (Input.GetKeyDown(KeyCode.Keypad2)) index = 2;
		if (Input.GetKeyDown(KeyCode.Keypad3)) index = 3;
		if (Input.GetKeyDown(KeyCode.Keypad4)) index = 4;

		if (index > -1 && index < 5) {
			CollectibleBonuses.AddBonus((BonusName) index, 5);
			UpdateBonuses();
		}
	}*/


	private void UpdateBonuses() {
		foreach (var bonusCell in Bonuses) {
			var amount = CollectibleBonuses.GetBonusesAmount(bonusCell.BonusName);
			bonusCell.CountText.text    = $"x{amount}";
			bonusCell.CanvasGroup.alpha = amount > 0 ? 1 : 0.5f;
		}
	}


	public void OnActivateBonus(int bonusIndex) {
		if (GameManager.Instance.gameState != GameManager.GameState.Run) return;
		var bonus  = (BonusName) bonusIndex;
		var canUse = CollectibleBonuses.UseBonus(bonus, 1);
		if (canUse) {
			GameManager.Instance.AddBonus(bonus);
			UpdateBonuses();
		}

		BlockActivate();
	}


	public void ShowPanel() {
		gameObject.SetActive(true);
	}


	public void HidePanel() {
		gameObject.SetActive(false);
	}


	private void BlockActivate() {
		foreach (var bonusCell in Bonuses) {
			bonusCell.CanvasGroup.alpha        = 0.5f;
			bonusCell.CanvasGroup.interactable = false;
		}
	}


	[Serializable]
	protected class BonusCell {
		[SerializeField] protected internal BonusName   BonusName;
		[SerializeField] protected internal Text        CountText;
		[SerializeField] protected internal CanvasGroup CanvasGroup;
	}
}