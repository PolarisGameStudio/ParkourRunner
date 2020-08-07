using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using AEngine;
using Managers;
using ParkourRunner.Scripts.Managers;

public class ReloadScene : MonoBehaviour {
	public void Reload() {
		int            index           = SceneManager.GetActiveScene().buildIndex;
		AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(index);
		AudioManager.Instance.PlaySound(Sounds.Tap);

		while (unloadOperation != null && !unloadOperation.isDone) { }

		ParkourSlowMo.Instance.UnSlow();
		SceneManager.LoadScene(index);
		AdManager.Instance.HideBottomBanner();

		SendEvent();
	}


	private static void SendEvent() {
		var level    = PlayerPrefs.GetInt(EnvironmentController.LEVEL_KEY, 0);
		var distance = GameManager.Instance.DistanceRun;
		var coins    = Wallet.Instance.InGameCoins;

		switch (EnvironmentController.CurrentMode) {
			case GameModes.Tutorial:
				AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.restart_tutorial);
				break;

			case GameModes.Endless:
				AppsFlyerManager.RestartEndless(distance, false,  coins);
				break;

			case GameModes.Levels:
				AppsFlyerManager.RestartLevel(distance, false, level, coins);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}