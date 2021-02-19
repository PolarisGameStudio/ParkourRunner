using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay_UI {
	public class DoubleCoinsButton : MonoBehaviour {
		private const float ActiveTime = 7.5f;

		[SerializeField] private ResultDialogController ResultDialogController;
		[SerializeField] private Button Button;
		[SerializeField] private Image  Fade;

		private float _timer;


		private void Start() {
			Button.onClick.AddListener(OnClick);
		}


		private void Update() {
			if (_timer > 0) {
				_timer          -= Time.deltaTime;
				Fade.fillAmount =  (ActiveTime - _timer) / ActiveTime;
			}
			else {
				Hide();
			}

			Button.interactable = _timer > 0;
		}


		public void Reset() {
			_timer = ActiveTime;
		}

		public void Show() {
			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}


		private void OnClick() {
			if (AdManager.EnableAds) {
				AdManager.Instance.ShowRewardedVideo(OnWatchAd, null, null);
			}
			else {
				OnWatchAd();
			}
		}


		private void OnWatchAd() {
			Wallet.Instance.AddCoins(Wallet.Instance.InGameCoins, Wallet.WalletMode.InGame);
			ResultDialogController.UpdateLabels();
			Hide();
		}
	}
}