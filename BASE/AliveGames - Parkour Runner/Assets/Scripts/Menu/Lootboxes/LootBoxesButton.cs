using System;
using System.Collections;
using UnityEngine;

public class LootBoxesButton : MonoBehaviour {
	[SerializeField] private MenuController MenuController;
	[SerializeField] private Animator       LootboxButtonAnimator;

	private readonly WaitForSeconds _oneSecond = new WaitForSeconds(1f);
	private static readonly int _canPickProperty = Animator.StringToHash("Can Pick");


	private void OnEnable() {
		StartCoroutine(UpdateTimer());
	}


	private void OnDisable() {
		StopCoroutine(UpdateTimer());
	}


	private IEnumerator UpdateTimer() {
		yield return new WaitForSeconds(0.5f);
		while (true) {
			if (LootBoxesRoom.CanOpenBox(LootBoxesRoom.LootBoxType.Brown) ||
				LootBoxesRoom.CanOpenBox(LootBoxesRoom.LootBoxType.Red)   ||
				LootBoxesRoom.CanOpenBox(LootBoxesRoom.LootBoxType.Purple)) {
				LootboxButtonAnimator.SetBool(_canPickProperty, true);
			}
			else LootboxButtonAnimator.SetBool(_canPickProperty, false);

			yield return _oneSecond;
		}
	}


	public void OnClick() {
		MenuController.OpenMenu(MenuKinds.LootBoxes);
	}
}