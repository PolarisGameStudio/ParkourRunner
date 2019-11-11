using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestTimer : MonoBehaviour
{
    [SerializeField] private Text _text;

    private void OnEnable()
    {
        SetTime();
        StartCoroutine(TimeTickProcess());
    }

    private IEnumerator TimeTickProcess()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SetTime();
        }
    }

    private void SetTime()
    {
        int seconds = (int)QuestManager.Instance.LeftSeconds;

        if (seconds > 0)
        {
            int hours = (seconds / 60) / 60;
            seconds = seconds - hours * 60 * 60;

            int minutes = seconds / 60;
            seconds -= minutes * 60;


            _text.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
        else
        {
            _text.text = "00:00:00";
        }
    }
}