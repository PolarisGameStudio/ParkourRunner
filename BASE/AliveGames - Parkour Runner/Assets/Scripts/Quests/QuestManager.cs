using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

public class QuestManager : MonoBehaviour {
	public const string JUMP_PROGRESS_KEY = "QUEST_JUMP";

	private const string ACTIVE_QUESTS_KEY    = "QUEST";
	private const string COMPLETED_QUESTS_KEY = "COMPLETED_QUESTS";
	private const string DATE_YEAR_KEY        = "DATE_YEAR";
	private const string DATE_MONTH_KEY       = "DATE_MONTH";
	private const string DATE_DAY_KEY         = "DATE_DAY";
	private const string DATE_HOUR_KEY        = "DATE_HOUR";
	private const string DATE_MINUTE_KEY      = "DATE_MINUTE";
	private const string DATE_SECOND_KEY      = "DATE_SECOND";

	private const int WATCH_AD_QUEST_ID     = 4;
	private const int JUMP_QUEST_ID         = 3;
	public const  int MULTIPLAYER_QUEST_ID  = 8;
	private const int COMPLETE_ALL_QUEST_ID = 7;

	public static QuestManager Instance;

	public event Action OnUpdateActiveQuestsEvent;

	[SerializeField] private int             _questsHoursPeriod;
	[SerializeField] private int             _activeQuestsCount;
	[SerializeField] private List<QuestData> _quests;

	public List<QuestData> ActiveQuests    { get; private set; }
	public List<int>       CompletedQuests { get; private set; }

	public double LeftSeconds { get; set; }


	private void Awake() {
		if (Instance == null) {
			Instance = this;

			this.ActiveQuests = new List<QuestData>();

			StartCoroutine(UpdateStateProcess());

			DontDestroyOnLoad(this);
		}
		else
			Destroy(this.gameObject);
	}


	#region Jump Quest Interface

	public void CompleteQuest(int id) {
		print("Complete Quest " + id);
		for (int i = 0; i < this.ActiveQuests.Count; i++) {
			if (this.ActiveQuests[i].ID == id) {
				// this.ActiveQuests.RemoveAt(i);
				CompletedQuests.Add(id);
				SaveActiveQuests();
				return;
			}
		}
	}


	private void CheckJumpKeys() {
		if (!PlayerPrefs.HasKey(JUMP_PROGRESS_KEY)) {
			PlayerPrefs.SetInt(JUMP_PROGRESS_KEY, 0);
			PlayerPrefs.Save();
		}
	}


	public void AddJumpProgressTick() {
		CheckJumpKeys();

		PlayerPrefs.SetInt(JUMP_PROGRESS_KEY, PlayerPrefs.GetInt(JUMP_PROGRESS_KEY) + 1);
		PlayerPrefs.Save();
	}


	public int GetJumpProgress() {
		CheckJumpKeys();
		return PlayerPrefs.GetInt(JUMP_PROGRESS_KEY);
	}


	public void ClearJumpProgress() {
		CheckJumpKeys();

		PlayerPrefs.SetInt(JUMP_PROGRESS_KEY, 0);
		PlayerPrefs.Save();
	}

	#endregion


	private IEnumerator UpdateStateProcess() {
		while (true) {
			DateTime date = DateTime.Now;

			// Check on first launch
			if (!PlayerPrefs.HasKey(DATE_YEAR_KEY)) {
				SelectRandomQuests();
				SaveActiveQuests();
				SaveDate(date);

				OnUpdateActiveQuestsEvent.SafeInvoke();
			}
			else {
				DateTime savedDate = new DateTime(PlayerPrefs.GetInt(DATE_YEAR_KEY), PlayerPrefs.GetInt(DATE_MONTH_KEY), PlayerPrefs.GetInt(DATE_DAY_KEY),
												PlayerPrefs.GetInt(DATE_HOUR_KEY), PlayerPrefs.GetInt(DATE_MINUTE_KEY), PlayerPrefs.GetInt(DATE_SECOND_KEY));

				double deltaTime = (date - savedDate).TotalHours;

				this.LeftSeconds = _questsHoursPeriod * 60 * 60 - (date - savedDate).TotalSeconds;

				if (this.LeftSeconds <= 0)
					this.LeftSeconds = 0;

				if (deltaTime >= _questsHoursPeriod) {
					SelectRandomQuests();
					SaveActiveQuests();
					SaveDate(date);

					OnUpdateActiveQuestsEvent.SafeInvoke();
				}
				else if (this.ActiveQuests.Count == 0) {
					LoadActiveQuests();

					OnUpdateActiveQuestsEvent.SafeInvoke();
				}
			}

			yield return new WaitForSeconds(1f);
		}
	}


