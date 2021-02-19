using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppsFlyerSDK;
using Managers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class RoomController : MonoBehaviourPunCallbacks {
	public const int MaxPlayers = 4;
	public const int MaxBet     = 1000;


	#region For bots

	private const int MinimumRooms          = 2;
	private const int MinimumVirtualPlayers = 1;
	private const int MaximumVirtualPlayers = 3;

	#endregion


	[SerializeField] private MultiplayerMenu MultiplayerMenu;
	[SerializeField] private RoomPanel       RoomPanelObject;
	[SerializeField] private RoomList        RoomListObject;
	[SerializeField] private GameObject      RoomPrefab;

	private int _bet = 100;

	private List<VirtualRoom> _virtualRoomList = new List<VirtualRoom>();
	private Coroutine         _virtualStartCoroutine;


	private void Start() {
		MultiplayerMenu.OnShowMenu += StartMultiplayer;
	}


	private void StartMultiplayer() {
		if (PhotonNetwork.IsConnectedAndReady) {
			OnConnectedToMaster();
			return;
		}

		print("StartMultiplayer");
		PhotonNetwork.ConnectUsingSettings();
		MultiplayerMenu.SetStatus("Connecting...");
	}


	private void StopMultiplayer() {
		PhotonNetwork.Disconnect();
	}


	public override void OnConnectedToMaster() {
		print("Connected");
		_bet = 100;
		MultiplayerMenu.HideStatusPanel();
		PhotonNetwork.JoinLobby();
		SetDefaultCustomProperties();
	}


	private void SetDefaultCustomProperties() {
		print("Set default player properties");
		Hashtable playerProperties = new Hashtable {{"money", Wallet.Instance.AllCoins}, {"ready", false}};
		PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
	}


	public override void OnJoinedLobby() {
		ShowRoomListPanel();
	}


	public override void OnDisconnected(DisconnectCause cause) {
		print($"Disconneted: {cause.ToString()}");
		if (cause == DisconnectCause.DisconnectByClientLogic) return;

		MultiplayerMenu.Hide(delegate {
			HideRoomListPanel();
			DestroyRoomButtons();
			StartMultiplayer();
			_virtualRoomList.Clear();
		});
	}


	private int GetMinBalanceInRoom() {
		if (!PhotonNetwork.InRoom) return 0;

		return (int) PhotonNetwork.CurrentRoom.Players.Values.ToList().Min(p => p.CustomProperties["money"]);
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

		PhotonNetwork.CurrentRoom.IsOpen    = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;

		var room       = PhotonNetwork.CurrentRoom;
		var players    = room.Players.Values.Select(p => p.NickName);
		var properties = PhotonNetwork.CurrentRoom.CustomProperties;
		var bet        = (int) properties["bet"];

		var isVirtualRoom = (room.CustomProperties.ContainsKey("isVirtual")) && (bool) room.CustomProperties["isVirtual"];

		if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && !isVirtualRoom) {
			_bet = 0;
			UpdateCustomProperties();
		}

		AppsFlyerManager.StartMultiplayer(players, bet);

		PhotonNetwork.LeaveLobby();
		MultiplayerMenu.Hide(MultiplayerMenu.OpenGame);
	}


	public void OnLeaveRoomBtn() {
		RoomPanelObject.SetReadyOnLocalPlayer(false);
		RoomPanelObject.SetVirtualRoom(null);
		_virtualRoomList = new List<VirtualRoom>();
		PhotonNetwork.LeaveRoom();
		HideRoomPanel();
		ShowRoomListPanel();
		OnRefreshRoomListBtn();
	}


	public void OnBetPlusBtn() {
		if (Wallet.Instance.AllCoins < _bet + 100) {
			RoomPanelObject.SetErrorText(RoomPanelObject.NotEnoughMoneyText.Text);
			return;
		}

		print($"Min balance: {GetMinBalanceInRoom()}");
		if (GetMinBalanceInRoom() < _bet + 100) {
			RoomPanelObject.SetErrorText(RoomPanelObject.NotEnoughMoneyOtherText.Text);
			return;
		}
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

		if (RoomPanelObject.IsVirtualRoom) {
			StartTimerForVirtualStart();
		}
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
		RoomPanelObject.SetVirtualRoom(null);
	}


	private void OnCreateVirtualRoom(string roomName) {
		var virtualRoom = _virtualRoomList.FirstOrDefault(v => v.Name == roomName);
		RoomPanelObject.SetVirtualRoom(virtualRoom);

		CreateRoom(virtualRoom);
	}


	private void OnJoinRoomBtn(string roomName) {
		PhotonNetwork.JoinRoom(roomName);
	}

	#endregion


	#region Room Control

	private void UpdateCustomProperties() {
		Hashtable roomProperties;
		if (RoomPanelObject.IsVirtualRoom) {
			roomProperties = new Hashtable {{"bet", _bet}, {"isVirtual", true}, {"vPlayers", string.Join(",", RoomPanelObject.VirtualRoom.VirtualPlayers)}};
		}
		else {
			roomProperties = new Hashtable {{"bet", _bet}};
		}
		PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
	}


	private void StartTimerForVirtualStart() {
		if (_virtualStartCoroutine != null) StopCoroutine(_virtualStartCoroutine);
		_virtualStartCoroutine = StartCoroutine(VirtualStartCoroutine());
	}


	private void StopTimerForVirtualStart() {
		if (_virtualStartCoroutine == null) return;
		StopCoroutine(_virtualStartCoroutine);
		_virtualStartCoroutine = null;
	}


	private IEnumerator VirtualStartCoroutine() {
		if (!RoomPanelObject.AllPlayersReady())  yield break;
		yield return new WaitForSeconds(Random.Range(3, 5));
		if (RoomPanelObject.AllPlayersReady()) OnStartBtn();
	}

	#endregion


	#region Room List

	private RoomOptions GetRoomOptions(VirtualRoom virtualRoom = null) {
		Hashtable roomProperties;
		if (virtualRoom != null) {
			roomProperties = new Hashtable {{"bet", virtualRoom.Bet}, {"isVirtual", true}, {"vPlayers", string.Join(",", virtualRoom.VirtualPlayers)}};
		}
		else {
			roomProperties = new Hashtable {{"bet", _bet}};
		}

		var roomOptions = new RoomOptions {
			CustomRoomPropertiesForLobby = new [] {"map", "scene", "bet", "vPlayers"},
			CustomRoomProperties         = roomProperties,
			MaxPlayers                   = MaxPlayers,
			CleanupCacheOnLeave          = false
		};

		return roomOptions;
	}


	private void CreateRoom(VirtualRoom virtualRoom = null) {
		AppsFlyerManager.SendBaseEvent(AppsFlyerManager.BaseEvents.multiplayer_room_created);

		if (virtualRoom == null) {
			_bet = 100;
			if (Wallet.Instance.AllCoins < _bet) _bet = 0;
		}
		else {
			_bet = virtualRoom.Bet;
		}

		var roomOptions = GetRoomOptions(virtualRoom);
		PhotonNetwork.CreateRoom(virtualRoom != null ? virtualRoom.Name : PhotonNetwork.LocalPlayer.NickName, roomOptions);
		RoomPanelObject.SetStatus("LOBBY SEARCH");

		RoomPanelObject.UnblockControlButton();
		RoomPanelObject.UnBlockStartButton();
		RoomPanelObject.HideError();
	}


	private void UpdateRoomList(List<RoomInfo> roomList) {
		DestroyRoomButtons();

		if (roomList.Count < MinimumRooms) {
			if (_virtualRoomList.Count < MinimumRooms) {
				while (_virtualRoomList.Count < MinimumRooms) {
					var creatorNickName = "Player" + Random.Range(100000, 999999);
					var roomName        = creatorNickName;

					var virtualPlayers = new List<string> {creatorNickName};
					var randomPlayers  = Random.Range(MinimumVirtualPlayers - 1, MaximumVirtualPlayers);
					for (int i = 0; i < randomPlayers; i++) {
						virtualPlayers.Add("Player" + Random.Range(100000, 999999));
					}
					var bet         = Random.Range(1, MaxBet / 100) * 100;
					var virtualRoom = new VirtualRoom(roomName, virtualPlayers, bet);
					_virtualRoomList.Add(virtualRoom);
				}
			}
		}

		foreach (var virtualRoom in _virtualRoomList) {
			var roomName    = virtualRoom.Name;
			var roomOptions = GetRoomOptions(virtualRoom);
			var room        = new Room(roomName, roomOptions);
			roomList.Add(room);
		}

		roomList.ForEach(CreateRoomRow);
	}


	private void CreateRoomRow(RoomInfo room) {
		if (!room.CustomProperties.ContainsKey("isVirtual") || !(bool) room.CustomProperties["isVirtual"]) {
			if (room.PlayerCount <= 0) return;
		}

		foreach (var p in room.CustomProperties) print($"{p.Key}:{p.Value}");
		var bet               = (room.CustomProperties.ContainsKey("bet")) ? (int) room.CustomProperties["bet"] : 0;
		var isLocalVirtual    = (room.CustomProperties.ContainsKey("isVirtual")) && (bool) room.CustomProperties["isVirtual"];
		var virtualPlayersStr = room.CustomProperties.ContainsKey("vPlayers") ? ((string) room.CustomProperties["vPlayers"]) : string.Empty;
		var vPlayers          = new List<string>();
		if (!string.IsNullOrEmpty(virtualPlayersStr)) {
			vPlayers = virtualPlayersStr.Split(',').ToList();
		}
		if (vPlayers.Count > 0 && room.PlayerCount > 0) vPlayers.Remove(room.Name);

		GameObject go = Instantiate(RoomPrefab, RoomListObject.RoomsContainer);
		go.SetActive(true);
		var roomRow = go.GetComponent<RoomRow>();
		roomRow.SetRoomName(room.Name);
		roomRow.SetBet(bet);
		roomRow.SetPlayers(room.PlayerCount + vPlayers.Count, room.MaxPlayers);

		roomRow.SetOnClickListener(roomName => {
			if (isLocalVirtual) OnCreateVirtualRoom(roomName);
			else OnJoinRoomBtn(roomName);
		});

		if (Wallet.Instance.AllCoins < bet) roomRow.BlockConnectButton();

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

		if (RoomPanelObject.IsVirtualRoom && PhotonNetwork.IsMasterClient) {
			StopTimerForVirtualStart();

			if (RoomPanelObject.VirtualRoom.VirtualPlayers.Count > 0) {
				RoomPanelObject.VirtualRoom.VirtualPlayers.RemoveAt(0);
				UpdateCustomProperties();
			}

			RoomPanelObject.SetReadyOnLocalPlayer(true);
			RoomPanelObject.UnblockControlButton();
		}

		RoomPanelObject.UpdatePlayers();
		RoomPanelObject.CheckPlayersReady();
		RoomPanelObject.CheckPlayersCount();
	}


	public override void OnPlayerLeftRoom(Player otherPlayer) {
		print($"Player {otherPlayer.NickName} left the room");
		RoomPanelObject.UpdatePlayers();

		if (PhotonNetwork.IsMasterClient) RoomPanelObject.UnblockControlButton();
		if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
			RoomPanelObject.SetReadyOnLocalPlayer(true);
			RoomPanelObject.UnblockControlButton();
			RoomPanelObject.UnBlockStartButton();
		}
	}


	public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
		if (propertiesThatChanged.ContainsKey("bet")) {
			_bet = (int) propertiesThatChanged["bet"];
			RoomPanelObject.SetBet(_bet);
		}
		if (propertiesThatChanged.ContainsKey("vPlayers")) {
			RoomPanelObject.UpdatePlayers();
		}
	}


	public override void OnJoinedRoom() {
		print("Joined to room");
		RoomPanelObject.SetStatus("WAITING FOR PLAYERS");

		HideRoomListPanel();
		ShowRoomPanel();
		RoomPanelObject.GetLocalPlayerRow().SetNickname(PhotonNetwork.NickName);
		RoomPanelObject.UpdatePlayers();
		RoomPanelObject.HideError();

		var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
		if (roomProps.ContainsKey("bet")) {
			_bet = (int) roomProps["bet"];
			RoomPanelObject.SetBet(_bet);
		}

		if (PhotonNetwork.IsMasterClient) {
			if (RoomPanelObject.IsVirtualRoom) RoomPanelObject.BlockControlButtons();
			else RoomPanelObject.SetReadyOnLocalPlayer(true);

			RoomPanelObject.CheckPlayersReady();
			RoomPanelObject.CheckPlayersCount();
		}
		else {
			RoomPanelObject.BlockControlButtons();
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


	public override void OnCreatedRoom() {
		UpdateCustomProperties();
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

		public VirtualRoom VirtualRoom;
		public bool        IsVirtualRoom => VirtualRoom != null;

		public LocalizationComponent NotEnoughMoneyText;
		public LocalizationComponent NotEnoughMoneyOtherText;

		private Dictionary<int, PlayerRow> _otherPlayersRows   = new Dictionary<int, PlayerRow>();
		private List<PlayerRow>            _virtualPlayersRows = new List<PlayerRow>();


		public void ShowPanel() {
			Panel.SetActive(true);
			AdManager.Instance.ShowBottomBanner();
		}


		public void HidePanel() {
			Panel.SetActive(false);
			AdManager.Instance.HideBottomBanner();
		}


		public void HideError() {
			ErrorText.gameObject.SetActive(false);
		}


		public void SetStatus(string status) {
			Status.text = status;
		}


		public void SetBet(int bet) {
			BetText.text = bet.ToString();
			print($"Set bet {bet}");
		}


		public void SetPlayersCount(int players, int maxPlayers) {
			PlayerCount.text = $"{players}/{maxPlayers}";
		}


		public void UpdatePlayers() {
			DestroyPlayerRows();

			var players        = new List<Player> (PhotonNetwork.PlayerListOthers);
			var virtualPlayers = new List<string> ();
			var roomProps      = PhotonNetwork.CurrentRoom.CustomProperties;
			var isVirtualRoom  = roomProps.ContainsKey("isVirtual") && (bool) roomProps["isVirtual"];

			if (isVirtualRoom) {
				var virtualPlayersStr = roomProps.ContainsKey("vPlayers") ? ((string) roomProps["vPlayers"]) : string.Empty;
				if (!string.IsNullOrEmpty(virtualPlayersStr)) {
					virtualPlayers = virtualPlayersStr.Split(',').ToList();
				}
			}

			SetPlayersCount(virtualPlayers.Count + PhotonNetwork.PlayerList.Length, MaxPlayers);

			foreach (var player in players) {
				var row = CreatePlayerRow(player);
				_otherPlayersRows.Add(player.ActorNumber, row);
			}

			foreach (var row in virtualPlayers.Select(CreateVirtualPlayerRow)) {
				_virtualPlayersRows.Add(row);
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


		public void SetVirtualRoom(VirtualRoom virtualRoom) {
			VirtualRoom = virtualRoom;
		}


		public void SetErrorText(string error) {
			ErrorText.text = error;
			ErrorText.gameObject.SetActive(true);
		}


		private PlayerRow CreatePlayerRow(Player player) {
			print($"Create player {player.NickName} row");
			var go     = Instantiate(PlayerRow, PlayersContainer);
			var pRow   = go.GetComponent<PlayerRow>();
			var pReady = (player.CustomProperties.ContainsKey("ready")) && (bool) player.CustomProperties["ready"];

			print("Properties:");
			foreach (var property in player.CustomProperties) {
				print($"{property.Key}:{property.Value}");
			}
			print("----------------------");

			pRow.SetNickname(player.NickName);
			pRow.SetReady(pReady);
			return pRow;
		}


		private PlayerRow CreateVirtualPlayerRow(string name) {
			var go   = Instantiate(PlayerRow, PlayersContainer);
			var pRow = go.GetComponent<PlayerRow>();
			pRow.SetNickname(name);
			pRow.SetReady(true);
			return pRow;
		}


		private void DestroyPlayerRows() {
			_virtualPlayersRows.ForEach(p => Destroy(p.gameObject));
			foreach (var playerRow in _otherPlayersRows.Values) {
				Destroy(playerRow.gameObject);
			}

			_otherPlayersRows.Clear();
			_virtualPlayersRows.Clear();
		}
	}

	[Serializable]
	private class RoomList {
		[SerializeField] private GameObject ListPanel;
		public                   Transform  RoomsContainer;

		internal List<GameObject> RoomListButtons = new List<GameObject>();


		public void ShowListPanel() {
			ListPanel.SetActive(true);
			AdManager.Instance.ShowBottomBanner();
		}


		public void HideListPanel() {
			ListPanel.SetActive(false);
			AdManager.Instance.HideBottomBanner();
		}


		public void UpdateRooms() { }
	}

	public class VirtualRoom {
		public string       Name;
		public int          Bet;
		public List<string> VirtualPlayers;


		public VirtualRoom(string name, List<string> virtualPlayers, int bet) {
			Name           = name;
			VirtualPlayers = virtualPlayers;
			Bet            = bet;
		}
	}
}