using UnityEngine;
using UnityEngine.UI;

public class Vibrations : MonoBehaviour
{
	private const string VibrationKey = "InGameVibration";
	public static bool Enabled {
		get => PlayerPrefs.GetInt(VibrationKey, 1) == 1;
		private set {
			PlayerPrefs.SetInt(VibrationKey, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	[SerializeField] private Image Icon;

    private void Start()
    {
		UpdateIcon();
    }


	public void OnClick() {
		Enabled = !Enabled;
		UpdateIcon();
	}


	public void UpdateIcon() {
		Icon.color = Enabled ? Color.white : Color.gray;
	}
}
