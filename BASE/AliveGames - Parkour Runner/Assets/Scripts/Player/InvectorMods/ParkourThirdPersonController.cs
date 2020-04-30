using System.Collections;
using Basic_Locomotion.Scripts.CharacterController;
using ParkourRunner.Scripts.Managers;
using RootMotion.Dynamics;
using UnityEngine;
using AEngine;
using Managers;
using Photon.Pun;

namespace ParkourRunner.Scripts.Player.InvectorMods
{
    public class ParkourThirdPersonController : vThirdPersonController
    {
        private const float MIN_LANDING_DELAY = 0.25f;

        public BehaviourPuppet BehavPuppet;
        public PuppetMaster PuppetMaster;
        public Weight RollPuppetCollisionResistance;
        public Vector3 StartPosition;
        public GameObject Head;

        public new static ParkourThirdPersonController instance;

        public bool IsPlayingAnimation { get; set; }
        public bool LoseBalance { get; set; }
        public bool InAir { get; set; }

        public bool IsDoAction { get; set; }
        public bool IsOnJumpPlatform;
        public bool IsSlidingDown = false;
        public bool IsSlidingTrolley = false;
        public bool IsRunningWall = false;
        public bool IsUsingHook = false;
        public bool Immune = false;

        public bool RestoreImmune { get; set; }
        public float RollKnockOutDistance = 4f;
        public float _oldKnockOutDistance;
        public float HookSpeed = 2f;
        public float CurrRunSpeed;
        public float CurrAnimSpeed;
        public float SpeedMult = 1f;

        private float _baseSpeed = 1f;
        private float _bonusBoostSpeed = 0f;
        private float _buttonSpeed = 0f;
        private float _minLandingDelay;

        public float HouseWallDelay = 4f;

        [HideInInspector] public Vector3 TrolleyOffset;
        [HideInInspector] public Vector3 WallOffset;
        [HideInInspector] public Vector3 HookOffset;
        [HideInInspector] public Transform TargetTransform;
        [SerializeField] private LayerMask _immuneLayers = new LayerMask();

        private float _oldSpeed;
        private bool _airSpeedFreeze = false;

        public float BaseSpeed
        {
            set { _baseSpeed = value; SpeedMult = _baseSpeed + _bonusBoostSpeed + _buttonSpeed; }
        }

        public float BonusBoostSpeed
        {
            set { _bonusBoostSpeed = value; SpeedMult = _baseSpeed + _bonusBoostSpeed + _buttonSpeed; }
        }

        public float ButtonSpeed
        {
            set { _buttonSpeed = value; SpeedMult = _baseSpeed + _bonusBoostSpeed + _buttonSpeed; }
        }

        //Чисто по приколу сделал чтоб он держался за IK пока едет на тарзанке

        [HideInInspector] public AvatarIKGoal TrolleyHand;

        private GameManager _gm;
        private LayerMask _damageLayers;
        private LayerMask _oldCollisions;
        private Weight _oldCollisionResistance;


        protected override void Awake()
        {
            if (PhotonGameManager.IsMultiplayerAndConnected && !GetComponent<PhotonView>().IsMine) {
                Destroy(this);
                return;
            }

            base.Awake();
            instance = this;
        }


        protected override void Start()
        {
            _damageLayers = LayerMask.NameToLayer("HouseWall");
            _gm = GameManager.Instance;
            ResetSpeed();

            base.Start();

            //почему то туда нельзя добавить ивент из этого класса, можно только из родительского
            BehavPuppet.onLoseBalance.unityEvent.AddListener(delegate {
                IsSlidingTrolley = false;
                _capsuleCollider.isTrigger = false;
                this.LoseBalance = true;
                if (PuppetMaster.state != PuppetMaster.State.Dead)
                {
                    if (Gender.Kind == Gender.GenderKinds.Male)
                        AudioManager.Instance.PlayRandomSound(Sounds.Damage1, Sounds.Damage2);
                    else
                        AudioManager.Instance.PlayRandomSound(Sounds.FemDamage1, Sounds.FemDamage2);
                }
            });
            BehavPuppet.onLoseBalance.unityEvent.AddListener(ResetSpeed);

            BehavPuppet.onRegainBalance.unityEvent.AddListener(delegate {
                //this.LoseBalance = false;
                StartCoroutine(ResetBalanceProcess());
            });
                        
            _damageLayers = BehavPuppet.collisionLayers;

            this.InAir = false;
            _minLandingDelay = MIN_LANDING_DELAY;
        }


