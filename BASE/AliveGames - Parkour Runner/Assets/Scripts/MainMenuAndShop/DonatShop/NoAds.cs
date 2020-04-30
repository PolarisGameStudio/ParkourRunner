using System;
using UnityEngine;

namespace MainMenuAndShop.DonatShop {
	public class NoAds : MonoBehaviour {
		[SerializeField] private ShopDonatsPanel ShopDonatsPanel;
		private void Start() {
			if (PlayerPrefs.GetInt("NoAds", 0) == 1) {
				ShopDonatsPanel.Purchased();
			}
		}
	}
}