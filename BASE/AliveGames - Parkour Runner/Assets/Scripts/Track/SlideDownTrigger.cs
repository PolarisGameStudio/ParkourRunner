using ParkourRunner.Scripts.Player.InvectorMods;
using UnityEngine;

namespace ParkourRunner.Scripts.Track
{
    public class SlideDownTrigger : MonoBehaviour
    {

        private ParkourThirdPersonController _player;

        private void OnTriggerExit(Collider other)
        {
            if(_player) _player.IsSlidingDown = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_player == null) _player = other.transform.GetComponent<ParkourThirdPersonController>();
                if (_player != null) {
                    _player.IsSlidingDown = true;
                    _player.animator.SetFloat("SlideAngle", transform.rotation.x);
                }
            }
            //_player.animator.CrossFadeInFixedTime("SlideDown", 0.2f);
        }
    
    }
}
