using System.Collections;
using Managers;
using ParkourRunner.Scripts.Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ExtremlyReloader : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _minY;
    [SerializeField] private float _checkTime;

    private void Start()
    {
        StartCoroutine(CheckFallErrorProcess());
    }

    private IEnumerator CheckFallErrorProcess()
    {
        float duration = _checkTime;

        while (true)
        {
            if(!_target) yield break;

            duration -= Time.deltaTime;
            if (duration <= 0 && _target.position.y < _minY)
            {
                if(PhotonGameManager.IsMultiplayer && PhotonGameManager.GameEnded) yield break;
                Debug.LogError("Fall player muscles (colliders). Camera under floor");
                Reload();
            }

            yield return null;
        }
    }

    private void Reload()
    {
        if (PhotonGameManager.IsMultiplayer) {
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
