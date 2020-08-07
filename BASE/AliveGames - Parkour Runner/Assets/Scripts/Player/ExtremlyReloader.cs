using System.Collections;
using Managers;
using ParkourRunner.Scripts.Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ExtremlyReloader : MonoBehaviour
{
    private const float MinY = -100f;
    private const float CheckTime = 1f;

    [SerializeField] private Transform _target;


    private void Start()
    {
        StartCoroutine(CheckFallErrorProcess());
    }

    private IEnumerator CheckFallErrorProcess()
    {
        float duration = CheckTime;

        while (true)
        {
            if(!_target) yield break;

            duration -= Time.deltaTime;
            if (duration <= 0 && _target.position.y < MinY)
            {
                if(PhotonGameManager.IsMultiplayerAndConnected && PhotonGameManager.GameEnded) yield break;
                Debug.LogError("Fall player muscles (colliders). Camera under floor");
                Reload();
            }

            yield return null;
        }
    }

    private void Reload()
    {
        if (PhotonGameManager.IsMultiplayerAndConnected) {
            GameManager.Instance.Revive();
            PhotonGameManager.LocalPlayer.StartRun();
            return;
        }

        int index = SceneManager.GetActiveScene().buildIndex;
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(index);

        while (unloadOperation != null && !unloadOperation.isDone)
        {
        }

        ParkourSlowMo.Instance.UnSlow();
        SceneManager.LoadScene(index);
    }
}
