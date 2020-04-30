using UnityEngine;
using UnityEngine.UI;

public class RoomRow : MonoBehaviour {
	[SerializeField] private Text   Name;
	[SerializeField] private Text   Bet;
	[SerializeField] private Text   Players;
	[SerializeField] private Button Button;

	public delegate void RoomOnClick(string roomName);


	public void SetRoomName(string name) {
		Name.text = name;
	}


	public void SetPlayers(int players, int maxPlayers) {
		Players.text = $"{players}/{maxPlayers}";
	}


	public void SetBet(int bet) {
		Bet.text = bet.ToString();
	}


	public void SetOnClickListener(RoomOnClick callback) {
		Button.onClick.AddListener(() => callback(Name.text));
	}


	public void BlockConnectButton() {
		Button.interactable = false;
	}
}