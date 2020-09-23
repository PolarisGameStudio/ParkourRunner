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

namespace DefaultNamespace.Menu {
	public class FreeGift : MonoBehaviour {
		private const string LastPickKey = "LastGiftPick";

		[SerializeField] private Text      Timer;
		[SerializeField] private Button    GiftButton;
		[SerializeField] private Animation GiftButtonAnimation;

		[Space] [SerializeField] private GameObject GiftPanel;
		[SerializeField]         private Image      GiftImage;
		[SerializeField]         private Text       GiftText;

		[Space] [SerializeField] private List<GiftType> GiftTypes;

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
			if (AdManager.EnableAds) AdManager.Instance.ShowRewardedVideo(TakeGift, null, TakeGift);
			TakeGift();
		}


		private void TakeGift() {
			ResetTime();

			var gift = GetRandomGift();
			OpenGiftPanel(gift);
			OpenGift(gift);
			GiftButton.interactable = false;
		}


		private void OpenGiftPanel(Gift gift) {
			GiftImage.sprite = gift.Icon;
			GiftText.text = gift.Description;//GetGiftDescription(gift);

			GiftPanel.SetActive(true);
		}


		public void CloseGiftPanel() {
			GiftPanel.SetActive(false);
		}


		private string GetGiftDescription(GiftType gift, int amount) {
			if(gift.Description) return $"x{amount} {gift.Description.Text}";
			return $"x{amount}";

			/*switch (gift.GiftType) {
				case Gift.Type.Coins:
					return $"{gift.Amount} {CoinsText.Text}";
				case Gift.Type.Boost:
				case Gift.Type.Jump:
				case Gift.Type.Magnet:
				case Gift.Type.Shield:
				case Gift.Type.DoubleCoins:
					return $"{gift.Amount}x{gift.GiftType}";
				default: return string.Empty;
			}*/
		}


		private void OpenGift(Gift gift) {
			AudioManager.Instance.PlaySound(Sounds.ShopSelect);
			switch (gift.GiftType) {
				case Gift.Type.Coins:
					Wallet.Instance.AddCoins(gift.Amount, Wallet.WalletMode.Global);
					break;
				case Gift.Type.Boost:
				case Gift.Type.Jump:
				case Gift.Type.Magnet:
				case Gift.Type.Shield:
				case Gift.Type.DoubleCoins:
					BonusName type;
					switch (gift.GiftType) {
						case Gift.Type.Boost:
							type = BonusName.Boost;
							break;
						case Gift.Type.Jump:
							type = BonusName.Jump;
							break;
						case Gift.Type.Magnet:
							type = BonusName.Magnet;
							break;
						case Gift.Type.Shield:
							type = BonusName.Shield;
							break;
						case Gift.Type.DoubleCoins:
							type = BonusName.DoubleCoins;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					CollectibleBonuses.AddBonus(type, gift.Amount);
					break;
			}
		}


		private Gift GetRandomGift() {
			var rnd = Random.Range(0, 100);
			var giftType     = GiftTypes.FirstOrDefault(g => rnd >= g.FromChance && rnd <= g.ToChance);
			var randomAmount = Random.Range(giftType.MinAmount, giftType.MaxAmount) * giftType.AmountMultiplier;
			return new Gift {Icon = giftType.Icon, Amount = randomAmount, GiftType = giftType.Type, Description = GetGiftDescription(giftType, randomAmount)};
		}


		[Serializable]
		private struct GiftType {
			public                 Sprite                Icon;
			public                 Gift.Type             Type;
			[Range(0, 100)] public int                   FromChance;
			[Range(0, 100)] public int                   ToChance;
			public                 int                   MinAmount;
			public                 int                   MaxAmount;
			public                 int                   AmountMultiplier;
			public                 LocalizationComponent Description;
		}

		private struct Gift {
			public Sprite Icon;
			public Type   GiftType;
			public int    Amount;
			public string Description;

			public enum Type {
				Coins,
				Boost,
				DoubleCoins,
				Jump,
				Magnet,
				Shield,
			}
		}
	}
}