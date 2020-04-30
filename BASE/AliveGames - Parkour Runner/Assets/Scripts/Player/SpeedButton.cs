using UnityEngine;
using System.Collections;
using Managers;
using ParkourRunner.Scripts.Player.InvectorMods;

public class SpeedButton : MonoBehaviour
{
    [SerializeField] private float _deltaSpeed;

    private ParkourThirdPersonController _player;

    private void OnEnable()
    {
        if (PhotonGameManager.IsMultiplayerAndConnected) {
            gameObject.SetActive(false);
            return;
        }
        _player = ParkourThirdPersonController.instance;

        if (_player == null)
            StartCoroutine(InitializeProcess());
        else
            _player.ButtonSpeed = 0f;
    }

    private IEnumerator InitializeProcess()
    {
        while (_player == null)
        {
            yield return new WaitForEndOfFrame();
            _player = ParkourThirdPersonController.instance;
        }

        _player.ButtonSpeed = 0f;
    }

    public void OnButtonPressedDown()
    {
        _player.ButtonSpeed = _deltaSpeed;
    }

    public void OnButtonPressedUp()
    {
        _player.ButtonSpeed = 0f;
    }
}