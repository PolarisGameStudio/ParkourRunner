using System;
using System.Collections.Generic;
using System.Linq;
using AppodealAds.Unity.Api;
using AppsFlyerSDK;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace Managers {
	public class AppsFlyerManager : MonoBehaviour, IAppsFlyerValidateReceipt, IAppsFlyerConversionData {
		[SerializeField] private string Key   = "scA4snPgJk3sUURRNj6pjX";
		[SerializeField] private string appID = "6FFSWJC9HG.com.AliveGames.ParkourRunnerTest";
		[SerializeField] private string licenseKey =
			"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxGeVL9X5loSTvOX/7E0cdU0PChD2JpNfElDG3EXly2lcerdhGrKzLHKVUtMit7QQg2Opa8/OYsTKRuS9M154neaJDJrbMbV/zJXtUQaaz+Gr6McW4QnAYEODLjXRlJ8odzeJznvpCvxdvWV3g4DC4BL53IgYhv69Xs5xZ8j4eTvgeOJWtuslZjer9h3hvnyD8/HdFlCJ9/Y+ipqqwttZzzsLyZhcNvVWQB6jhwaLP4axRYZQQMKoISsk3CVZNBPuB9hhvgIWrCh+vivVMRxQW6TlpgxYTFRBUuxz1U9+vMP0IbSTJT2a0xnzUPa5QWKA8hQUXnnyS3mP3H17rsSphQIDAQAB";

		private static AppsFlyerManager  Instance;
		private        PurchaseEventArgs ValidatingPurchase;


		#region AppsFlyer logic

		private void Start() {
			if (Instance) {
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);

			Initialize();
		}


		private void Initialize() {
			AppsFlyer.initSDK(Key, appID, this);
			AppsFlyer.startSDK();

			SendBaseEvent(BaseEvents.game_start);
		}


		public static void ValidatePurchase(PurchaseEventArgs args) {
			string prodID   = args.purchasedProduct.definition.id;
			string price    = args.purchasedProduct.metadata.localizedPrice.ToString();
			string currency = args.purchasedProduct.metadata.isoCurrencyCode;

			string receipt     = args.purchasedProduct.receipt;
			var    recptToJSON = (Dictionary<string, object>) AFMiniJSON.Json.Deserialize(receipt);
			// var    transactionID = (string) recptToJSON["TransactionID"];
			var transactionID = args.purchasedProduct.transactionID;

			print($"---Validating purchase {prodID}---");

#if UNITY_IOS
			AppsFlyeriOS.validateAndSendInAppPurchase(prodID, price, currency, transactionID, null, Instance);
#elif UNITY_ANDROID
			var wrapper = (Dictionary<string, object>) MiniJson.JsonDecode (receipt);
			if (null == wrapper) {
				throw new InvalidReceiptDataException ();
			}

			var store   = (string) wrapper ["Store"];
			var payload = (string) wrapper ["Payload"];

			var details = (Dictionary<string, object>) MiniJson.JsonDecode (payload);
			var json    = (string) details ["json"];      // This is the INAPP_PURCHASE_DATA information
			var sig     = (string) details ["signature"]; // This is the INAPP_DATA_SIGNATURE information

			AppsFlyerAndroid.validateAndSendInAppPurchase(Instance.licenseKey, sig, json, price, currency, null,
														Instance);
#endif

			Instance.ValidatingPurchase = args;
		}


		private static void SendEvent(string eventName, Dictionary<string, string> values) {
			string values_s = string.Join(";", values.Select(x => x.Key + "=" + x.Value).ToArray());
			AppsFlyer.AFLog("Send event", $"{eventName}: {values_s}");
			AppsFlyer.sendEvent(eventName, values);
		}


		public static void SendCustomEvent(string customEvent, Dictionary<string, string> parameters) {
			SendEvent(customEvent, parameters);
		}


		public static void SendBaseEvent(BaseEvents baseEvent) {
			SendEvent(baseEvent.ToString(), new Dictionary<string, string>());
		}


		public static void SendExtendedEvent(ExtendedEvents extendedEvent, Dictionary<string, string> values) {
			SendEvent(extendedEvent.ToString(), values);
		}

		#endregion


		#region Events

		public static void RestartLevel(float distanceRun, bool died, int level, int earnedCoins) {
			var values = new Dictionary<string, string> {
				{"level", level.ToString()},
				{"distance_run", distanceRun.ToString()},
				{"restart", died ? "by died" : "from pause menu"},
				{"earned_coins", earnedCoins.ToString()}
			};

			SendExtendedEvent(ExtendedEvents.restart_level, values);
		}


		public static void RestartEndless(float distanceRun, bool died, int earnedCoins) {
			var values = new Dictionary<string, string> {
				{"distance_run", distanceRun.ToString()},
				{"restart", died ? "by died" : "from pause menu"},
				{"earned_coins", earnedCoins.ToString()}
			};

			SendExtendedEvent(ExtendedEvents.restart_endless_level, values);
		}


		public static void LevelComplete(int level, int earnedCoins) {
			var values = new Dictionary<string, string> {
				{"level", level.ToString()},
				{"earned_coins", earnedCoins.ToString()},
			};

			SendExtendedEvent(ExtendedEvents.level_complete, values);
		}


		public static void StartLevel(int level) {
			var values = new Dictionary<string, string> {
				{"level", level.ToString()},
			};

			SendExtendedEvent(ExtendedEvents.start_level, values);
		}


		/// <summary>
		/// Мастер запускает игру
		/// </summary>
		public static void StartMultiplayer(IEnumerable<string> players, int bet) {
			var values = new Dictionary<string, string> {
				{"players", string.Join(",", players)},
				{"bet", bet.ToString()},
			};

			SendExtendedEvent(ExtendedEvents.multiplayer_starting_game, values);
		}


		/// <summary>
		/// Когда у игрока начинается забег
		/// </summary>
		public static void MultiplayerIsRunning(IEnumerable<string> players, int bet) {
			var values = new Dictionary<string, string> {
				{"players", string.Join(",", players)},
				{"bet", bet.ToString()},
			};

			SendExtendedEvent(ExtendedEvents.multiplayer_game_is_started, values);
		}


		/// <summary>
		/// Когда игрок финиширует
		/// </summary>
		public static void MultiplayerIsFinished(IEnumerable<string> players, int bet) {
			var values = new Dictionary<string, string> {
				{"players", string.Join(",", players)},
				{"bet", bet.ToString()},
			};

			SendExtendedEvent(ExtendedEvents.multiplayer_game_is_finished, values);
		}


		/// <summary>
		/// Когда игрок финиширует
		/// </summary>
		public static void LeaveMultiplayer(string nickname, int players) {
			var values = new Dictionary<string, string> {
				{"nickname", nickname},
				{"other players count", players.ToString()},
			};

			SendExtendedEvent(ExtendedEvents.multiplayer_leave, values);
		}


		/// <summary>
		/// Покупка шлема
		/// </summary>
		public static void ShopBuyHelmet(string helmet) {
			var values = new Dictionary<string, string> {
				{"helmet", helmet},
			};

			SendExtendedEvent(ExtendedEvents.shop_buy_helmet, values);
		}


		/// <summary>
		/// Покупка скина
		/// </summary>
		public static void ShopBuySkin(string skin) {
			var values = new Dictionary<string, string> {
				{"skin", skin},
			};

			SendExtendedEvent(ExtendedEvents.shop_buy_skin, values);
		}


		/// <summary>
		/// Покупка бонуса
		/// </summary>
		public static void ShopBuyBonus(string bonus, int level) {
			var values = new Dictionary<string, string> {
				{"bonus", bonus},
				{"bonus level", level.ToString()},
			};

			SendExtendedEvent(ExtendedEvents.shop_buy_bonus, values);
		}

		#endregion


		#region Interface Callbacks

		public void didFinishValidateReceipt(string result) {
			if (ValidatingPurchase == null) return;
			AppsFlyer.AFLog("ValidatePurchase",
							$"Purchase {ValidatingPurchase.purchasedProduct.definition.id} validated");
			InAppManager.Instance.PurchaseValidated(ValidatingPurchase);
			ValidatingPurchase = null;
		}


		public void didFinishValidateReceiptWithError(string error) {
			if (ValidatingPurchase == null) return;
			AppsFlyer.AFLog("ValidatePurchase",
							$"Purchase {ValidatingPurchase.purchasedProduct.definition.id} is not validated: {error}");
			ValidatingPurchase = null;
		}


		public void onConversionDataSuccess(string conversionData) {
			AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
			Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
			foreach (var attr in conversionDataDictionary) {
				Debug.Log("attribute: " + attr.Key + " = " + attr.Value);
			}
			/*Appodeal.setSegmentFilter("campaign", conversionDataDictionary["campaign"].ToString());
			Appodeal.setSegmentFilter("adset", conversionDataDictionary["adset"].ToString());*/
		}


		public void onConversionDataFail(string error) {
			AppsFlyer.AFLog("onConversionDataFail", error);
		}


		public void onAppOpenAttribution(string attributionData) {
			AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
			Dictionary<string, object> attributionDataDictionary =
				AppsFlyer.CallbackStringToDictionary(attributionData);
			// add direct deeplink logic here
		}


		public void onAppOpenAttributionFailure(string error) {
			AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
		}

		#endregion


		/// <summary>
		/// События без дополнительных параметров
		/// </summary>
		public enum BaseEvents {
			game_start,               // Запуск игры
			main_menu_open,           // Открывается главное меню
			shop_menu_open,           // Открывается магазин
			daily_reward_menu_open,
			multiplayer_menu_open,    // Вход в список комнат
			multiplayer_room_created, // Создание комнаты
			show_pause,               // Открытие меню паузы во время забега
			close_pause,              // Закрытие меню паузы во время забега

			start_tutorial,    // Старт обучения
			restart_tutorial,  // Перезапуск обучения
			leave_tutorial,    // Выход из обучения
			complete_tutorial, // Обучение пройдено

			start_endless, // Запуск режима с бесконечным уровнем

			rewarded_video_shown, // Запуск Rewarded Ad
			interstitial_shown,   // Запуск межстраничной рекламы
			banner_shown,         // Отображение баннера
		}
		/// <summary>
		/// События с доп параметрами
		/// </summary>
		public enum ExtendedEvents {
			start_level,			// Запуск уровня
			level_complete,			// Уровень пройден
			restart_level,          // Перезапуск уровня
			restart_endless_level,  // Перезапуск режима бесконечной игры

			multiplayer_starting_game, 		// Кнопка "старт" в меню мультиплеера
			multiplayer_game_is_started,	// Вызывается когда игроки начинают бежать
			multiplayer_game_is_finished,	// Игроки добежали до финиша
			multiplayer_leave,				// Игрок покинул игру до финиша

			shop_buy_helmet,	// Покупка шлема
			shop_buy_skin,		// Покупка скина
			shop_buy_bonus,		// Покупка бонуса
		}
	}
}