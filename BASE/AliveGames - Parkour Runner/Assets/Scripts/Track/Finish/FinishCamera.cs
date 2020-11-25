using Managers;
using UnityEngine;
using ParkourRunner.Scripts.Player.InvectorMods;
using Photon.Pun;

public class FinishCamera : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PhotonGameManager.IsMultiplayerAndConnected) {
                if(!other.GetComponent<PhotonView>().IsMine || other.CompareTag("Bot Player")) return;
            }

            ParkourCamera.Instance.LockCamera = true;

            var player = ParkourThirdPersonController.instance;
            var input = player.GetComponent<ParkourThirdPersonInput>();

            input.lockInput = true;
        }
    }
}