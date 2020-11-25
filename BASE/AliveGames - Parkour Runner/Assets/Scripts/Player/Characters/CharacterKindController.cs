using System;
using System.Linq;
using Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterKindController : MonoBehaviour {
	[SerializeField] private GameObject _camera;

	[Tooltip("It must be true (load character by player choose). Use false as debug mode with Kind property below.")] [SerializeField]
	private bool _selectBySettings = true;

	[Tooltip("Use as debug mode with disabled property SelectBySettings.")] [SerializeField]
	private CharacterKinds _kind;

	[SerializeField] private Vector3        _startPosition;
	[SerializeField] private CharactersData _data;


	private void Awake() {
		_camera.SetActive(false);

		CharactersData.Data targetData = _data.GetCharacterData(_selectBySettings ? LoadChoosenKind() : _kind);

		if (targetData != null) {
			if (PhotonGameManager.IsMultiplayerAndConnected) {
				PhotonNetwork.Instantiate("Character/Final Characters/" + targetData.targetPrefab.name, _startPosition, Quaternion.identity);

				if (PhotonNetwork.IsMasterClient) {
					CreateBots();
				}
			}
			else {
				Instantiate(targetData.targetPrefab, _startPosition, Quaternion.identity);
			}
		}

		ParkourThirdPersonController.instance.StartPosition = _startPosition;
		_camera.SetActive(true);
	}


	private void CreateBots() {
		var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		var isVirtualRoom  = roomProperties.ContainsKey("isVirtual") && (bool) roomProperties["isVirtual"];

		if (isVirtualRoom) {
			var virtualPlayersStr = roomProperties.ContainsKey("vPlayers") ? (string) roomProperties["vPlayers"] : string.Empty;
			if (!string.IsNullOrEmpty(virtualPlayersStr)) {
				var bots = virtualPlayersStr.Split(',').ToList();
				for (var index = 0; index < bots.Count; index++) {
					var bot = bots[index];
					InstantiateBot(bot, index);
				}
			}
		}
	}


	private void InstantiateBot(string botName, int botIndex) {
		var characterKinds = Enum.GetValues(typeof(CharacterKinds));
		var randomKind     = (CharacterKinds) characterKinds.GetValue(Random.Range(0, characterKinds.Length));
		var prefab         = _data.GetCharacterData(randomKind);

		var bot = PhotonNetwork.Instantiate("Character/Final Characters/" + prefab.targetPrefab.name, _startPosition, Quaternion.identity);
		var photonPlayer = bot.GetComponentInChildren<PhotonPlayer>();
		photonPlayer.PhotonView.RPC(nameof(PhotonPlayer.SetIsBot), RpcTarget.All, botName, botIndex);
	}


	private CharacterKinds LoadChoosenKind() {
		if (!PlayerPrefs.HasKey(CharactersData.CHARACTER_KEY)) {
			PlayerPrefs.SetString(CharactersData.CHARACTER_KEY, CharacterKinds.Base.ToString());
			PlayerPrefs.Save();
		}

		return (CharacterKinds) Enum.Parse(typeof(CharacterKinds), PlayerPrefs.GetString(CharactersData.CHARACTER_KEY));
	}
}