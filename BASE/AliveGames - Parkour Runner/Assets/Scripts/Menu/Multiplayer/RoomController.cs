using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = System.Random;

public class RoomController : MonoBehaviourPunCallbacks {
	public const int MaxPlayers = 4;
	public const int MaxBet     = 1000;

	[SerializeField] private MultiplayerMenu MultiplayerMenu;
	[SerializeField] private RoomPanel       RoomPanelObject;
	[SerializeField] private RoomList        RoomListObject;
	[SerializeField] private GameObject      RoomPrefab;

	private int _bet = 100;


	private void Start() {
		if (PhotonNetwork.IsConnectedAndReady) {
			OnConnectedToMaster();
		}
		else {
			PhotonNetwork.NickName     =  "Player" + UnityEngine.Random.Range(100000, 999999);
			MultiplayerMenu.OnShowMenu += StartMultiplayer;
		}
	}


	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.LeaveLobby();
			SceneManager.LoadScene("Menu");
		}
	}


	private void StartMultiplayer() {
		PhotonNetwork.ConnectUsingSettings();
		MultiplayerMenu.SetStatus("Connecting...");
	}


	private void StopMultiplayer() {
		PhotonNetwork.Disconnect();
	}


	public override void OnConnectedToMaster() {
		MultiplayerMenu.HideStatusPanel();
		PhotonNetwork.JoinLobby();
		ShowRoomListPanel();
	}


	#region Show/Hide Panels

	public void ShowRoomPanel() {
		RoomPanelObject.ShowPanel();
	}


	public void HideRoomPanel() {
		RoomPanelObject.HidePanel();
	}


	public void ShowRoomListPanel() {
		RoomListObject.ShowListPanel();
	}


	public void HideRoomListPanel() {
		RoomListObject.HideListPanel();
	}

	#endregion


	#region Room Button Callbacks

	public void OnStartBtn() {
		RoomPanelObject.HideError();
		var players = PhotonNetwork.CurrentRoom.Players.Select(p => p.Value.NickName).ToArray();
		if (players.Length == 1) _bet = 0;

		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;

		PhotonNetwork.LeaveLobby();
		MultiplayerMenu.Hide(MultiplayerMenu.OpenGame);
	}


	public void OnLeaveRoomBtn() {
		RoomPanelObject.SetReadyOnLocalPlayer(false);
		PhotonNetwork.LeaveRoom();
		HideRoomPanel();
		ShowRoomListPanel();
		OnRefreshRoomListBtn();
	}


	public void OnBetPlusBtn() {
		/*if (User.Data.Money < _bet + 100) {
			RoomPanelObject.SetErrorText("Недостаточно средств для увеличения ставки");
			return;
		}*/
		_bet += 100;
		_bet =  (_bet <= MaxBet) ? _bet : MaxBet;
		UpdateCustomProperties();
	}


	public void OnBetMinusBtn() {
		RoomPanelObject.HideError();
		_bet -= 100;
		_bet =  (_bet >= 0) ? _bet : 0;
		UpdateCustomProperties();
	}


	public void OnReadyBtn() {
		var ready = RoomPanelObject.GetLocalPlayerRow().Ready;
		RoomPanelObject.SetReadyOnLocalPlayer(!ready);
	}

	#endregion


	#region Room List Button Callbacks

	public void OnCloseRoomListBtn() {
		HideRoomListPanel();
		DestroyRoomButtons();
		StopMultiplayer();
		MultiplayerMenu.OnHomeButtonClick();
	}


	public void OnRefreshRoomListBtn() {
		PhotonNetwork.JoinLobby();
	}


	public void OnCreateRoomBtn() {
		CreateRoom();
	}


	public void OnJoinRoomBtn(string roomName) {
		PhotonNetwork.JoinRoom(roomName);
	}

	#endregion


	#region Room Control

	private void UpdateCustomProperties() {
		Hashtable roomProperties = new Hashtable() { { "bet", _bet } };
		PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
	}

	#endregion


	#region Room List

	public void CreateRoom() {
		_bet = 100;
		// if (User.Data.Money < _bet) _bet = 0;

		Hashtable roomProperties = new Hashtable() {{"bet", _bet}, };

		RoomOptions roomOptions = new RoomOptions {
			CustomRoomPropertiesForLobby = new [] {"map", "scene", "bet"},
			CustomRoomProperties         = roomProperties,
			MaxPlayers                   = MaxPlayers,
			CleanupCacheOnLeave          = false
		};

		PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName, roomOptions);
		RoomPanelObject.SetStatus("LOBBY SEARCH");
	}


	private void UpdateRoomList(List<RoomInfo> roomList) {
		DestroyRoomButtons();
		roomList.ForEach(CreateRoomRow);
	}


	private void CreateRoomRow(RoomInfo room) {
		foreach (var p in room.CustomProperties) print($"{p.Key}:{p.Value}");
		var bet = (room.CustomProperties.ContainsKey("bet")) ? (int) room.CustomProperties["bet"] : 0;

		GameObject go = Instantiate(RoomPrefab, RoomListObject.RoomsContainer);
		go.SetActive(true);
		var roomRow = go.GetComponent<RoomRow>();
		roomRow.SetRoomName(room.Name);
		roomRow.SetPlayers(room.PlayerCount, room.MaxPlayers);
		roomRow.SetBet(bet);
		roomRow.SetOnClickListener(OnJoinRoomBtn);
		// if(User.Data.Money < bet) roomRow.BlockConnectButton();

		RoomListObject.RoomListButtons.Add(go);
	}


	private void DestroyRoomButtons() {
		RoomListObject.RoomListButtons.ForEach(Destroy);
		RoomListObject.RoomListButtons.Clear();
	}

	#endregion


	#region Pun Callbacks


	public override void OnPlayerEnteredRoom(Player newPlayer) {
		print($"Player {newPlayer.NickName} joined to room");
		RoomPanelObject.UpdatePlayers();
		RoomPanelObject.CheckPlayersReady();
		RoomPanelObject.CheckPlayersCount();
	}


	public override void OnPlayerLeftRoom(Player otherPlayer) {
		print($"Player {otherPlayer.NickName} left the room");
		RoomPanelObject.UpdatePlayers();

		if (PhotonNetwork.IsMasterClient) RoomPanelObject.UnblockControlButton();
	}


	public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
		if (!propertiesThatChanged.ContainsKey("bet")) return;

		_bet = (int) propertiesThatChanged["bet"];
		RoomPanelObject.SetBet(_bet);
	}


	public override void OnJoinedRoom() {
		print("Joined to room");
		RoomPanelObject.SetStatus("WAITING FOR PLAYERS");

		HideRoomListPanel();
		ShowRoomPanel();
		RoomPanelObject.GetLocalPlayerRow().SetNickname(PhotonNetwork.NickName);
		RoomPanelObject.UpdatePlayers();

		if (!PhotonNetwork.IsMasterClient) RoomPanelObject.BlockControlButtons();
		else {
			RoomPanelObject.SetReadyOnLocalPlayer(true);
			RoomPanelObject.CheckPlayersReady();
			RoomPanelObject.CheckPlayersCount();
		}
	}


	public override void OnJoinRandomFailed(short returnCode, string message) {
		print($"Failed to join room: {message}, code:{returnCode}");
		HideRoomPanel();
		ShowRoomListPanel();
	}


	public override void OnRoomListUpdate(List<RoomInfo> roomList) {
		UpdateRoomList(roomList);
	}


	public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps) {
		if (Equals(target, PhotonNetwork.LocalPlayer)) return;
		if (!changedProps.ContainsKey("ready")) return;

		var ready = (bool) changedProps["ready"];
		RoomPanelObject.GetPlayerRow(target.ActorNumber).SetReady(ready);
		RoomPanelObject.CheckPlayersReady();
		RoomPanelObject.CheckPlayersCount();
	}

	#endregion


	[Serializable]
	private class RoomPanel {
		[SerializeField] private GameObject Panel;
		[SerializeField] private Text       Status;
		[SerializeField] private Text       PlayerCount;
		[SerializeField] private Transform  PlayersContainer;
		[SerializeField] private PlayerRow  PlayerRow;
		[SerializeField] private Text       BetText;
		[SerializeField] private Text       ErrorText;

		[SerializeField] private Button StartButton;
		[SerializeField] private Button BetPlusButton;
		[SerializeField] private Button BetMinusButton;
		[SerializeField] private Button ReadyButton;

		private Dictionary<int, PlayerRow> _otherPlayersRows = new Dictionary<int, PlayerRow>();


		public void ShowPanel() {
			Panel.SetActive(true);
		}


		public void HidePanel() {
			Panel.SetActive(false);
		}


		public void HideError() {
			ErrorText.gameObject.SetActive(false);
		}


		public void SetStatus(string status) {
			Status.text = status;
		}


		public void SetBet(int bet) {
			BetText.text = bet.ToString();
		}


		public void SetPlayersCount(int players, int maxPlayers) {
			PlayerCount.text = $"{players}/{maxPlayers}";
		}


		public void UpdatePlayers() {
			SetPlayersCount(PhotonNetwork.PlayerList.Length, MaxPlayers);

			DestroyPlayerRows();
			foreach (var player in PhotonNetwork.PlayerListOthers) {
				var row = CreatePlayerRow(player);
				_otherPlayersRows.Add(player.ActorNumber, row);
			}
		}


		public void CheckPlayersReady() {
			if (AllPlayersReady()) UnBlockStartButton();
			else BlockStartButton();
		}


		public void CheckPlayersCount() {
			// if (_otherPlayersRows.Count <= 0) BlockStartButton();
		}


		public bool AllPlayersReady() {
			if (!PlayerRow.Ready) return false;

			foreach (var p in _otherPlayersRows) {
				if (!p.Value.Ready) return false;
			}
			return true;
		}


		public void BlockControlButtons() {
			StartButton.gameObject.SetActive(false);
			ReadyButton.gameObject.SetActive(true);
			BetPlusButton.interactable  = false;
			BetMinusButton.interactable = false;
		}


		public void UnblockControlButton() {
			StartButton.gameObject.SetActive(true);
			ReadyButton.gameObject.SetActive(false);
			BetPlusButton.interactable  = true;
			BetMinusButton.interactable = true;
		}


		public void BlockStartButton() {
			StartButton.interactable = false;
		}


		public void UnBlockStartButton() {
			StartButton.interactable = true;
		}


		public PlayerRow GetPlayerRow(int actorId) {
			return _otherPlayersRows[actorId];
		}


		public PlayerRow GetLocalPlayerRow() {
			return PlayerRow;
		}


		public void SetReadyOnLocalPlayer(bool ready) {
			Hashtable playerProperties = new Hashtable() { { "ready", ready } };
			PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
			PlayerRow.SetReady(ready);
		}


		public void SetErrorText(string error) {
			ErrorText.text = error;
			ErrorText.gameObject.SetActive(true);
		}


		private PlayerRow CreatePlayerRow(Player player) {
			var go     = Instantiate(PlayerRow, PlayersContainer);
			var pRow   = go.GetComponent<PlayerRow>();
			var pReady = (player.CustomProperties.ContainsKey("ready")) && (bool) player.CustomProperties["ready"];
			pRow.SetNickname(player.NickName);
			pRow.SetReady(pReady);
			return pRow;
		}


		private void DestroyPlayerRows() {
			foreach (PlayerRow playerRow in _otherPlayersRows.Values) {
				Destroy(playerRow.gameObject);
			}
			_otherPlayersRows.Clear();
		}
	}

	[Serializable]
	private class RoomList {
		[SerializeField] private GameObject ListPanel;
		public                   Transform  RoomsContainer;

		internal List<GameObject> RoomListButtons = new List<GameObject>();


		public void ShowListPanel() {
			ListPanel.SetActive(true);
		}


		public void HideListPanel() {
			ListPanel.SetActive(false);
		}


		public void UpdateRooms() { }
	}
}