	private void SaveDate(DateTime date) {
		PlayerPrefs.SetInt(DATE_YEAR_KEY,   date.Year);
		PlayerPrefs.SetInt(DATE_MONTH_KEY,  date.Month);
		PlayerPrefs.SetInt(DATE_DAY_KEY,    date.Day);
		PlayerPrefs.SetInt(DATE_HOUR_KEY,   date.Hour);
		PlayerPrefs.SetInt(DATE_MINUTE_KEY, date.Minute);
		PlayerPrefs.SetInt(DATE_SECOND_KEY, date.Second);

		PlayerPrefs.Save();
	}


	private void SelectRandomQuests() {
		this.ActiveQuests.Clear();
		CompletedQuests = new List<int>();

		List<QuestData> list = _quests.Where(x => x.Enable && x.ID != COMPLETE_ALL_QUEST_ID && x.ID != WATCH_AD_QUEST_ID).ToList();

		for (int i = 0; i < _activeQuestsCount; i++) {
			QuestData item = list[UnityEngine.Random.Range(0, list.Count)];
			ActiveQuests.Add(item);

			//Jump quest
			if (item.ID == JUMP_QUEST_ID)
				ClearJumpProgress();

			list.Remove(item);
			// list = list.Where(x => x.Enable && x.ID != item.ID).ToList();
		}

		var completeAllQuest = _quests.FirstOrDefault(x => x.Enable && x.ID == COMPLETE_ALL_QUEST_ID);
		if (completeAllQuest) ActiveQuests.Add(completeAllQuest);

		var watchAdQuest = _quests.FirstOrDefault(x => x.Enable && x.ID == WATCH_AD_QUEST_ID);
		if (watchAdQuest) ActiveQuests.Add(watchAdQuest);
	}


	private void SaveActiveQuests() {
		if (this.ActiveQuests == null) {
			Debug.LogError("Quests list was not initialized correctly");
		}
		else {
			for (int i = 0; i < _quests.Count; i++) {
				string key = ACTIVE_QUESTS_KEY + _quests[i].ID;
				PlayerPrefs.SetInt(key, this.ActiveQuests.Contains(_quests[i]) ? 1 : 0);
			}
			PlayerPrefs.SetString(COMPLETED_QUESTS_KEY, string.Join(",", CompletedQuests));

			PlayerPrefs.Save();
		}
	}


	private void LoadActiveQuests() {
		print("Loading quests");
		this.ActiveQuests.Clear();

		for (int i = 0; i < _quests.Count; i++) {
			string key = ACTIVE_QUESTS_KEY + _quests[i].ID;

			if (!PlayerPrefs.HasKey(key)) {
				Debug.LogError("Couldn't find quest key. Select random items.");
				SelectRandomQuests();
				SaveActiveQuests();
				SaveDate(DateTime.Now);
				break;
			}
			else {
				int isActive = PlayerPrefs.GetInt(key);
				if (isActive == 1)
					this.ActiveQuests.Add(_quests[i]);
			}
		}

		if (PlayerPrefs.HasKey(COMPLETED_QUESTS_KEY)) {
			var completedQuestsStr = PlayerPrefs.GetString(COMPLETED_QUESTS_KEY);
			if (string.IsNullOrEmpty(completedQuestsStr)) CompletedQuests = new List<int>();
			else {
				var completedQuestsStrArr = completedQuestsStr.Split(',');
				CompletedQuests = Array.ConvertAll(completedQuestsStrArr, int.Parse).ToList();
				print($"Loaded Completed Quests: {CompletedQuests.Count}");
			}
		}
		else {
			print("new CompletedQuests");
			CompletedQuests = new List<int>();
		}
	}
}