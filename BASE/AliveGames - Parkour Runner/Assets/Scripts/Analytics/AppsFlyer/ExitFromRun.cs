using Managers;
using Photon.Pun;
using UnityEngine;

namespace Analytics.AppsFlyer {
	public class ExitFromRun : MonoBehaviour {
		public void OnExitButton() {
			if (EnvironmentController.CurrentMode == GameModes.Tutorial) {
				AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.leave_tutorial);
			}
			else if (EnvironmentController.CurrentMode == GameModes.Multiplayer) {
				if(PhotonGameManager.GameEnded) return;

				var room = PhotonNetwork.CurrentRoom;
				var nickname = PhotonNetwork.NickName;
				var otherPlayers = room.PlayerCount - 1;

				AppsFlyerManager.LeaveMultiplayer(nickname, otherPlayers);
			}
		}
	}
}