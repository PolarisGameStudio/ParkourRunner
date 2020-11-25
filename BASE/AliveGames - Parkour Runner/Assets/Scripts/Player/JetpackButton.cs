using System;
using MainMenuAndShop.Jetpacks;
using ParkourRunner.Scripts.Player;
using ParkourRunner.Scripts.Player.InvectorMods;
using UnityEngine;
using UnityEngine.UI;

public class JetpackButton : MonoBehaviour {
	[SerializeField] private Button JetpackBtn;
	[SerializeField] private Image  JetpackFuelLeftImage;

	private JetpackController _jetpackController;


	private void Start() {
		_jetpackController = ParkourThirdPersonController.instance.GetComponent<JetpackController>();
		gameObject.SetActive(Jetpacks.ActiveJetpackType != Jetpacks.JetpacksType.NoJetpack);
	}


	private void Update() {
		var fuel = _jetpackController.GetProgress();

		JetpackBtn.interactable         = fuel > 0f;
		JetpackFuelLeftImage.fillAmount = fuel;
	}


	public void Activate() {
		_jetpackController.ActivateJetpack();
	}
}