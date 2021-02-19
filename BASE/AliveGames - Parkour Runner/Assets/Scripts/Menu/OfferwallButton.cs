using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using AEngine;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OfferwallButton : MonoBehaviour {
	[SerializeField] private Button Button;
	[SerializeField] private Text   Text;
	public                   float  AnimationMaxTextSize = 1.25f;
	public                   float  AnimationSpeed = 2f;


	private void Update() {
		Button.interactable = TapjoyController.OfferwallAvailable;

		if (TapjoyController.OfferwallAvailable) {
			var sin   = (Mathf.Sin(Time.time * AnimationSpeed) + 1) / 2f;
			var value = Mathf.Lerp(1f, AnimationMaxTextSize, sin);
			var scale = new Vector2(value, value);
			Text.transform.localScale = scale;
		}
		else {
			Text.transform.localScale = Vector3.one;
		}
	}


	public void ShowOfferwall() {
		if (TapjoyController.OfferwallAvailable) {
			TapjoyController.ShowOfferwall();
		}
	}
}