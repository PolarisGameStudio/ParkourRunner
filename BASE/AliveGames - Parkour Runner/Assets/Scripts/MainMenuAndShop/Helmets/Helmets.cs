using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MainMenuAndShop.Helmets {
	[CreateAssetMenu(fileName = "Helmets Configuration", menuName = "Helmets Data", order = 51)]
	public class Helmets : ScriptableObject {
		public const string HELMET_KEY = "Helmet";
		public static HelmetsType CurrentHelmetType {
			get => (HelmetsType) PlayerPrefs.GetInt(HELMET_KEY, 0);
			set => PlayerPrefs.SetInt(HELMET_KEY, (int) value);
		}

		[SerializeField] private List<Data> _helmets = new List<Data>();

		[Serializable]
		public class Data {
			public HelmetsType HelmetType;
			public int         Price;

			public string BoughtKey => HELMET_KEY + HelmetType;

			public bool Bought {
				get {
					if (HelmetType                          == HelmetsType.NoHelmet) return true;
					return PlayerPrefs.GetInt(BoughtKey, 0) == 1;
				}

				set {
					PlayerPrefs.SetInt(BoughtKey, value ? 1 : 0);
					PlayerPrefs.Save();
				}
			}
		}


		public Data GetHelmetData(HelmetsType helmetType) {
			return _helmets.FirstOrDefault(h => h.HelmetType == helmetType);
		}


		public enum HelmetsType {
			NoHelmet,
			Helmet1,
			Helmet2,
			Helmet3,
		}
	}
}