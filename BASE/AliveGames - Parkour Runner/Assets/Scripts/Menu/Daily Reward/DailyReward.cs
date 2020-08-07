using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using DG.Tweening;
using AEngine;
using Managers;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Utilities;
using Button = UnityEngine.UI.Button;

public class DailyReward : Menu {
	public event Action OnShowMenu;
	public event Action OnHideMenu;

	[Space] [SerializeField] private List<Reward> Rewards;
	[SerializeField]         private List<Sprite> CharacterSkins;

	[Space] [SerializeField] private Sprite         CoinsIcon;
	[SerializeField]         private Sprite         Coins2Icon;
	[SerializeField]         private CharactersData CharactersData;

	[Space] [SerializeField] private MovingAnimation RewardPanelAnim;
	[SerializeField]         private MovingAnimation RewardLabelAnim;
	[SerializeField]         private MovingAnimation RewardButtonsAnim;
	[SerializeField]         private AlphaAnimation  FadeScreenAnim;
	[SerializeField]         private AlphaAnimation  PickedRewardPanelAnim;

	[Space] [SerializeField] private Transform       RewardContainer;
	[SerializeField]         private Transform       RewardPrefab;
	[SerializeField]         private PickRewardPanel PickRewardPanel;

	[Space] [SerializeField] private Button PickButton;
	[SerializeField]         private Button BackButton;

	private const string SaveFileName     = "DailyRewards.dat";
	private const string StartWeekKey     = "StartDailyWeek";
	private const string LastDayKey       = "LastDay";
	private const string PickedRewardsKey = "PickedRewards";

	private List<RewardCard>     _rewardCards  = new List<RewardCard>();
	private List<CharacterKinds> _freeSkinList = new List<CharacterKinds>();
	private List<SavedReward>    WeekRewards   = new List<SavedReward>();

	public bool CanPickReward => !WeekRewards[Day].IsPicked;

	// Новый день начинается с 6 утра
	private DateTime TimeNow => DateTime.Now - new TimeSpan(6, 0, 0);

	private int Day {
		get {
			var format        = "O";
			var startWeekTime = TimeNow;

			// Если есть запись о начале недели
			if (PlayerPrefs.HasKey(StartWeekKey)) {
				// print("Есть запись о первом дне");
				// Если есть запись о последнем дне входа в игру
				if (PlayerPrefs.HasKey(LastDayKey)) {
					// print("Есть запись о последнем входе");
					var lastDayStr       = PlayerPrefs.GetString(LastDayKey);
					var lastDay          = DateTime.ParseExact(lastDayStr, format, CultureInfo.InvariantCulture);
					var daysFromLastPlay = (TimeNow - lastDay).Days;
					// print($"Текущее время: {TimeNow}");
					// print($"Последний вход: {lastDay}");
					// print($"С последнего входа прошло {daysFromLastPlay} д.");

					// Если с последнего входа прошел больше или меньше чем 1 день, то сбрасываем все
					if (daysFromLastPlay > 1 || daysFromLastPlay < 0 || TimeNow < lastDay) {
						ResetWeek();
						return 0;
					}
					else {
						var time = TimeNow - TimeNow.TimeOfDay;
						// print($"Установка времени последнего входа на {time}");
						var strTime = time.ToString(format);
						PlayerPrefs.SetString(LastDayKey, strTime);
						PlayerPrefs.Save();
					}
				}
				// Иначе обнуляем дату
				else {
					ResetWeek();
					return 0;
				}

				var startWeekTimeStr = PlayerPrefs.GetString(StartWeekKey);
				startWeekTime = DateTime.ParseExact(startWeekTimeStr, format, CultureInfo.InvariantCulture);
				// print($"Неделя началась {startWeekTime}");
			}
			// Если нет записи о начале недели
			else {
				// print("Нет записи о начале недели. Сброс прогресса");
				ResetWeek();
				return 0;
			}

			var days = (TimeNow - startWeekTime).Days;
			if (days < 7) return days;
			ResetWeek();
			return 0;
		}
	}

