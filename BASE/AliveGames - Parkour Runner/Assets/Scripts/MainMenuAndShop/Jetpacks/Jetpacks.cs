using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainMenuAndShop.Jetpacks {
	[CreateAssetMenu(fileName = "Jetpacks Shop Configuration", menuName = "Jetpacks Data", order = 52)]
	public class Jetpacks : ScriptableObject {
		public static            List<Jetpack> JetpackList => Instance._jetpacks;
		[SerializeField] private List<Jetpack> _jetpacks = new List<Jetpack>();

		public const string JETPACK_KEY = "Jetpack";
		public static JetpacksType ActiveJetpackType {
			get => (JetpacksType) PlayerPrefs.GetInt(JETPACK_KEY, 0);
			set => PlayerPrefs.SetInt(JETPACK_KEY, (int) value);
		}


		#region Load From Resources

		private static Jetpacks Instance {
			get {
				if (_instance != null) return _instance;

				_instance = Resources.Load("Character/Jetpacks Shop Configuration") as Jetpacks;
				if (_instance == null) {
					throw new FileNotFoundException("Can't found 'Jetpacks' file");
				}
				return _instance;
			}
		}

		private static Jetpacks _instance;

		#endregion


		public static Jetpack GetActiveJetpack() {
			return GetJetpackData(ActiveJetpackType);
		}


		public static Jetpack GetJetpackData(JetpacksType jetpackType) {
			return Instance._jetpacks.FirstOrDefault(h => h.JetpackType == jetpackType);
		}


		[Serializable]
		public class Jetpack {
			public JetpacksType JetpackType;
			public GameObject   JetpackPrefab;
			public Sprite       JetpackIcon;
			public int          Price;
			public float        BoostTime;

			public string BoughtKey => JETPACK_KEY + JetpackType;

			public bool Bought {
				get {
					if (JetpackType                         == JetpacksType.NoJetpack) return true;
					return PlayerPrefs.GetInt(BoughtKey, 0) == 1;
				}

				set {
					PlayerPrefs.SetInt(BoughtKey, value ? 1 : 0);
					PlayerPrefs.Save();
				}
			}
		}

		public enum JetpacksType {
			NoJetpack,
			Jetpack1,
			Jetpack2,
			Jetpack3,
			Jetpack4,
			Jetpack5,
			Jetpack6,
		}
	}
}