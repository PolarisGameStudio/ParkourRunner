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

	[HideInInspector] public PhotonView PhotonView;
	[HideInInspector] public bool       IsFinished;
	[HideInInspector] public bool       Ready;

	[SerializeField] private GameObject[] DestroyOtherPlayersObjects;

	private ParkourThirdPersonInput _playerInput;
	private Animator                _animator;


	private void Awake() {
		if (!PhotonGameManager.IsMultiplayer) return;

		GetComponents();
		if (!PhotonView.IsMine) {
			DestroyComponents();
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
		}

		StopRun();
		PhotonGameManager.AddPlayer(this);
	}


	private void GetComponents() {
		PhotonView   = GetComponent<PhotonView>();
		_playerInput = GetComponent<ParkourThirdPersonInput>();
		_animator    = GetComponent<Animator>();
	}


	private void Update() {
		if (!PhotonView.IsMine) return;

		if (Input.GetKeyDown(KeyCode.Backspace)) {
			var controller = GetComponent<ParkourThirdPersonController>();
			if (controller.PuppetMaster.state == PuppetMaster.State.Alive) controller.Die();
			else controller.Revive();
		}
		else if (Input.GetKeyDown(KeyCode.P)) {
			_playerInput.LockRunning = !_playerInput.LockRunning;
		}
	}


	public void DestroyComponents() {
		DestroyOtherPlayersObjects.ToList().ForEach(Destroy);

		Destroy(GetComponent<ParkourThirdPersonController>());
		Destroy(GetComponent<ParkourThirdPersonInput>());
		Destroy(GetComponent<GenericActionPlusPuppet>());
		Destroy(GetComponent<InvectorPlusPuppet>());
		Destroy(GetComponent<CharacterEffects>());
		Destroy(transform.parent.GetComponent<ExtremlyReloader>());
	}


	public void StopRun() {
		_playerInput.Stop();
	}


	public void StopInput() {
		_playerInput.lockInput = false;
	}


	public void StartRun() {
		_playerInput.LockRunning = false;
		_playerInput.lockInput   = false;
	}


	[PunRPC]
	public void PlayerReady() {
		PlayerCanvas.PlayerReady();
		Ready = true;

		PhotonGameManager.CheckReady();
	}


	[PunRPC]
	public void StartGame() {
		if(!PhotonView.IsMine) return;

		var startPosition = ParkourThirdPersonController.instance.StartPosition;
		startPosition.x         = transform.position.x;
		transform.position      = startPosition;
		transform.localRotation = Quaternion.identity;
		StartRun();
	}


	[PunRPC]
	public void PlayAnimation(string animationName) {
		_animator.CrossFadeInFixedTime(animationName, 0.1f);
	}


	[PunRPC]
	public void Die() {
		_animator.SetBool("isDead", true);
	}


	[PunRPC]
	public void LoseBalance() {
		_animator.SetBool("isDead", true);
	}


	[PunRPC]
	public void RegainBalance() {
		_animator.SetBool("isDead", false);
	}


	[PunRPC]
	public void Finish() {
		if (PhotonView.IsMine) {
			StopInput();
		}
	}
}