	/*private bool[] PickedRewards {
		get {
			if (PlayerPrefs.HasKey(PickedRewardsKey)) {
				var pickedRewardsStr      = PlayerPrefs.GetString(PickedRewardsKey);
				var pickedRewardsStrArray = pickedRewardsStr.Split(',');
				// print($"Загружены собранные награды: {pickedRewardsStr}");
				var pickedRewards = Array.ConvertAll(pickedRewardsStrArray, bool.Parse);
				return pickedRewards;
			}
			else {
				var pickedRewardsStr = string.Join(",", new bool[7]);
				// print($"Creating picked rewards save: {pickedRewardsStr}");
				PlayerPrefs.SetString(PickedRewardsKey, pickedRewardsStr);
				PlayerPrefs.Save();
				return new bool[7];
			}
		}
		set {
			var pickedRewardsStr = string.Join(",", value);
			// print($"Set picked rewards save: {pickedRewardsStr}");
			PlayerPrefs.SetString(PickedRewardsKey, pickedRewardsStr);
			PlayerPrefs.Save();
		}
	}*/


	public void Initialize() {
		if (WeekRewards.Count > 0) return;
		UpdateFreeSkin();
		LoadWeek();
	}


	protected override void Show() {
		Initialize();
		CreateRewards();
		base.Show();

		var sequence = DOTween.Sequence();
		sequence.Append(FadeScreenAnim.Show());
		sequence.Append(RewardPanelAnim.Show());
		sequence.Insert(0.1f, RewardButtonsAnim.Show());
		sequence.Insert(0.1f, RewardLabelAnim.Show());
		sequence.OnComplete(delegate { OnShowMenu.SafeInvoke(); });

		AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.daily_reward_menu_open);
	}


	protected override void StartHide(Action callback) {
		base.StartHide(callback);

		var sequence = DOTween.Sequence();
		sequence.Append(RewardPanelAnim.Hide());
		sequence.Append(FadeScreenAnim.Hide());
		sequence.Insert(0.1f, RewardLabelAnim.Hide());
		sequence.Insert(0.1f, RewardButtonsAnim.Hide());
		sequence.OnComplete(() => {
			FinishHide(callback);
			OnHideMenu.SafeInvoke();
			DestroyRewards();
			PickButton.interactable = true;
		});
	}


	private void CreateRewards() {
		var currentDay = Day;

		for (int day = 0; day < 7; day++) {
			var canBePicked = day <= currentDay && !WeekRewards[day].IsPicked;
			_rewardCards.Add(CreateReward(day, WeekRewards[day].IsPicked, canBePicked));
		}

		var canPick = !WeekRewards[currentDay].IsPicked;
		PickButton.gameObject.SetActive(canPick);
		BackButton.gameObject.SetActive(!canPick);
	}


	private void DestroyRewards() {
		_rewardCards.ForEach(r => Destroy(r.gameObject));
		_rewardCards.Clear();
	}


	private RewardCard CreateReward(int day, bool isPicked, bool canBePicked) {
		var rewardObject = Instantiate(RewardPrefab, RewardContainer);
		var rewardCard   = rewardObject.GetComponent<RewardCard>();
		var savedReward  = WeekRewards[day];
		var reward       = GetDailyReward(day, savedReward);
		// print($"Day {day}. Saved reward ({savedReward.RewardType}, {savedReward.Value}, {savedReward.IsPicked}); Reward ({reward.RewardType}, {reward.Value})");

		if (isPicked) rewardCard.CompleteDay();

		rewardCard.SetData(day + 1, reward);
		rewardCard.CanBePicked(canBePicked);

		return rewardCard;
	}


	private Reward GetDailyReward(int day) {
		var reward = Rewards[day];
		if (reward.RewardType == RewardType.FreeSkin) {
			var freeSkin = GetFreeSkin();
			// print($"Free skin: {(CharacterKinds) freeSkin}");
			if (freeSkin < 0) {
				reward.RewardType = RewardType.Coins;
				reward.Icon       = CoinsIcon;
			}
			else {
				reward.Icon  = CharacterSkins[freeSkin];
				reward.Value = freeSkin;
			}
		}

		if (reward.RewardType == RewardType.Coins) {
			reward.Icon = reward.Value < 300 ? CoinsIcon : Coins2Icon;
		}

		return reward;
	}


	private Reward GetDailyReward(int day, SavedReward savedReward) {
		var reward = Rewards[day];
		var skins  = GetFreeSkinList();
		if (savedReward.RewardType == RewardType.FreeSkin) {
			if (!savedReward.IsPicked && !skins.Contains((CharacterKinds) savedReward.Value)) {
				savedReward.RewardType = RewardType.Coins;
				savedReward.Value      = reward.Value;
				SaveWeek();
			}
			else {
				reward.Icon = CharacterSkins[savedReward.Value];
			}
		}

		reward.RewardType = savedReward.RewardType;
		reward.Value      = savedReward.Value;

		if (reward.RewardType == RewardType.Coins) {
			reward.Icon = reward.Value < 300 ? CoinsIcon : Coins2Icon;
		}

		return reward;
	}


	private int GetFreeSkin() {
		if (_freeSkinList.Count <= 0) return -1;

		var skin = (int) _freeSkinList[0];
		_freeSkinList.RemoveAt(0);
		return skin;
	}


	private List<CharacterKinds> GetFreeSkinList() {
		var skins = new List<CharacterKinds>();

		var characters = Enum.GetValues(typeof(CharacterKinds));
		foreach (var character in characters) {
			var characterKind = (CharacterKinds) character;
			if (characterKind == CharacterKinds.Base) continue;

			var key = CharactersData.CHARACTER_KEY + " : " + characterKind;
			if (!PlayerPrefs.HasKey(key) || PlayerPrefs.GetInt(key) == 0) {
				skins.Add(characterKind);
			}
		}

		return skins;
	}


	private void UpdateFreeSkin() {
		_freeSkinList = GetFreeSkinList();
	}


	private bool PickReward() {
		var currentDay = Day;

		for (int day = 0; day <= currentDay; day++) {
			if (WeekRewards[day].IsPicked) continue;

			var savedReward = WeekRewards[day];
			switch (savedReward.RewardType) {
				case RewardType.Coins: {
					Wallet.Instance.AddCoins(savedReward.Value, Wallet.WalletMode.Global);
					break;
				}
				case RewardType.FreeSkin: {
					var skin     = (CharacterKinds)  savedReward.Value;
					var skinData = CharactersData.GetCharacterData(skin);

					print($"Pick skin reward. Value: {savedReward.Value}-{skin}");

					skinData.Bought = true;
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}

			_rewardCards[day].CompleteDay();
			WeekRewards[day].IsPicked = true;
		}

		var reward = GetDailyReward(currentDay, WeekRewards[currentDay]);
		PickRewardPanel.SetData(reward);
		SaveWeek();

		return true;
	}


	private void ResetWeek() {
		// print("Сброс прогресса");
		var format  = "O";
		var strTime = (TimeNow - TimeNow.TimeOfDay).ToString(format);
		PlayerPrefs.SetString(LastDayKey,   strTime);
		PlayerPrefs.SetString(StartWeekKey, strTime);
		PlayerPrefs.Save();

		WeekRewards.Clear();
		for (int i = 0; i < 7; i++) {
			var reward      = GetDailyReward(i);
			var savedReward = new SavedReward(reward.RewardType, reward.Value);
			WeekRewards.Add(savedReward);
		}

		SaveWeek();
	}


	private void LoadWeek() {
		if (SaveSystem.Exists(SaveFileName)) {
			WeekRewards = SaveSystem.BinaryLoad<List<SavedReward>>(SaveFileName);
		}
		else {
			ResetWeek();
		}
	}


	private void SaveWeek() {
		// print("Save week");
		SaveSystem.BinarySave<List<SavedReward>>(SaveFileName, WeekRewards);
		// print("Saved");
	}


	#region Events

	public void OnBackButtonClick() {
		if (PickReward()) {
			PickButton.interactable = false;
			PickedRewardPanelAnim.Show();
			_audio.PlaySound(Sounds.WinQuest);
			// DOVirtual.DelayedCall(3f, () => _menuController.OpenMenu(MenuKinds.MainMenu));
		}
		else {
			_audio.PlaySound(Sounds.Tap);
			_menuController.OpenMenu(MenuKinds.MainMenu);
		}
	}


	public void OnClosePickedRewardPanel() {
		PickedRewardPanelAnim.Hide();
		_audio.PlaySound(Sounds.Tap);
		_menuController.OpenMenu(MenuKinds.MainMenu);
	}

	#endregion


	[Serializable]
	public struct Reward {
		public RewardType RewardType;
		public int        Value;
		public Sprite     Icon;
	}

	[Serializable]
	public class SavedReward {
		public RewardType RewardType;
		public int        Value;
		public bool       IsPicked;


		public SavedReward(RewardType rewardType, int value) {
			RewardType = rewardType;
			Value      = value;
		}
	}

	public enum RewardType {
		Coins,
		FreeSkin
	}
}