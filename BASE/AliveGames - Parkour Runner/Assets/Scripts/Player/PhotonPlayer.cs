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

		DestroyComponents();

		PhotonGameManager.Players.Add(this);
		if (PhotonView.IsMine) PhotonGameManager.LocalPlayer = this;
		_animator = GetComponent<Animator>();
	}


	public void DestroyComponents() {
		if (PhotonView.IsMine) return;

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
		_animator.SetBool("isDead", true);
		// gameObject.GetComponent<Rigidbody>().isKinematic = false;
		// PuppetMaster.state = PuppetMaster.State.Dead;
	}
}