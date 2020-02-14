using System;
using System.Linq;
using Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
using Photon.Pun;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.Serialization;

public class PhotonPlayer : MonoBehaviour {
	public PlayerCanvas PlayerCanvas;
	public PhotonView   PhotonView;
	public PuppetMaster PuppetMaster;

	[HideInInspector] public bool Ready;

	[SerializeField] private GameObject[]            DestroyOtherPlayersObjects;
	[SerializeField] private Behaviour[]             DestroyOtherPlayersComponents;
	[SerializeField] private ParkourThirdPersonInput PlayerInput;

	private Animator _animator;


	private void Awake() {
		if (!PhotonGameManager.IsMultiplayer) {
			PlayerInput.LockRunning = false;
			return;
		}

		if (!PhotonView.IsMine) {
			DestroyComponents();
			gameObject.tag = "Untagged";
		}

		PhotonGameManager.Players.Add(this);
		if (PhotonView.IsMine) PhotonGameManager.LocalPlayer = this;
		_animator = GetComponent<Animator>();
	}


    private void Update()
    {
        if (!PhotonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            var controller = GetComponent<ParkourThirdPersonController>();
            if(controller.PuppetMaster.state == PuppetMaster.State.Alive) controller.Die();
            else controller.Revive();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerInput.LockRunning = !PlayerInput.LockRunning;
        }
    }


	public void DestroyComponents() {
		DestroyOtherPlayersObjects.ToList().ForEach(Destroy);
		DestroyOtherPlayersComponents.ToList().ForEach(Destroy);
		// DestroyOtherPlayersComponents.ToList().ForEach(c => c.enabled = false);
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}


	[PunRPC]
	public void PlayerReady() {
		PlayerCanvas.PlayerReady();
		Ready = true;

		PhotonGameManager.CheckReady();
	}
	
	
	public void StartGame() {
		var startPosition = ParkourThirdPersonController.instance.StartPosition;
		startPosition.x = transform.position.x;
		transform.position = startPosition;
		transform.localRotation = Quaternion.identity;
		PlayerInput.LockRunning = false;
		PlayerCanvas.HideReady();
	}


	[PunRPC]
	public void PlayAnimation(string animationName) {
		_animator.CrossFadeInFixedTime(animationName, 0.1f);
	}


	[PunRPC]
	public void Die() {
		print("Other Die");
		_animator.SetBool("isDead", true);
	}


	[PunRPC]
	public void LoseBalance() {
		print("Other lose balance");
		_animator.SetBool("isDead", true);
	}


	[PunRPC]
	public void RegainBalance() {
		print("Other regain balance");
		_animator.SetBool("isDead", false);
	}
}