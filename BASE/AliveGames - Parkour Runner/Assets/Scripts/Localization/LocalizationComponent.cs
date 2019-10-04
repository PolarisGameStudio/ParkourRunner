using UnityEngine;

public class LocalizationComponent : MonoBehaviour
{
    [SerializeField] private string _defaultText;
    [SerializeField] private string _key;

    private LocalizationManager _locManager;

    private void Awake()
    {
        _locManager = LocalizationManager.Instance;
    }

    public string Text
    {
        get
        {
            if (_locManager == null)
                _locManager = LocalizationManager.Instance;

            if (!_locManager.LockLocalization)
            {
                string txt = LocalizationManager.Instance.GetText(_key);
                return string.IsNullOrEmpty(txt) ? _defaultText : txt;
            }

            Debug.Log("Localization key " + _key + "was not found or used debug mode");

            return _defaultText;
        }
    }
}