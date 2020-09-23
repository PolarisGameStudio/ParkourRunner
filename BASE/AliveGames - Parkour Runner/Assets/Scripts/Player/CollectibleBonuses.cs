using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CollectibleBonuses {
	public const string BONUSES_KEY       = "CollectibleBonuses";
	public const char   BONUSES_SEPARATOR = ',';

	/// <summary>
	/// Key - Тип бонуса, Value - кол-во
	/// </summary>
	public static Dictionary<BonusName, int> Bonuses;


	public static int GetBonusesAmount(BonusName type) {
		CheckLoad();
		return Bonuses[type];
	}


	public static bool UseBonus(BonusName type, int amount) {
		CheckLoad();
		if (Bonuses[type] < amount) return false;

		Bonuses[type] -= amount;
		Save();
		return true;
	}


	public static void AddBonus(BonusName type, int amount) {
		CheckLoad();

		Bonuses[type] += amount;
		Save();
	}


	private static void CheckLoad() {
		if (Bonuses == null) Load();
	}


	private static void Load() {
		if (PlayerPrefs.HasKey(BONUSES_KEY)) {
			var bonusesString = PlayerPrefs.GetString(BONUSES_KEY);
			var bonusesArray  = bonusesString.Split(BONUSES_SEPARATOR);

			Bonuses = new Dictionary<BonusName, int>();
			for (var i = 0; i < bonusesArray.Length; i++) {
				Bonuses.Add((BonusName) i, int.Parse(bonusesArray[i]));
			}
		}
		else {
			Bonuses = new Dictionary<BonusName, int>();
			foreach (var value in Enum.GetValues(typeof(BonusName))) {
				Bonuses.Add((BonusName) value, 0);
			}
			Save();
		}
	}


	private static void Save() {
		var bonusesString = string.Join(BONUSES_SEPARATOR.ToString(), Bonuses.Values);
		PlayerPrefs.SetString(BONUSES_KEY, bonusesString);
		PlayerPrefs.Save();
	}
}