    using System.Collections;
    using Managers;
    using UnityEngine;
using ParkourRunner.Scripts.Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
    using Photon.Pun;

    public class FinishPoint : MonoBehaviour
{
    [SerializeField] private float _resultWindowDelay;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PhotonGameManager.IsMultiplayerAndConnected) return;

            StartCoroutine(FinishLevelProcess(other));
        }
    }

    private IEnumerator FinishLevelProcess(Collider other)
    {
        var hud = HUDManager.Instance;
        var player = ParkourThirdPersonController.instance;

        var input = player.GetComponent<ParkourThirdPersonInput>();
        input.Stop();

        yield return new WaitForSeconds(_resultWindowDelay);

        GameManager.Instance.CompleteLevel();
        hud.PostMortemScreen.CheckRateMe();
    }
}