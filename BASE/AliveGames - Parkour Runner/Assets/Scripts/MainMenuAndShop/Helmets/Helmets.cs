using System;
using System.Collections.Generic;
using System.IO;
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


		#region Load From Resources

		private static Helmets Instance {
			get {
				if (_instance != null) return _instance;

				_instance = Resources.Load("Character/Helmets Configuration") as Helmets;
				if (_instance == null) {
					throw new FileNotFoundException("Can't found 'Jetpacks' file");
				}
				return _instance;
			}
		}

		private static Helmets _instance;

		#endregion


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


		public static Data GetHelmetData(HelmetsType helmetType) {
			return Instance._helmets.FirstOrDefault(h => h.HelmetType == helmetType);
		}


		public static List<HelmetsType> GetFreeHelmetsList() {
			var helmets      = new List<HelmetsType>();
			var allHelmets = Enum.GetValues(typeof(HelmetsType)).OfType<object>().Select(c => (HelmetsType) c).ToList();

			foreach (var helmet in allHelmets) {
				if(helmet == HelmetsType.NoHelmet) continue;

				var key = HELMET_KEY + helmet;
				if (!PlayerPrefs.HasKey(key) || PlayerPrefs.GetInt(key) == 0) {
					helmets.Add(helmet);
				}
			}

			return helmets;
		}


		public enum HelmetsType {
			NoHelmet,
			Helmet1,
			Helmet2,
			Helmet3,
		}
	}
}