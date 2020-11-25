using Managers;
using ParkourRunner.Scripts.Managers;
using Photon.Pun;
using UnityEngine;

namespace ParkourRunner.Scripts.Track.Pick_Ups
{
    public abstract class PickUp : MonoBehaviour
    {
        [SerializeField] public GameObject ParticlePrefab;

        protected abstract void Pick();

        private void OnTriggerEnter(Collider other)
        {
            TriggerEvent(other);
        }
                
        private void TriggerEvent(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Bot Player"))
            {
                if (!PhotonGameManager.IsMultiplayerAndConnected || other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine) {
                    Pick();
                }

                if (ParticlePrefab != null)
                {
                    var particle = Instantiate(ParticlePrefab, transform.position, transform.rotation);
                    Destroy(particle, 2f);
                }

                PoolManager.Instance.Remove(gameObject);
            }
        }
    }
}
