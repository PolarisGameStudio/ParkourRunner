using System;
using System.Collections;
using System.Globalization;
using MainMenuAndShop.Helmets;
using MainMenuAndShop.Jetpacks;
using UnityEngine;
using UnityEngine.UI;

public class StarterPack : MonoBehaviour {
	[SerializeField] private GameObject      StarterPackButton;
	[SerializeField] private GameObject      StarterPackWindow;
	[SerializeField] private ShopDonatsPanel ShopDonatsPanel;
	[SerializeField] private Text            TimerText;

	private const string StarterPackTimerKey = "StarterPackTimer";
	public const  string SkinKey             = "Character : Character2";
	public static string HelmetBoughtKey => "Helmet" + HelmetType;

	public const CharacterKinds        SkinKind    = CharacterKinds.Character2;
	public const Jetpacks.JetpacksType JetpackType = Jetpacks.JetpacksType.Jetpack1;
	public const Helmets.HelmetsType   HelmetType  = Helmets.HelmetsType.Helmet1;

	private readonly WaitForSeconds _oneSecond = new WaitForSeconds(1f);
	private readonly TimeSpan       _maxTimer  = new TimeSpan(7, 0, 0, 0);
	private          DateTime       TimeLeft   = DateTime.Now;

	public static bool CanBuy => PlayerPrefs.GetInt(SkinKey, 0) == 0 || !Jetpacks.GetJetpackData(JetpackType).Bought;


	private void Start() {
		CharacterSelection.OnSelectCharacter += OnSelectCharacter;
		JetpackSelection.OnSelectJetpack     += OnSelectJetpack;

		StartCoroutine(StartTimer());
	}


	private void OnEnable() {
		UpdatePackButton();
	}


	private IEnumerator StartTimer() {
		var format = "O";
		TimeLeft = DateTime.Now + _maxTimer;

		if (PlayerPrefs.HasKey(StarterPackTimerKey)) {
			var loadTimeStr = PlayerPrefs.GetString(StarterPackTimerKey);
			TimeLeft = DateTime.ParseExact(loadTimeStr, format, CultureInfo.InvariantCulture);
			if (TimeLeft > DateTime.Now + _maxTimer) {
				TimeLeft = DateTime.Now + _maxTimer;
				ResetTimer();
			}
		}
		else {
			ResetTimer();
		}

		while (true) {
			var timeLeft = TimeLeft - DateTime.Now;
			if (DateTime.Now > TimeLeft) {
				TimeLeft = DateTime.Now + _maxTimer;
				ResetTimer();
				continue;
			}
			var timerStr = $"{(int) timeLeft.TotalHours}:{timeLeft.Minutes}:{timeLeft.Seconds}";
			TimerText.text = timerStr;
			yield return _oneSecond;
		}
	}


	private void ResetTimer() {
		var format  = "O";
		var strTime = (DateTime.Now + _maxTimer).ToString(format);
		PlayerPrefs.SetString(StarterPackTimerKey, strTime);
		PlayerPrefs.Save();
	}


	private void OnDestroy() {
		CharacterSelection.OnSelectCharacter -= OnSelectCharacter;
		JetpackSelection.OnSelectJetpack     -= OnSelectJetpack;
	}


	public void Show() {
		StarterPackWindow.SetActive(true);
		InAppManager.OnBuySuccess += OnPurchase;
	}


	public void Hide() {
		StarterPackWindow.SetActive(false);
		InAppManager.OnBuySuccess -= OnPurchase;
	}


	private void OnPurchase(DonatShopData.DonatKinds kinds) {
		if (kinds == DonatShopData.DonatKinds.StarterPack) {
			Hide();
			StarterPackButton.SetActive(false);
		}
	}


	private void OnSelectJetpack(Jetpacks.JetpacksType _) {
		UpdatePackButton();
	}


	private void OnSelectCharacter(CharacterKinds _) {
		UpdatePackButton();
	}


	private void UpdatePackButton() {
		StarterPackButton.SetActive(CanBuy);
		if (!CanBuy) ShopDonatsPanel.Purchased();
	}
}