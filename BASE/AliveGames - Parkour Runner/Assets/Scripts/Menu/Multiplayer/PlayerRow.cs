using UnityEngine;
using UnityEngine.UI;

public class PlayerRow : MonoBehaviour {
	[HideInInspector] public bool Ready;

	[SerializeField] private Text       Nickname;
	[SerializeField] private GameObject Checkmark;


	public void SetNickname(string nickname) {
		Nickname.text = nickname;
	}


	public void SetReady(bool ready) {
		print($"Set ready {ready} for nickname {Nickname.text}");
		Ready = ready;
		Checkmark.SetActive(ready);
	}
}