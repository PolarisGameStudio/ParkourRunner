using System;
using System.Collections;
using System.Globalization;
using System.Timers;
using AEngine;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DefaultNamespace.Menu {
	public class FreeGift : MonoBehaviour {
		private const string LastPickKey = "LastGiftPick";

		[SerializeField] private Text      Timer;
		[SerializeField] private Button    GiftButton;
		[SerializeField] private Animation GiftButtonAnimation;

		[Space] [SerializeField] private GameObject GiftPanel;
		[SerializeField]         private Image      GiftImage;
		[SerializeField]         private Text       GiftText;

		[Space] [SerializeField] private Sprite                CoinsSprite;
		[SerializeField]         private LocalizationComponent CoinsText;

		private readonly WaitForSeconds _oneSecond = new WaitForSeconds(1f);
		private readonly TimeSpan       _maxTimer  = new TimeSpan(0, 0, 30, 0);

		private DateTime LastPick {
			get {
				var format = "O";

				if (PlayerPrefs.HasKey(LastPickKey)) {
					var lastPickStr = PlayerPrefs.GetString(LastPickKey);
					var lastPick    = DateTime.ParseExact(lastPickStr, format, CultureInfo.InvariantCulture);

					if (lastPick > DateTime.Now) {
						ResetTime();
						return DateTime.Now;
					}
					return lastPick;
				}

				ResetTime();
				return DateTime.Now;
			}
		}


		private void OnEnable() {
			foreach (AnimationState state in GiftButtonAnimation) {
				state.speed = 0.5f;
			}
			StartCoroutine(UpdateTimer());
		}


		private void OnDisable() {
			StopCoroutine(UpdateTimer());
		}


		private IEnumerator UpdateTimer() {
			while (true) {
				var timer = GetTimer();
				Timer.text = $"{timer:mm\\:ss}";

				var canPick = timer <= TimeSpan.Zero;
				GiftButton.interactable = canPick;
				if (canPick && !GiftButtonAnimation.isPlaying) GiftButtonAnimation.Play();
				yield return _oneSecond;
			}
		}


		private TimeSpan GetTimer() {
			var timer =  LastPick + _maxTimer - DateTime.Now;
			return timer <= TimeSpan.Zero ? TimeSpan.Zero : timer;
		}


		private void ResetTime() {
			var format  = "O";
			var strTime = DateTime.Now.ToString(format);
			PlayerPrefs.SetString(LastPickKey, strTime);
			PlayerPrefs.Save();
		}


		public void OnOpenButtonClick() {
			if(AdManager.EnableAds) AdManager.Instance.ShowRewardedVideo(TakeGift, null, TakeGift);
		}


		private void TakeGift() {
			ResetTime();

			var gift = GetRandomGift();
			OpenGiftPanel(gift);
			OpenGift(gift);
		}


		private void OpenGiftPanel(Gift gift) {
			GiftImage.sprite = gift.Icon;
			GiftText.text    = GetGiftDescription(gift);

			GiftPanel.SetActive(true);
		}


		public void CloseGiftPanel() {
			GiftPanel.SetActive(false);
		}


		private string GetGiftDescription(Gift gift) {
			switch (gift.GiftType) {
				case Gift.Type.Coins:
					return $"{gift.Data} {CoinsText.Text}";
				default: return string.Empty;
			}
		}


		private void OpenGift(Gift gift) {
			AudioManager.Instance.PlaySound(Sounds.ShopSelect);
			switch (gift.GiftType) {
				case Gift.Type.Coins:
					Wallet.Instance.AddCoins(int.Parse(gift.Data), Wallet.WalletMode.Global);
					break;
			}
		}


		private Gift GetRandomGift() {
			var randomCoins = Random.Range(4, 10) * 50;
			return new Gift {Icon = CoinsSprite, Data = randomCoins.ToString(), GiftType = Gift.Type.Coins};
		}


		private struct Gift {
			public Sprite Icon;
			public Type   GiftType;
			public string Data;

			public enum Type {
				Coins
			}
		}
	}
}