        void OnAnimatorIK(int layerIndex)
        {
            if (!IsSlidingTrolley) return;

            animator.SetIKPositionWeight(TrolleyHand, 0.7f);
            animator.SetIKPosition(TrolleyHand, TargetTransform.position);
        }


        private void ResetSpeed()
        {
            CurrRunSpeed = StaticConst.MinRunSpeed;
            CurrAnimSpeed = StaticConst.MinAnimSpeed;
        }


        private IEnumerator ResetBalanceProcess()
        {
            yield return new WaitForSeconds(0.7f);  // Вычислено опытным путем
            this.LoseBalance = false;
        }

        private void Update()
        {
            CheckImmunity();
            ControllStates();
            ControllSpeed();

            ControllStopMove();

            if (isGrounded && !isJumping)
            {
                CameraEffects.Instance.IsHighJumping = false;
            }

            if (_minLandingDelay >= 0f)
            {
                _minLandingDelay = Mathf.Clamp(_minLandingDelay - Time.deltaTime, 0f, MIN_LANDING_DELAY);
            }

            var a = animator;
        }

        private void ControllStopMove()
        {
            if (stopMove)
            {
                StartCoroutine(StopMoveTimer());
            }
        }

        bool stopped;
        private IEnumerator StopMoveTimer()
        {
            if (stopped)
            {
                yield break;
            }

            float timer = 0;
            while (stopMove)
            {
                timer += Time.deltaTime;
                yield return null;
                if (timer >= HouseWallDelay)
                {
                    Die();
                    _gm.Revive();
                    yield break;
                }
            }
        }

        private void CheckImmunity()
        {
            if (customAction || Immune || RestoreImmune)
            {
                BehavPuppet.collisionLayers = _immuneLayers;
                _gm.PlayerCanBeDismembered = false;
                //PuppetMaster.mode = PuppetMaster.Mode.Kinematic;
            }
            else
            {
                BehavPuppet.collisionLayers = _damageLayers;
                _gm.PlayerCanBeDismembered = true;
                //PuppetMaster.mode = PuppetMaster.Mode.Active;
            }
        }

        private void ControllSpeed()
        {
            if (!_airSpeedFreeze)
            {
                freeSpeed.runningSpeed = CurrRunSpeed * SpeedMult;
                freeSpeed.walkSpeed = CurrRunSpeed;
                animator.SetFloat("TrickSpeedMultiplier", CurrAnimSpeed);

                CurrRunSpeed = Utility.MapValue(_gm.GameSpeed, 1f, StaticConst.MaxGameSpeed, StaticConst.MinRunSpeed, StaticConst.MaxRunSpeed);
                CurrAnimSpeed = Utility.MapValue(_gm.GameSpeed, 1f, StaticConst.MaxGameSpeed, StaticConst.MinAnimSpeed, StaticConst.MaxAnimSpeed);
            }
        }

        private void FixedUpdate()
        {
            AudioManager audio = AudioManager.Instance;
            bool isBaseAction = isJumping || isRolling || isSliding || IsSlidingDown || IsSlidingTrolley || this.IsPlayingAnimation;

            if (isGrounded && !isBaseAction && !this.LoseBalance && !_gm.IsLevelComplete && _gm.gameState != GameManager.GameState.Pause && !TutorialMessage.IsShowMessage && !this.IsDoAction)
            {
                audio.PlayUniqueSound(_gm.IsOneLeg ? Sounds.RunOneLeg : Sounds.Run);
            }
            else
            {
                audio.StopSound(Sounds.RunOneLeg);
                audio.StopSound(Sounds.Run);
            }
                        
            bool nowInAir = isJumping || !isGrounded;
            if (nowInAir != this.InAir)
            {
                if (nowInAir)
                    _minLandingDelay = MIN_LANDING_DELAY;

                if (!nowInAir && _minLandingDelay == 0f)
                {
                    audio.PlayUniqueSound(Sounds.Landing);
                }
            }

            this.InAir = nowInAir;

            if (IsSlidingTrolley)
            {
                float speed = 18f;
                transform.position = Vector3.MoveTowards(transform.position, TargetTransform.position + TrolleyOffset, speed * Time.deltaTime);
                
                Quaternion newRot = TargetTransform.rotation;
                newRot.x = 0;
                newRot.z = 0;
                transform.rotation = newRot;

                PuppetMaster.mode = PuppetMaster.Mode.Active;
                return;
            }

            if (IsRunningWall)
            {
                var newPos = Vector3.Lerp(transform.position, TargetTransform.position + WallOffset, 0.5f);
                transform.position = newPos;
            }

            if (IsUsingHook)
            {
                transform.position = Vector3.MoveTowards(transform.position, TargetTransform.position + HookOffset, HookSpeed * Time.fixedDeltaTime);
                transform.rotation = TargetTransform.rotation;
                if (Vector3.Distance(TargetTransform.position + HookOffset, transform.position) <= (HookSpeed * Time.deltaTime))
                {
                    IsUsingHook = false;
                }
            }
        }

