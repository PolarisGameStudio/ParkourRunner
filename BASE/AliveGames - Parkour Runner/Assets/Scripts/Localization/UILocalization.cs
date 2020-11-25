using UnityEngine;
using UnityEngine.UI;

public class UILocalization : MonoBehaviour
{
    [SerializeField] private string _key;
    [SerializeField] private Text _text;

    private void Start()
    {
        Localize();
        Assets.SimpleLocalization.LocalizationManager.LocalizationChanged += Localize;
        /*if (!LocalizationManager.Instance.LockLocalization)
        {
            string txt = LocalizationManager.Instance.GetText(_key);

            if (!string.IsNullOrEmpty(txt))
                _text.text = txt;
        }*/
    }

    public void OnDestroy()
    {
        Assets.SimpleLocalization.LocalizationManager.LocalizationChanged -= Localize;
    }

    private void Localize()
    {
        _text.text = Assets.SimpleLocalization.LocalizationManager.Localize(_key);
    }
}