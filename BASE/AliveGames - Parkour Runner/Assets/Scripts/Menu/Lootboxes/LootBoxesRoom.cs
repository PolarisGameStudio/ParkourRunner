using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using AEngine;
using DG.Tweening;
using MainMenuAndShop.Helmets;
using MainMenuAndShop.Jetpacks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LootBoxesRoom : Menu {
	private const string BrownBoxLastPickKey  = "BrownBoxLastPick";
	private const string RedBoxLastPickKey    = "RedBoxTimerLastPick";
	private const string PurpleBoxLastPickKey = "PurpleBoxTimerLastPick";

	private const int    BrownBoxMaxPickTimes = 5;
	private const string BrownBoxPickTimesKey = "BrownBoxPickTimes";

	private static readonly int _canPickProperty = Animator.StringToHash("Can Pick");

	private static readonly TimeSpan BrownBoxTimerValue  = new TimeSpan(0, 0,  5, 0);
	private static readonly TimeSpan RedBoxTimerValue    = new TimeSpan(0, 0,  5, 0);
	private static readonly TimeSpan PurpleBoxTimerValue = new TimeSpan(0, 45, 0, 0);

	private static LootBoxesRoom Instance;
	private const  int           BrownMaxBonuses = 5;
	private const  int           RedMaxBonuses   = 10;
	private const  float         GiveRewardDelay = 1.45f;

	[SerializeField] private List<RewardDescription> Rewards;
	[SerializeField] private List<LootBox>           LootBoxPrefabs;

	[Space] [SerializeField] private MenuController  MenuController;
	[SerializeField]         private MovingAnimation HomeButtonAnim;
	[SerializeField]         private AlphaAnimation  BackgroundAnim;
	[SerializeField]         private GameObject      BackgroundWall;

	[Header("Reward Panel")] [SerializeField]
	private AlphaAnimation RewardPanelAlphaAnimation;
	[SerializeField] private Image RewardImage;
	[SerializeField] private Text  RewardText;
	[SerializeField] private Image RewardBackgroundImage;

	[Space] [SerializeField] private Color BrownColorReward;
	[SerializeField]         private Color RedColorReward;
	[SerializeField]         private Color PurpleColorReward;
	[SerializeField]         private Color GoldColorReward;

	[Space] [SerializeField] private DonatShopData GoldBoxData;
	[SerializeField]         private Text          GoldBoxPriceText;

	[Space] [SerializeField] private Text BrownBoxTimerText;
	[SerializeField]         private Text RedBoxTimerText;
	[SerializeField]         private Text PurpleBoxTimerText;

	[Space] [SerializeField] private Animator BrownBoxTimerTextAnimator;
	[SerializeField]         private Animator RedBoxTimerTextAnimator;
	[SerializeField]         private Animator PurpleBoxTimerTextAnimator;

	private WaitForSeconds     _updateTimer = new WaitForSeconds(1f);
	private Coroutine          _updateTimersCoroutine;
	private Queue<LootBoxType> _lootBoxesQueue = new Queue<LootBoxType>();
	private bool               _openLootBox;
	private LootBoxType        _openBoxType;

	private GameObject   _lootBoxInstance;
	private Reward       _reward;
	private LootBoxType? _openLootBoxOnShowRoom = null;

	private static readonly List<CharacterKinds> _skinsForRedLootBox = new List<CharacterKinds>() {
		CharacterKinds.Character1,
		CharacterKinds.Character1Fem,
		CharacterKinds.Character2,
		CharacterKinds.Character2Fem,
		CharacterKinds.Character3,
		CharacterKinds.Character3Fem,
		CharacterKinds.Character4,
		CharacterKinds.Character4Fem,
	};

	private static readonly List<CharacterKinds> _skinsForPurpleLootBox = new List<CharacterKinds>() {
		CharacterKinds.CaptainAmerica,
		CharacterKinds.CaptainAmericaFem,
	};

	private static readonly List<Jetpacks.JetpacksType> _jetpacksForPurpleLootBox = new List<Jetpacks.JetpacksType>() {
		Jetpacks.JetpacksType.Jetpack1,
		Jetpacks.JetpacksType.Jetpack2,
		Jetpacks.JetpacksType.Jetpack3
	};

	private static readonly List<CharacterKinds> _skinsForGoldLootBox = new List<CharacterKinds>() {
		CharacterKinds.CaptainAmerica,
		CharacterKinds.CaptainAmericaFem,
	};

	private static readonly List<Jetpacks.JetpacksType> _jetpacksForGoldLootBox = new List<Jetpacks.JetpacksType>() {
		Jetpacks.JetpacksType.Jetpack4,
		Jetpacks.JetpacksType.Jetpack5,
		Jetpacks.JetpacksType.Jetpack6
	};


	private void Awake() {
		Instance = this;
	}


	private void Start() {
		StartCoroutine(LoadingDataProcess());
	}


	private void OnEnable() {
		_updateTimersCoroutine    =  StartCoroutine(UpdateTimersText());
		InAppManager.OnBuySuccess += OnBuySuccess;
	}


	private void OnDisable() {
		if (_updateTimersCoroutine != null) {
			StopCoroutine(_updateTimersCoroutine);
			_updateTimersCoroutine = null;
		}
		InAppManager.OnBuySuccess -= OnBuySuccess;
	}


	private void OnBuySuccess(DonatShopData.DonatKinds productKind) {
		if (GoldBoxData.DonatKind == productKind) {
			Shoping.GetDonat(GoldBoxData);
			AudioManager.Instance.PlaySound(Sounds.ShopSlot);
		}
	}


	protected override void Show() {
		base.Show();

		Instance.BackgroundWall.SetActive(true);
		HomeButtonAnim.Show();
		RewardPanelAlphaAnimation.Hide();

		var sequence = DOTween.Sequence();
		sequence.Append(Instance.BackgroundAnim.Hide());
		if (_openLootBoxOnShowRoom.HasValue) {
			sequence.onComplete    += () => OpenLootBox(_openLootBoxOnShowRoom.Value);
			_openLootBoxOnShowRoom =  null;
		}

		// AdManager.Instance.ShowBanner();
	}


	protected override void StartHide(Action callback) {
		base.StartHide(callback);
		RewardPanelAlphaAnimation.Hide();
		HomeButtonAnim.Hide();
		_openLootBox = false;
		// AdManager.Instance.HideBottomBanner();

		var sequence = DOTween.Sequence();
		sequence.Append(BackgroundAnim.Show());

		sequence.OnComplete(() => {
			Instance.BackgroundWall.SetActive(false);
			HideLootBoxReward();
			FinishHide(callback);
		});
	}


	public void ShowReward(string rewardText, Sprite rewardIcon, Color backgroundColor) {
		RewardPanelAlphaAnimation.Show();
		RewardText.text             = rewardText;
		RewardBackgroundImage.color = backgroundColor;
		RewardImage.sprite          = rewardIcon;
	}


	public void OnCloseRewardPanelButton() {
		_audio.PlaySound(Sounds.Tap);
		HideLootBoxReward();

		_openLootBox = false;
		if (_lootBoxesQueue.Count > 0) {
			var box = _lootBoxesQueue.Dequeue();
			OpenLootBox(box);
		}
	}


	public void OnHomeButton() {
		_audio.PlaySound(Sounds.Tap);
		_menuController.OpenMenu(MenuKinds.MainMenu);
	}


	private IEnumerator LoadingDataProcess() {
		while (!InAppManager.Instance.IsInitialized())
			yield return null;

		RefreshProductData();
	}


	private IEnumerator UpdateTimersText() {
		while (true) {
			UpdateTimerText(BrownBoxTimerText, BrownBoxTimerTextAnimator, LootBoxType.Brown);
			UpdateTimerText(RedBoxTimerText, RedBoxTimerTextAnimator, LootBoxType.Red);
			UpdateTimerText(PurpleBoxTimerText, PurpleBoxTimerTextAnimator, LootBoxType.Purple);
			yield return _updateTimer;
		}
	}


	private void OnGiveReward() {
		ShowRewardPanel(_reward);
		// GiveReward(_reward);
	}


	public static void OpenLootBox(LootBoxType type) {
		if (Instance._openLootBox) {
			Instance._lootBoxesQueue.Enqueue(type);
			return;
		}
		Instance.HideLootBoxReward();

		if (Instance.MenuController.CurrentMenu.Kind == MenuKinds.LootBoxes) {
			Instance._openLootBox = true;
			SpawnLootBox(type);
			Instance._openBoxType = type;
			Instance._reward      = GetReward(type);
			Instance.Invoke(nameof(OnGiveReward), GiveRewardDelay);

			return;
		}

		Instance._openLootBoxOnShowRoom = type;
		Instance.MenuController.OpenMenu(MenuKinds.LootBoxes);
	}


	private static void SpawnLootBox(LootBoxType type) {
		var lootBoxPrefab = Instance.GetLootBoxPrefab(type);
		Instance._lootBoxInstance = Instantiate(lootBoxPrefab);
	}


	private void HideLootBoxReward() {
		if (_lootBoxInstance) Destroy(_lootBoxInstance);

		var sequence = DOTween.Sequence();
		sequence.Append(RewardPanelAlphaAnimation.Hide());
	}


	private GameObject GetLootBoxPrefab(LootBoxType type) {
		return LootBoxPrefabs.FirstOrDefault(l => l.Type == type)?.LootBoxPrefab;
	}


	private static Reward GetReward(LootBoxType type) {
		switch (type) {
			case LootBoxType.Brown:
				return GetRandomBonus(BrownMaxBonuses);
			case LootBoxType.Red:
				var redFreeSkins = CharactersData.GetFreeSkinList(_skinsForRedLootBox);
				if (redFreeSkins.Count > 0) {
					const float bonusChance = 0.8f;
					var         random      = Random.value;
					return random <= bonusChance ? GetRandomBonus(RedMaxBonuses) : GetRandomSkin(redFreeSkins);
				}
				return GetRandomBonus(RedMaxBonuses);
			case LootBoxType.Purple:
				return GetExtraReward(false);
			case LootBoxType.Gold:
				return GetExtraReward(true);
			default: throw new InvalidEnumArgumentException();
		}
	}


	private static Reward GetRandomBonus(int maxBonuses) {
		var bonusNames   = Enum.GetNames(typeof(BonusName));
		var randomBonus  = (BonusName) Random.Range(0, bonusNames.Length);
		var randomAmount = Random.Range(1, maxBonuses);
		return new Reward(RewardType.Bonus, randomBonus.ToString(), randomAmount);
	}


	private static Reward GetRandomSkin(List<CharacterKinds> skins) {
		var skinIndex = Random.Range(0, skins.Count);
		var skin      = skins[skinIndex];
		return new Reward(RewardType.Skin, skin.ToString());
	}


	private static Reward GetRandomHelmet(List<Helmets.HelmetsType> helmets) {
		var helmetIndex = Random.Range(0, helmets.Count);
		var helmet      = helmets[helmetIndex];
		return new Reward(RewardType.Helmet, helmet.ToString());
	}


	private static Reward GetRandomJetpack(List<Jetpacks.JetpacksType> jetpacks) {
		var jetpackIndex = Random.Range(0, jetpacks.Count);
		var jetpack      = jetpacks[jetpackIndex];
		return new Reward(RewardType.Jetpack, jetpack.ToString());
	}


	private static Reward GetExtraReward(bool isGold) {
		var skins    = CharactersData.GetFreeSkinList(isGold ? _skinsForGoldLootBox : _skinsForPurpleLootBox);
		var jetpacks = Jetpacks.GetFreeJetpacksList(isGold ? _jetpacksForGoldLootBox : _jetpacksForPurpleLootBox);
		var helmets  = Helmets.GetFreeHelmetsList();

		var rewards = new List<RewardType>();
		for (int i = 0; i < skins.Count; i++) rewards.Add(RewardType.Skin);
		for (int i = 0; i < jetpacks.Count; i++) rewards.Add(RewardType.Jetpack);
		for (int i = 0; i < helmets.Count; i++) rewards.Add(RewardType.Helmet);

		var reward    = rewards[Random.Range(0, rewards.Count)];
		var parameter = "";

		switch (reward) {
			case RewardType.Skin:
				return GetRandomSkin(skins);
			case RewardType.Jetpack:
				return GetRandomJetpack(jetpacks);
			case RewardType.Helmet:
				return GetRandomHelmet(helmets);
			default: throw new InvalidEnumArgumentException();
		}
	}


	private static void GiveReward(Reward reward) {
		switch (reward.Type) {
			case RewardType.Bonus:
				var bonus = (BonusName) Enum.Parse(typeof(BonusName), reward.RewardParameter, true);
				CollectibleBonuses.AddBonus(bonus, reward.Amount);
				break;
			case RewardType.Skin:
				var skin     = (CharacterKinds)  Enum.Parse(typeof(CharacterKinds), reward.RewardParameter);
				var skinData = CharactersData.GetCharacterData(skin);
				skinData.Bought = true;
				break;
			case RewardType.Jetpack:
				var jetpack = (Jetpacks.JetpacksType)  Enum.Parse(typeof(Jetpacks.JetpacksType), reward.RewardParameter);
				Jetpacks.GetJetpackData(jetpack).Bought = true;
				break;
			case RewardType.Helmet:
				var helmet = (Helmets.HelmetsType)  Enum.Parse(typeof(Helmets.HelmetsType), reward.RewardParameter);
				Helmets.GetHelmetData(helmet).Bought = true;
				break;
		}
	}


	private void ShowRewardPanel(Reward reward) {
		var rewardDescription = Rewards.FirstOrDefault(r => r.Type == reward.Type && r.RewardParameter == reward.RewardParameter);
		var text              = rewardDescription.Name + (reward.Type == RewardType.Bonus ? $" x{reward.Amount}" : "");
		var sprite            = rewardDescription.Icon;

		var color = BrownColorReward;
		switch (_openBoxType) {
			case LootBoxType.Red:
				color = RedColorReward;
				break;
			case LootBoxType.Purple:
				color = PurpleColorReward;
				break;
			case LootBoxType.Gold:
				color = GoldColorReward;
				break;
		}

		ShowReward(text, sprite, color);
	}


	private TimeSpan GetTimer(LootBoxType boxType) {
		string   key;
		TimeSpan maxTimer;

		switch (boxType) {
			case LootBoxType.Brown:
				key      = BrownBoxLastPickKey;
				maxTimer = BrownBoxTimerValue;
				break;
			case LootBoxType.Red:
				key      = RedBoxLastPickKey;
				maxTimer = RedBoxTimerValue;
				break;
			case LootBoxType.Purple:
				key      = PurpleBoxLastPickKey;
				maxTimer = PurpleBoxTimerValue;
				break;
			default: throw new InvalidEnumArgumentException();
		}

		return GetLastPickTime(key) + maxTimer - DateTime.Now;
	}


	private string TimerToText(TimeSpan timer) {
		timer = timer <= TimeSpan.Zero ? TimeSpan.Zero : timer;
		return timer.Hours > 0 ? $"{timer:hh\\:mm\\:ss}" : $"{timer:mm\\:ss}";
	}


	private void UpdateTimerText(Text timerText, Animator animator, LootBoxType type) {
		var timer = GetTimer(type);
		if (timer <= TimeSpan.Zero) {
			timerText.text = "Tap";
			animator.SetBool(_canPickProperty, true);
		}
		else {
			timerText.text = TimerToText(timer);
			animator.SetBool(_canPickProperty, false);
		}
	}


	private void RefreshProductData() {
		var price    = InAppManager.Instance.GetLocalizedPrice(GoldBoxData.ProductGameId);
		var currency = InAppManager.Instance.GetLocalizedCurrency(GoldBoxData.ProductGameId);
		GoldBoxPriceText.text = $"{price} {currency}";
	}


	public void SelectLootBox(int id) {
		var lootBoxType = (LootBoxType) id;

		switch (lootBoxType) {
			case LootBoxType.Gold:
#if UNITY_EDITOR
				OnBuySuccess(GoldBoxData.DonatKind);
				return;
#endif
				InAppManager.Instance.BuyProductID(GoldBoxData.ProductGameId);
				break;
			case LootBoxType.Brown:
				if (!CanOpenBox(LootBoxType.Brown)) return;
				OpenLootBox(LootBoxType.Brown);
				ResetBoxTime(BrownBoxLastPickKey);
				break;
			case LootBoxType.Red:
				if (!CanOpenBox(LootBoxType.Red)) return;
				AdManager.Instance.ShowRewardedVideo(delegate {
					OpenLootBox(LootBoxType.Red);
					ResetBoxTime(RedBoxLastPickKey);
				}, null, null);
				break;
			case LootBoxType.Purple:
				if (!CanOpenBox(LootBoxType.Purple)) return;
				AdManager.Instance.ShowRewardedVideo(delegate {
					OpenLootBox(LootBoxType.Purple);
					ResetBoxTime(PurpleBoxLastPickKey);
				}, null, null);
				break;
		}
	}


	public static bool CanOpenBox(LootBoxType lootBoxType) {
		switch (lootBoxType) {
			case LootBoxType.Brown:
				var lastPickTime = GetLastPickTime(BrownBoxLastPickKey);
				if (lastPickTime.Day == DateTime.Now.Day) {
					if (PlayerPrefs.HasKey(BrownBoxPickTimesKey)) {
						var pickTimes = PlayerPrefs.GetInt(BrownBoxPickTimesKey);
						if (pickTimes >= BrownBoxMaxPickTimes) return false;
					}
				}
				return DateTime.Now - lastPickTime > BrownBoxTimerValue;
			case LootBoxType.Red:
				return DateTime.Now - GetLastPickTime(RedBoxLastPickKey) > RedBoxTimerValue;
			case LootBoxType.Purple:
				return DateTime.Now - GetLastPickTime(PurpleBoxLastPickKey) > PurpleBoxTimerValue;
			default: throw new InvalidEnumArgumentException();
		}
	}


	private static DateTime GetLastPickTime(string key) {
		var format = "O";

		if (PlayerPrefs.HasKey(key)) {
			var lastPickStr = PlayerPrefs.GetString(key);
			var lastPick    = DateTime.ParseExact(lastPickStr, format, CultureInfo.InvariantCulture);

			if (lastPick > DateTime.Now) {
				ResetBoxTime(key);
				return DateTime.Now;
			}
			return lastPick;
		}

		ResetBoxTime(key);
		return DateTime.Now;
	}


	private static string GetTimerKey(LootBoxType lootBoxType) {
		switch (lootBoxType) {
			case LootBoxType.Brown:
				return BrownBoxLastPickKey;
			case LootBoxType.Red:
				return RedBoxLastPickKey;
			case LootBoxType.Purple:
				return PurpleBoxLastPickKey;
			default: throw new InvalidEnumArgumentException();
		}
	}


	private static void ResetBoxTime(string key) {
		var format  = "O";
		var strTime = DateTime.Now.ToString(format);
		PlayerPrefs.SetString(key, strTime);
		PlayerPrefs.Save();
	}


	[Serializable]
	protected class LootBox {
		public GameObject  LootBoxPrefab;
		public LootBoxType Type;
	}

	[Serializable]
	protected class RewardDescription {
		public string     RewardParameter;
		public string     Name;
		public RewardType Type;
		public Sprite     Icon;
	}

	private class Reward {
		public RewardType Type;
		public string     RewardParameter;
		public int        Amount;


		public Reward(RewardType type, string rewardParameter, int amount) {
			Type            = type;
			RewardParameter = rewardParameter;
			Amount          = amount;
		}


		public Reward(RewardType type, string rewardParameter) {
			Type            = type;
			RewardParameter = rewardParameter;
			Amount          = 1;
		}
	}

	public enum LootBoxType {
		Brown,
		Red,
		Purple,
		Gold
	}

	public enum RewardType {
		Bonus,
		Skin,
		Jetpack,
		Helmet
	}
}