using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class NickNameField : MonoBehaviour
{
	private InputField _inputField;
	private const string NickNameKey = "NickName";

	private void Start() {
		_inputField = GetComponent<InputField>();

		LoadNickName();
	}


	private void LoadNickName() {
		var nickName = "Player" + Random.Range(100000, 999999);

		if (!PlayerPrefs.HasKey(NickNameKey)) {
			SubmitForm(nickName);
		}
		else {
			nickName         = PlayerPrefs.GetString(NickNameKey);
			SubmitForm(nickName);
		}
	}


	public void OnEndEdit(string value)
	{
		if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Tab))
		{
			SubmitForm(value);
		}
		else
		{
			SubmitForm(value);
		}
	}

	public void SubmitForm(string value) {
		value = value.Trim();
		if (value.Length < 3) value = PlayerPrefs.GetString(NickNameKey);
		PlayerPrefs.SetString(NickNameKey, value);
		PhotonNetwork.NickName = value;
		_inputField.text = value;
	}
}
