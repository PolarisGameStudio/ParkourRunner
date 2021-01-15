using System;
using System.Linq;
using MainMenuAndShop.Helmets;
using MainMenuAndShop.Jetpacks;
using UnityEngine;
using UnityEngine.Serialization;

public class CharactersView : MonoBehaviour {
	[SerializeField] private CharacterBlock[] _characters;

	[Space] [SerializeField] private Vector3 DefaultRotation;
	[SerializeField]         private Vector3 ShowSpineRotation;

	private GameObject _jetpack;


	private void Start() {
		OnSelectCharacter(CharactersData.CurrentCharacter);
		OnSelectHelmet(Helmets.CurrentHelmetType);
	}


	private void OnEnable() {
		CharacterSelection.OnSelectCharacter += OnSelectCharacter;
		HelmetSelection.OnSelectHelmet       += OnSelectHelmet;
		JetpackSelection.OnSelectJetpack     += OnSelectJetpack;
	}


	private void OnDisable() {
		CharacterSelection.OnSelectCharacter -= OnSelectCharacter;
		HelmetSelection.OnSelectHelmet       -= OnSelectHelmet;
		JetpackSelection.OnSelectJetpack     -= OnSelectJetpack;
	}


	#region Events

	private void OnSelectCharacter(CharacterKinds kind) {
		foreach (CharacterBlock item in _characters) {
			item.Characters.CharacterModel.SetActive(item.kind == kind);
		}

		OnSelectJetpack(Jetpacks.ActiveJetpackType);
	}


	private void OnSelectHelmet(Helmets.HelmetsType helmet) {
		foreach (CharacterBlock item in _characters) {
			var helmets = item.Characters.Helmets;

			foreach (var h in helmets) h.SetActive(false);
			if (helmet == Helmets.HelmetsType.NoHelmet) continue;

			helmets[(int) helmet - 1].SetActive(true);
		}
	}


	private void OnSelectJetpack(Jetpacks.JetpacksType jetpacksType) {
		if (_jetpack) Destroy(_jetpack);

		var character   = _characters.FirstOrDefault(c => c.kind == CharactersData.CurrentCharacter);
		var jetpackData = Jetpacks.GetActiveJetpack();
		if (jetpackData.JetpackType == Jetpacks.JetpacksType.NoJetpack) return;

		_jetpack                         = Instantiate(jetpackData.JetpackPrefab, character.Characters.JetpackParent);
		_jetpack.transform.localPosition = Vector3.zero;
		_jetpack.transform.localRotation = Quaternion.identity;
	}

	#endregion


	public void RotateToShowSpine() {
		foreach (var characterBlock in _characters) {
			characterBlock.Characters.CharacterModel.transform.localRotation = Quaternion.Euler(ShowSpineRotation);
		}
	}


	public void ResetRotation() {
		foreach (var characterBlock in _characters) {
			characterBlock.Characters.CharacterModel.transform.localRotation = Quaternion.Euler(DefaultRotation);
		}
	}


	[Serializable]
	private struct CharacterBlock {
		[FormerlySerializedAs("character")] public Character      Characters;
		public                                     CharacterKinds kind;

		[Serializable]
		public struct Character {
			public GameObject   CharacterModel;
			public GameObject[] Helmets;

			public Transform JetpackParent;
		}
	}
}