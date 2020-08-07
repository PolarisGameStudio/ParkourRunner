using System;
using System.Collections;
using AEngine;
using MainMenuAndShop.Jetpacks;
using ParkourRunner.Scripts.Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
using UnityEngine;
using UnityEngine.Serialization;

namespace ParkourRunner.Scripts.Player {
	public class JetpackController : MonoBehaviour {
		// Sound
		private const string JetpackOnPath     = "AudioResources/Sounds/Gameplay/Jetpack/jtpkon";
		private const string JetpackOffPath    = "AudioResources/Sounds/Gameplay/Jetpack/jtpk0ff";
		private const string JetpackFlyingPath = "AudioResources/Sounds/Gameplay/Jetpack/jtpkflyng";

		private const float MaxFlyHeight          = 15f;
		private const float MultiplierHeightSpeed = 15f;
		private const float FlySpeed              = 15f;

		[SerializeField] private Transform JetpackParent;

		private GameObject     _jetpackInstance;
		private Animator       _jetpackAnimator;
		private ParticleSystem _jetpackParticles;

		private float _maxFuel;
		private float _fuel;

		private ParkourThirdPersonController PlayerController;
		private Coroutine                    _jetpackCoroutine;

		private static readonly int         _isUsedParameter = Animator.StringToHash("IsUsed");
		private                 AudioSource _audioSourceOn, _audioSourceFly, _audioSourceOff;
		private                 AudioClip   _jetpackOnClip, _jetpackOffClip, _jetpackFlyingClip;


		public void ActivateJetpack() {
			if (_jetpackCoroutine != null) return;
			if (GameManager.Instance.gameState == GameManager.GameState.Dead) return;

			PlayerController = GetComponent<ParkourThirdPersonController>();
			ParkourCamera.Instance.OnJump(0.1f);
			_jetpackCoroutine = StartCoroutine(RunningJetpack());
		}


		private IEnumerator RunningJetpack() {
			PlayerController.JetpackIsActive = true;
			_jetpackAnimator.SetBool(_isUsedParameter, false);
			_jetpackParticles.Play();

			_audioSourceOn.Play();

			var playerTransform = transform;
			var rigidbody       = GetComponent<Rigidbody>();
			var velocity        = 0f;

			var oldSpeed = PlayerController.jumpForward;

			while (_fuel > 0) {
				if (GameManager.Instance.gameState == GameManager.GameState.Dead) break;

				if (!_audioSourceOn.isPlaying && !_audioSourceFly.isPlaying &&
					GameManager.Instance.gameState == GameManager.GameState.Run) {
					_audioSourceFly.Play();
				}
				else if (_audioSourceFly.isPlaying && GameManager.Instance.gameState == GameManager.GameState.Pause) {
					_audioSourceFly.Pause();
				}

				PlayerController.freeSpeed.runningSpeed = FlySpeed;
				PlayerController.freeSpeed.walkSpeed    = FlySpeed;
				PlayerController.jumpForward            = FlySpeed;

				var playerPosition = playerTransform.position;
				var rbVelocity     = rigidbody.velocity;
				var heightDelta    = MaxFlyHeight - playerPosition.y;
				var progress       = heightDelta         / MaxFlyHeight;
				var sin            = Mathf.Sin(progress) * 3f;

				rbVelocity.y       =  Mathf.SmoothStep(0f, MultiplierHeightSpeed, sin);
				rigidbody.velocity =  rbVelocity;
				_fuel              -= Time.deltaTime;

				yield return null;
			}

			_audioSourceFly.Stop();
			_audioSourceOff.Play();

			PlayerController.jumpForward     = oldSpeed;
			PlayerController.JetpackIsActive = false;
			_fuel                            = 0f;

			_jetpackAnimator.SetBool(_isUsedParameter, true);
			_jetpackParticles.Stop();
			_jetpackCoroutine = null;
		}


		private void Start() {
			InstantiateJetpack();
			InstantiateSound();
		}


		private void InstantiateJetpack() {
			var activeJetpack = Jetpacks.GetActiveJetpack();
			_fuel = _maxFuel  = activeJetpack.BoostTime;

			_jetpackInstance = Instantiate(activeJetpack.JetpackPrefab, JetpackParent);

			_jetpackInstance.transform.localPosition = Vector3.zero;
			_jetpackInstance.transform.localRotation = Quaternion.identity;

			_jetpackAnimator  = _jetpackInstance.GetComponent<Animator>();
			_jetpackParticles = _jetpackInstance.GetComponentInChildren<ParticleSystem>();
		}


		private void InstantiateSound() {
			_audioSourceOn  = gameObject.AddComponent<AudioSource>();
			_audioSourceFly = gameObject.AddComponent<AudioSource>();
			_audioSourceOff = gameObject.AddComponent<AudioSource>();

			_audioSourceOn.volume  = AudioManager.Instance.SoundVolumme;
			_audioSourceFly.volume = AudioManager.Instance.SoundVolumme;
			_audioSourceOff.volume = AudioManager.Instance.SoundVolumme;

			_jetpackOnClip     = Resources.Load (JetpackOnPath) as AudioClip;
			_jetpackOffClip    = Resources.Load (JetpackOffPath) as AudioClip;
			_jetpackFlyingClip = Resources.Load (JetpackFlyingPath) as AudioClip;

			_audioSourceOn.clip  = _jetpackOnClip;
			_audioSourceFly.clip = _jetpackFlyingClip;
			_audioSourceOff.clip = _jetpackOffClip;

			_audioSourceFly.loop = true;
		}


		/// <summary>
		/// Normalized fuel from 1 to 0
		/// </summary>
		public float GetProgress() {
			return _fuel / _maxFuel;
		}
	}
}