        private void ControllStates()
        {
            animator.SetBool("IsRunningWall", IsRunningWall);
            animator.SetBool("IsSlidingDown", IsSlidingDown);
            animator.SetBool("IsSlidingTrolley", IsSlidingTrolley);
            animator.SetBool("IsUsingHook", IsUsingHook);
        }

        public new bool actions
        {
            get { return /*isRolling ||*/ quickStop || landHigh || customAction; }
        }

        public override void Roll()
        {
            bool staminaCondition = currentStamina > rollStamina;
            // can roll even if it's on a quickturn or quickstop animation
            bool actionsRoll = !actions || (actions && (quickStop));
            // general conditions to roll
            bool rollConditions = (input != Vector2.zero || speed > 0.25f) && actionsRoll && isGrounded && staminaCondition && !isJumping;

            if (!rollConditions || isRolling) return;

            string randomRoll = RandomTricks.GetRoll();
            animator.CrossFadeInFixedTime(randomRoll, 0.1f);
            _capsuleCollider.isTrigger = false;

            ParkourCamera.Instance.OnRoll();

            AudioManager.Instance.PlayUniqueSound(Sounds.Rift);
        }

        public override void Jump()
        {
            if (customAction) return;

            // know if has enough stamina to make this action
            bool staminaConditions = currentStamina > jumpStamina;
            // conditions to do this action
            bool jumpConditions = !isCrouching && isGrounded && !actions && staminaConditions && !isJumping;
            // return if jumpCondigions is false
            if (!jumpConditions) return;
            // trigger jump behaviour
            jumpCounter = jumpTimer;
            isJumping = true;

            jumpForward = isSprinting ? freeSpeed.sprintSpeed : freeSpeed.runningSpeed;

            // trigger jump animations
            if (input.sqrMagnitude < 0.1f)
                animator.CrossFadeInFixedTime("Jump", 0.1f);
            else
                animator.CrossFadeInFixedTime("JumpMove", .2f);
            // reduce stamina
            ReduceStamina(jumpStamina, false);
            currentStaminaRecoveryDelay = 1f;

            ParkourCamera.Instance.OnJump(0.1f);

            AudioManager.Instance.PlaySound(Sounds.Jump);
        }

        public void Die()
        {
            PuppetMaster.state = PuppetMaster.State.Dead;
            PuppetMaster.muscles[0].rigidbody.AddForce(_rigidbody.velocity); //толкаем таз скоростью капсулы

            var pView = GetComponent<PhotonView>();
            if (pView.IsMine) {
                pView.RPC("Die", RpcTarget.Others);
            }
        }
                
        public void Revive()
        {
            PuppetMaster.state = PuppetMaster.State.Alive;
        }
        
        public virtual void PlatformJump(float speed, float height)
        {
            jumpCounter = jumpTimer;
            isJumping = true;
            // trigger jump animations
            animator.CrossFadeInFixedTime("JumpMove", .2f);

            CameraEffects.Instance.IsHighJumping = true;
            ParkourCamera.Instance.OnJump(0.1f, 0.5f);

            StartCoroutine(FreezeInAir(speed, height));

            AudioManager.Instance.PlayUniqueSound(Gender.Kind == Gender.GenderKinds.Male ? Sounds.JumpStrong : Sounds.JumpStrongFem);
        }

        //Это нужно чтобы на прыжке с батута всегда была постоянная скорость
        private IEnumerator FreezeInAir(float speed, float height)
        {
            if (_airSpeedFreeze) yield break;
            _airSpeedFreeze = true;

            float oldSpeed = jumpForward;
            float oldHeight = jumpHeight;

            while (isJumping || !isGrounded)
            {
                IsOnJumpPlatform = true;
                freeSpeed.runningSpeed = speed;
                freeSpeed.walkSpeed = speed;
                jumpForward = speed;
                jumpHeight = height;
                yield return null;
            }

            jumpForward = oldSpeed;
            jumpHeight = oldHeight;
            _airSpeedFreeze = false;
            IsOnJumpPlatform = false;
        }
    }
}