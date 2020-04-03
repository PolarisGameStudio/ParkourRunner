using System;
using System.Linq;
using Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
using Photon.Pun;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.Serialization;

public class PhotonPlayer : MonoBehaviour {
	[HideInInspector] public PlayerCanvas PlayerCanvas;
	[HideInInspector] public PhotonView   PhotonView;
	[HideInInspector] public bool         IsFinished;
	[HideInInspector] public int          FinishPlace;
	[HideInInspector] public bool         Ready;

	private ParkourThirdPersonInput _playerInput;
	private Animator                _animator;


	private void Awake() {
		PhotonView = GetComponent<PhotonView>();
		if (!PhotonGameManager.IsMultiplayer) return;

		GetComponents();
		if (!PhotonView.IsMine) {
			DestroyComponents();
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
		}

		StopRun();
	}


	private void Start() {
		if (!PhotonGameManager.IsMultiplayer) return;
		PhotonGameManager.AddPlayer(this);
	}


	private void GetComponents() {
		_playerInput = GetComponent<ParkourThirdPersonInput>();
		_animator    = GetComponent<Animator>();

		for (int i = 0; i < transform.childCount; i++) {
			var child = transform.GetChild(i);
			if (child.name.Equals("MultiplayerCanvas")) {
				PlayerCanvas = child.GetComponent<PlayerCanvas>();
				PlayerCanvas.PhotonView = PhotonView;
				break;
			}
		}
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
		var parent = transform.parent;
		for (int i = 0; i < parent.childCount; i++) {
			var child = parent.GetChild(i);
			if (child.name.Equals("Behaviours") || child.name.Equals("PuppetMaster")) {
				Destroy(child.gameObject);
				i -= 1;
			}
		}
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
		_playerInput.lockInput = true;
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
		if (!PhotonView.IsMine) return;

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


	[PunRPC]
	public void SetPosition(Vector3 position) {
		if (!PhotonView.IsMine) return;

		transform.position = position;
	}


	[PunRPC]
	public void SetLocalRotation(Quaternion rotation) {
		if (!PhotonView.IsMine) return;
		transform.localRotation = rotation;
	}


	[PunRPC]
	public void LockCamera() {
		ParkourCamera.Instance.LockCamera = true;
	}


	[PunRPC]
	public void UnlockCamera() {
		ParkourCamera.Instance.LockCamera = false;
	}


	[PunRPC]
	public void SetReward(int reward) {
		PlayerCanvas.SetReward(reward);
		if (PhotonView.IsMine) {
			Wallet.Instance.AddCoins(reward, Wallet.WalletMode.InGame);
		}
	}


	[PunRPC]
	public void SetFinishPlace(int place) {
		FinishPlace = place;
	}
}