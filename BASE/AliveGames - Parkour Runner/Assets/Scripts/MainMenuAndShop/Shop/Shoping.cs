using MainMenuAndShop.Helmets;
using MainMenuAndShop.Jetpacks;
using UnityEngine;

public static class Shoping {
	public static void GetDonat(DonatShopData data) {
		switch (data.DonatKind) {
			case DonatShopData.DonatKinds.NoAds:
				PlayerPrefs.SetInt("NoAds", 1);
				PlayerPrefs.Save();
				break;

			case DonatShopData.DonatKinds.ByCoins1:
			case DonatShopData.DonatKinds.ByCoins2:
			case DonatShopData.DonatKinds.ByCoins3:
			case DonatShopData.DonatKinds.ByCoins4:
				Wallet.Instance.AddCoins(int.Parse(data.DonatValue), Wallet.WalletMode.Global);
				break;
			case DonatShopData.DonatKinds.StarterPack:
				PlayerPrefs.SetInt(StarterPack.SkinKey, 1);
				PlayerPrefs.SetString(CharactersData.CHARACTER_KEY, StarterPack.SkinKind.ToString());

				Jetpacks.GetJetpackData(StarterPack.JetpackType).Bought = true;
				Jetpacks.ActiveJetpackType = StarterPack.JetpackType;

				PlayerPrefs.SetInt(StarterPack.HelmetBoughtKey, 1);
				Helmets.CurrentHelmetType = StarterPack.HelmetType;

				CollectibleBonuses.AddBonus(BonusName.DoubleCoins, 20);

				PlayerPrefs.SetInt("NoAds", 1);
				PlayerPrefs.Save();
				break;
		}
	}


	public static void GetBonus(string bonusName) {
		PlayerPrefs.SetInt(bonusName, PlayerPrefs.GetInt(bonusName) + 1);
	}
}