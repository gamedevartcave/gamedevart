using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Experimental.Input;

namespace CityBashers
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class PlayerController : MonoBehaviour
	{
		public static PlayerController instance { get; private set; }

		[Header ("General")]
		public Collider playerCol;
		public Rigidbody playerRb;
		public Animator PlayerUI;
		public Transform startingPoint;
		private Vector3 m_Move; // the world-relative desired move direction, calculated from camForward and user input.

		[SerializeField] public float m_MovingTurnSpeed = 360;
		[SerializeField] public float m_StationaryTurnSpeed = 180;
		[SerializeField] public float m_JumpPower = 12f;
		[SerializeField] float m_JumpPower_Forward = 2f;
		[SerializeField] public float m_DoubleJumpPower = 1.5f;
		[SerializeField] float m_AirControl = 5;
		[Range(1f, 10f)]
		[SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] public float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;
		public float terminalVelocity = 10;

		Rigidbody m_Rigidbody;
		public Animator m_Animator;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		public float moveMultiplier;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;

		[Header ("Health")]
		[ReadOnlyAttribute] public bool lostAllHealth;
		public int health;
		public int StartHealth = 100;
		public int MaximumHealth = 100;
		public Slider HealthSlider;
		public Slider HealthSlider_Smoothed;
		public float healthSliderSmoothing;
		public UnityEvent OnLostAllHealth;
		public PostProcessVolume postProcessUIVolume;

		[Header ("Magic")]
		public int magic;
		public int StartingMagic = 100;
		public int MaximumMagic = 100;
		public Slider MagicSlider;
		public Slider MagicSlider_Smoothed;
		public float magicSliderSmoothing;

		[Header ("Aiming")]
		public float normalFov = 65;
		public float aimFov = 35;
		public float aimSmoothing = 10;
		public GameObject CrosshairObject;
		public Transform AimingOrigin;
		[HideInInspector] public Vector3 AimDir;
		[HideInInspector] public Vector3 AimNoPitchDir;

		[Header ("Camera rig")]
		public SimpleFollow camRigSimpleFollow;
		public Vector3 camRigSimpleFollowRotNormal = new Vector3 (5, 15, 0);
		public Vector3 camRigSimpleFollowRotAiming;
		public MouseLook mouseLook;
		public Camera m_Cam; // A reference to the main camera in the scenes transform
		private Vector3 m_CamForward; // The current forward direction of the camera
		public Animator CrosshairAnim;

		[Header ("Camera offset")]
		public Transform CamFollow; // Camera offset follow point.
		private bool isRight; // Camera horizontal offset toggle state.
		public Vector2 cameraOffset; // Camera horizontal offset values.
		public float cameraOffsetSmoothing = 5; // Smoothing between offsets.

		[Header ("Shooting")]
		public float currentFireRate;
		[HideInInspector] public float nextFire;
		public UnityEvent OnShoot;

		[Header ("Weapons")]
		public int currentWeaponIndex;
		public GameObject[] Weapons;

		public RawImage DisplayedWeaponImage;
		public TextMeshProUGUI DisplayedWeaponAmmoText;

		[Header ("WeaponWheel")]
		private bool weaponTexturesAssigned;
		public TextMeshProUGUI currentSelectedWeaponAmmoText;
		public RawImage CurrentSelectedWeaponImage;
		public Texture2D[] weaponTextures;

		[Space (10)]
		public RawImage[] WeaponSelectionBackgrounds;
		public RawImage[] WeaponSelectionImages;

		public Color WeaponAvailableColor;
		public Color WeaponAmmoOutColor;
		[Space (10)]
		public TextMeshProUGUI[] weaponWheelAmmoTexts;
		public int[] currentAmmoAmounts;
		public int[] maxAmmoAmounts;

		[Header ("Weapon changing")]
		[ReadOnlyAttribute] public bool isChangingWeapon;
		public float weaponChangeRate =  0.25f;
		private float nextWeaponChange;
		public float OnWeaponChangeTimeScale = 0.05f;
		[ReadOnlyAttribute] public float WeaponChangeModeTime = 1;
		public float WeaponChangeDuration = 1;
		public UnityEvent OnWeaponChange;
		public UnityEvent WeaponChangeEnded;

		[Header ("Using")]
		public UnityEvent OnUse;

		[Header ("Footsteps")]
		public AudioSource footstepAudioSource;
		public AudioClip[] footstepClips;
		private int footstepSoundIndex;
		public float footStepRate = 0.25f;
		private float nextFootstepTime;
		public UnityEvent OnFootstep;

		[Header ("Jumping")]
		public AudioSource jumpAudioSource;
		public AudioClip[] jumpClips;
		private int jumpSoundIndex;
		private bool m_Jump; // Jump state.
		[HideInInspector] public bool m_DoubleJump; // Double jump input state.
		[SerializeField] [ReadOnlyAttribute] public bool doubleJumped; // Double jump state.m_Rigidbody.velocity.z.
		public UnityEvent OnJump;

		[Header ("Double jumping")]
		public AudioSource doubleJumpAudioSource;
		public AudioClip[] doubleJumpClips;
		private int doubleJumpSoundIndex;
		public UnityEvent OnDoubleJump;

		[Header ("Landing")]
		public AudioSource landingAudioSource;
		public AudioClip[] landingClips;
		private int landingSoundIndex;
		public UnityEvent OnLand;

		[Header ("Dodging")]
		[ReadOnlyAttribute] public bool isDodging;
		public float dodgeRate = 0.5f;
		public float dodgeSpeed = 15;
		private float nextDodge;
		private float dodgeTimeRemain;
		public float DodgeTimeDuration;
		public float dodgeTimeScale = 0.25f;
		public float dodgeSpeedupFactor = 20;
		public UnityEvent OnDodgeBegan;
		public UnityEvent OnDodgeEnded;

		[Header ("Hit stun")]
		[ReadOnlyAttribute] public bool isInHitStun;
		public Renderer[] skinnedMeshes;
		[ReadOnlyAttribute] [SerializeField] private float hitStunCurrentTime;
		public float hitStunDuration = 2;
		private WaitForSeconds hitStunYield;
		public float HitStunRenderToggleWait = 0.07f;
		public UnityEvent OnHitStunBegin;
		public UnityEvent OnHitStunEnded;

		private PlayerActions playerActions;

		void Awake ()
		{
			instance = this;

			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				skinnedMeshes [i].enabled = false;
			}

			this.enabled = false;
		}

		void Start ()
		{
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;

			// get the transform of the main camera
			if (Camera.main != null)
			{
				m_Cam = Camera.main;
			}

			else

			{
				Debug.LogWarning(
					"Warning: no main camera found. " + 
					"Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", 
					gameObject
				);
			}

			health = StartHealth;
			HealthSlider.value = health;
			HealthSlider_Smoothed.value = health;

			magic = StartingMagic;
			MagicSlider.value = magic;
			MagicSlider_Smoothed.value = magic;

			OnFootstep.AddListener (OnFootStep);
			OnJump.AddListener (OnJumpBegan);
			OnDoubleJump.AddListener (OnDoubleJumpBegan);
			OnLand.AddListener (OnLanded);

			hitStunYield = new WaitForSeconds (HitStunRenderToggleWait);

			playerActions = InControlActions.instance.playerActions;
		}

		void Update ()
		{
			JumpAction ();
			DodgeAction ();
			AimAction ();
			ShootAction ();
			MeleeAction ();
			UseAction ();
			CameraChangeAction ();
			AbilityAction ();
			CheckHealthSliders ();
			CheckMagicSliders ();
			CheckHealthMagicIsLow ();
		}

		void FixedUpdate ()
		{
			float fallMult = 2.5f;

			// If we are falling.
			if (m_Rigidbody.velocity.y < 0)
			{
				m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMult - 1) * Time.deltaTime;
			}
			else if (m_Rigidbody.velocity.y > 0 && InControlActions.instance.playerActions.Jump.IsPressed == false)
			{
				m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (m_JumpPower - 1) * Time.deltaTime;
			}

			m_Rigidbody.velocity = Vector3.ClampMagnitude (m_Rigidbody.velocity, terminalVelocity);

			float h = playerActions.Shoot.Value > 0.5f ? 0 : playerActions.Move.Value.x;
			float v = playerActions.Shoot.Value > 0.5f ? 0 : playerActions.Move.Value.y;

			//Debug.Log (playerActions.Move.Value);

			//bool crouch = playerActions.Crouch.IsPressed;

			// calculate move direction to pass to character
			if (m_Cam != null)
			{
				// Using self-relative controls.
				// calculate camera relative direction to move:
				m_CamForward = Vector3.Scale (m_Cam.transform.forward, new Vector3 (1, 0, 1)).normalized;
				m_Move = v * m_CamForward + h * m_Cam.transform.right;
			}

			else // calculate move direction in world space

			{
				// we use world-relative directions in the case of no main camera
				m_Move = v*Vector3.forward + h*Vector3.right;
			}

			//#if !MOBILE_INPUT
			// walk speed multiplier
			//if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
			//#endif

			// pass all parameters to the character control script
			//Move(m_Move, crouch, m_Jump, m_DoubleJump);
			Move (m_Move, false, m_Jump, m_DoubleJump);
			m_Jump = false;
			m_DoubleJump = false;
		}

		#region Movement

		public void Move(Vector3 move, bool crouch, bool jump, bool doubleJump)
		{
			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired direction.
			if (move.magnitude > 1f)
			{
				move.Normalize ();
			}

			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation();

			// control and velocity handling is different when grounded and airborne:
			if (m_IsGrounded)
			{
				HandleGroundedMovement(false, jump);

				if (PlayerController.instance.doubleJumped)
				{
					PlayerController.instance.doubleJumped = false;
				}
			}

			else

			{
				HandleAirborneMovement(doubleJump);
			}

			//ScaleCapsuleForCrouching(crouch);
			PreventStandingInLowHeadroom();

			// send input and other state parameters to the animator
			UpdateAnimator(move);
		}

		void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = 0.5f * m_Capsule.height;
				m_Capsule.center = 0.5f * m_Capsule.center;
				m_Crouching = true;
			}

			else

			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;

				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
					return;
				}

				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;

				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
				}
			}
		}

		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			//m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("OnGround", m_IsGrounded);

			if (m_IsGrounded == false)
			{
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle = Mathf.Repeat(
				m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;

			if (m_IsGrounded == true)
			{
				m_Animator.SetFloat ("JumpLeg", jumpLeg);
				m_Animator.SetFloat ("Jump", 0);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_Animator.GetBool ("Dodging") == false)
			{
				if (m_IsGrounded && move.magnitude > 0)
				{
					m_Animator.speed = m_AnimSpeedMultiplier;
				}

				else

				{
					// don't use that while airborne
					m_Animator.speed = 1;
				}
			}
		}

		void HandleAirborneMovement(bool doubleJump)
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce (extraGravityForce);

			float airControlForce = m_AirControl * InControlActions.instance.playerActions.Move.Value.magnitude;

			m_Rigidbody.AddRelativeForce (new Vector3 (0, 0, Mathf.Abs (airControlForce * 0.05f)),
				ForceMode.VelocityChange
			);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;

			// check whether conditions are right to allow a double jump:
			if (doubleJump && m_IsGrounded == false)
			{
				// Override vertical velocity.
				m_Rigidbody.velocity = new Vector3 (
					m_Rigidbody.velocity.x,
					m_DoubleJumpPower,
					m_Rigidbody.velocity.z
				);

				// Add forward force.
				m_Rigidbody.AddRelativeForce (
					0, 
					0, 
					m_JumpPower_Forward, 
					ForceMode.VelocityChange
				);

				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
				PlayerController.instance.OnDoubleJump.Invoke ();
			}
		}

		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump 
				//&& 
				//!crouch && 
				//m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded")
			)
			{
				// jump!
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				//m_GroundCheckDistance = 0.1f;
				PlayerController.instance.OnJump.Invoke ();
			}
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}

		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (m_IsGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;
			}
		}

		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
			#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine (transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
			#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;

				// Get grounded.
				if (m_IsGrounded == false)
				{
					m_IsGrounded = true;
					//m_Rigidbody.velocity = Vector3.zero;
					m_Rigidbody.velocity = new Vector3 (
						0.05f * m_Rigidbody.velocity.x, 
						m_Rigidbody.velocity.y, 
						0.05f * m_Rigidbody.velocity.z
					);

					PlayerController.instance.m_DoubleJump = false;
					PlayerController.instance.doubleJumped = false;

					if (DontDestroyOnLoadInit.Instance.initialized == true)
					{
						PlayerController.instance.OnLand.Invoke ();
					}

					m_Animator.applyRootMotion = true;
				}
			}

			else // is in air.

			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}

		#endregion

		#region Actions
		void JumpAction ()
		{
			// Jumping.
			if (!m_Jump && doubleJumped == false)
			{
				m_Jump = playerActions.Jump.WasPressed;
			}

			// Double jumping.
			if (m_Jump && doubleJumped == false)
			{
				m_DoubleJump = playerActions.Jump.WasPressed;

				if (playerActions.Jump.WasPressed)
				{
					doubleJumped = true;
				}
			}
		}

		void DodgeAction ()
		{
			if (playerActions.DodgeLeft.WasPressed || playerActions.DodgeRight.WasPressed)
			{
				// Get dodge angle.
				// Assign to player animation.
				if (Time.time > nextDodge)
				{
					isDodging = true;
					m_Animator.SetFloat ("DodgeDir", playerActions.Dodge.Value);
					m_Animator.SetTrigger ("Dodge");
					m_Animator.SetBool ("Dodging", true);
					moveMultiplier *= dodgeSpeedupFactor;
					m_AnimSpeedMultiplier *= dodgeSpeedupFactor;
					m_MovingTurnSpeed *= dodgeSpeedupFactor;
					m_StationaryTurnSpeed *= dodgeSpeedupFactor;
					m_Animator.updateMode = AnimatorUpdateMode.UnscaledTime;

					dodgeTimeRemain = DodgeTimeDuration;
					TimescaleController.instance.targetTimeScale = dodgeTimeScale;

					OnDodgeBegan.Invoke ();
					nextDodge = Time.time + dodgeRate;
					Debug.Log ("Dodged " + playerActions.Dodge.Value);
				}

				else // Not able to dodge yet.

				{
				}
			}

			if (dodgeTimeRemain <= 0)
			{
				if (isDodging == true)
				{
					if (GameController.instance.isPaused == false)
					{
						TimescaleController.instance.targetTimeScale = 1;
						isDodging = false;

						// Reset dodging.
						m_Animator.SetFloat ("DodgeDir", 0);
						m_Animator.ResetTrigger ("Dodge");
						m_Animator.SetBool ("Dodging", false);

						moveMultiplier /= dodgeSpeedupFactor;
						m_AnimSpeedMultiplier /= dodgeSpeedupFactor;
						m_MovingTurnSpeed /= dodgeSpeedupFactor;
						m_StationaryTurnSpeed /= dodgeSpeedupFactor;
						m_Animator.updateMode = AnimatorUpdateMode.Normal;

						OnDodgeEnded.Invoke ();
					}
				}
			} 

			else // There is dodge time.

			{
				if (GameController.instance.isPaused == false)
				{
					dodgeTimeRemain -= Time.unscaledDeltaTime;
					Vector3 relativeDodgeDir = 

						transform.InverseTransformDirection (
							//Camera.main.transform.right * 
							transform.forward * 
							//m_Animator.GetFloat ("DodgeDir") * 
							(playerActions.Move.Value.sqrMagnitude > 0 ? 1 : -1) * 
							dodgeSpeed * Time.unscaledDeltaTime
						);

					transform.Translate (relativeDodgeDir, Space.Self);

					m_Animator.SetBool ("Dodging", true);
				}
			}
		}

		void AimAction ()
		{
			// Aiming.
			if (playerActions.Aim.Value > 0.5f)
			{
				m_Cam.fieldOfView = Mathf.Lerp (
					m_Cam.fieldOfView, 
					PlayerController.instance.aimFov, 
					PlayerController.instance.aimSmoothing * Time.deltaTime
				);

				camRigSimpleFollow.FollowRotSmoothTime = camRigSimpleFollowRotAiming;

				/*
				if (CrosshairAnim.GetCurrentAnimatorStateInfo (0).IsName ("CrosshairOut") == true)
				{
					CrosshairAnim.ResetTrigger ("Out");
					CrosshairAnim.SetTrigger ("In");
					//mouseLook.rotationY = 0;
				}
				*/
			}

			// Not aiming.
			if (playerActions.Aim.Value <= 0.5f)
			{
				m_Cam.fieldOfView = Mathf.Lerp (
					m_Cam.fieldOfView, 
					PlayerController.instance.normalFov,
					PlayerController.instance.aimSmoothing * Time.deltaTime
				);

				camRigSimpleFollow.FollowRotSmoothTime = camRigSimpleFollowRotNormal;

				/*
				if (CrosshairAnim.GetCurrentAnimatorStateInfo (0).IsName ("CrosshairIn") == true)
				{
					CrosshairAnim.ResetTrigger ("In");
					CrosshairAnim.SetTrigger ("Out");
				}
				*/
			}
		}

		void ShootAction ()
		{
			if (playerActions.Shoot.Value > 0.5f)
			{
				if (Time.time > PlayerController.instance.nextFire && GameController.instance.isPaused == false)
				{
					PlayerController.instance.transform.rotation = Quaternion.LookRotation (
						PlayerController.instance.AimNoPitchDir, 
						Vector3.up
					);

					PlayerController.instance.Shoot ();
					PlayerController.instance.nextFire = Time.time + PlayerController.instance.currentFireRate;

					GameController.instance.camShakeScript.shakeDuration = 1;
					GameController.instance.camShakeScript.shakeAmount = 0.1f;
					GameController.instance.camShakeScript.Shake ();

					VibrateController.instance.Vibrate (0.25f, 0.25f, 0.25f, 1);

					Debug.Log ("Shooting from weapon " + PlayerController.instance.currentWeaponIndex);
				}
			}
		}

		void MeleeAction ()
		{
			if (playerActions.Melee.WasPressed)
			{
				// TODO: Make combos.

				Debug.Log ("Melee action was pressed.");
			}
		}

		void WeaponChangeAction ()
		{
			if (playerActions.NextWeapon.IsPressed == true && 
				GameController.instance.isPaused == false)
			{
				if (Time.unscaledTime > nextWeaponChange && GameController.instance.isPaused == false)
				{
					// Change to next weapon.
					if (PlayerController.instance.currentWeaponIndex < PlayerController.instance.Weapons.Length - 1)
					{
						PlayerController.instance.currentWeaponIndex++;
					} 

					else

					{
						PlayerController.instance.currentWeaponIndex = 0;
					}
				}

				isChangingWeapon = true;
				WeaponChangeModeTime = WeaponChangeDuration;
				PlayerController.instance.SetWeaponIndex (PlayerController.instance.currentWeaponIndex, true);
				OnWeaponChange.Invoke ();
				TimescaleController.instance.targetTimeScale = OnWeaponChangeTimeScale;
				nextWeaponChange = Time.unscaledTime + weaponChangeRate;
			}

			if (playerActions.PreviousWeapon.IsPressed == true && 
				GameController.instance.isPaused == false)
			{
				if (Time.unscaledTime > nextWeaponChange && GameController.instance.isPaused == false)
				{
					// Change to previous weapon.
					if (PlayerController.instance.currentWeaponIndex > 0)
					{
						PlayerController.instance.currentWeaponIndex--;
					} 

					else

					{
						PlayerController.instance.currentWeaponIndex = PlayerController.instance.Weapons.Length - 1;
					}
				}

				isChangingWeapon = true;
				WeaponChangeModeTime = WeaponChangeDuration;
				PlayerController.instance.SetWeaponIndex (PlayerController.instance.currentWeaponIndex, false);
				OnWeaponChange.Invoke ();
				TimescaleController.instance.targetTimeScale = OnWeaponChangeTimeScale;
				nextWeaponChange = Time.unscaledTime + weaponChangeRate;
			}

			if (WeaponChangeModeTime <= 0)
			{
				if (isChangingWeapon == true)
				{
					isChangingWeapon = false;
					TimescaleController.instance.targetTimeScale = 1;
					WeaponChangeEnded.Invoke ();
				}
			} 

			else // There is weapon changing time

			{
				if (GameController.instance.isPaused == false)
				{
					WeaponChangeModeTime -= Time.unscaledDeltaTime;
				}
			}
		}

		void UseAction ()
		{
			if (playerActions.Use.WasPressed == true)
			{
				PlayerController.instance.OnUse.Invoke ();
			}
		}

		void CameraChangeAction ()
		{
			if (playerActions.CameraChange.WasPressed)
			{
				isRight = !isRight;
			}

			GetCameraChangeSmoothing ();
		}

		void GetCameraChangeSmoothing ()
		{
			if (isRight == false)
			{
				CamFollow.localPosition = Vector3.Lerp (
					CamFollow.localPosition, 
					new Vector3 (
						cameraOffset.x,
						CamFollow.localPosition.y,
						CamFollow.localPosition.z
					), 
					cameraOffsetSmoothing * Time.deltaTime
				);

				return;
			} 

			else

			{
				CamFollow.localPosition = Vector3.Lerp (
					CamFollow.localPosition,
					new Vector3 (
						cameraOffset.y,
						CamFollow.localPosition.y,
						CamFollow.localPosition.z
					),
					cameraOffsetSmoothing * Time.deltaTime
				);

				return;
			}
		}

		void AbilityAction ()
		{
			if (playerActions.Ability.WasPressed)
			{
				Debug.Log ("Ability pressed.");
			}
		}

		#endregion

		#region Health
		void CheckHealthSliders ()
		{
			HealthSlider.value = Mathf.Clamp (health, 0, MaximumHealth);
			HealthSlider_Smoothed.value = Mathf.Lerp (
				HealthSlider_Smoothed.value, 
				health, 
				healthSliderSmoothing * Time.deltaTime
			);

			postProcessUIVolume.profile.GetSetting <Vignette> ().intensity.value = -0.005f * HealthSlider.value + 0.5f;
			postProcessUIVolume.profile.GetSetting <MotionBlur> ().shutterAngle.value = -3.6f * HealthSlider.value + 360;

			SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting<ColorGrading> ().mixerGreenOutGreenIn.value =
				Mathf.Clamp (3 * HealthSlider.value, 0, 100);
			
			SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting<ColorGrading> ().mixerBlueOutBlueIn.value =
				Mathf.Clamp (3 * HealthSlider.value, 0, 100);
			
			if (health <= 0)
			{
				if (lostAllHealth == false)
				{
					lostAllHealth = true;
					OnLostAllHealth.Invoke ();
					Debug.Log ("Player died.");
				}
			}
		}

		void CheckHealthMagicIsLow ()
		{
			if (health < 25 || magic < 25)
			{
				if (PlayerUI.GetBool ("Low") == false)
				{
					PlayerUI.SetBool ("Low", true);
				}
			} 

			else

			{
				if (PlayerUI.GetBool ("Low") == true)
				{
					PlayerUI.SetBool ("Low", false);
				}
			}
		}
		#endregion

		#region Magic
		void CheckMagicSliders ()
		{
			MagicSlider.value = Mathf.Clamp (magic, 0, MaximumMagic);
			MagicSlider_Smoothed.value = Mathf.Lerp (
				MagicSlider_Smoothed.value, 
				magic, 
				magicSliderSmoothing * Time.deltaTime
			);
		}
		#endregion

		#region Shooting
		public void Shoot ()
		{
			RaycastHit hit;
			Ray ray = m_Cam.ScreenPointToRay (Input.mousePosition);
			AimDir = ray.direction;
			AimNoPitchDir = new Vector3 (ray.direction.x, 0, ray.direction.z);

			if (Physics.Raycast (AimingOrigin.position, ray.direction, out hit, Mathf.Infinity))
			{
				Debug.DrawRay (AimingOrigin.position, ray.direction * 10, Color.cyan, 0.2f);
				Debug.DrawRay (AimingOrigin.position, AimNoPitchDir * 10, Color.green, 0.2f);
			}

			OnShoot.Invoke ();
		}
		#endregion

		#region Weapons
		public void SetWeaponIndex (int index, bool reverse)
		{
			bool runout = true;

			for (int i = 0; i < Weapons.Length; i++)
			{
				// If there is ammo in any weapon.
				if (currentAmmoAmounts [i] != 0)
				{
					runout = false;
				}

				// If there is no ammo in any weapon.
				if (runout == true)
				{
					return;
				}
			}

			// Scrolling down.
			if (reverse == true)
			{
				// Check for ammo has run out.
				// Then restart loop.
				if (currentAmmoAmounts [index] <= 0)
				{
					currentWeaponIndex++;

					if (currentWeaponIndex >= Weapons.Length)
					{
						currentWeaponIndex = 0;
					}

					SetWeaponIndex (currentWeaponIndex, true);
					return;
				}
			}

			else // Scrolling up

			{
				// Check for ammo has run out.
				// Then restart loop.
				if (currentAmmoAmounts [index] <= 0)
				{
					currentWeaponIndex--;

					if (currentWeaponIndex < 0)
					{
						currentWeaponIndex = Weapons.Length - 1;
					}

					SetWeaponIndex (currentWeaponIndex, false);
					return;
				}
			}

			// Update all weapon selections.
			for (int i = 0; i < Weapons.Length; i++)
			{	
				Weapons [i].SetActive ((index == i) ? true : false);
				weaponWheelAmmoTexts [i].text = currentAmmoAmounts [i] + " / " + maxAmmoAmounts [i];
				WeaponSelectionBackgrounds [i].enabled = ((index == i) ? true : false);

				// Sets all weapons without ammo to be not available.
				if (currentAmmoAmounts [i] <= 0)
				{
					WeaponSelectionImages [i].color = WeaponAmmoOutColor;
				} 

				else // Weapon is available.
				
				{
					WeaponSelectionImages [i].color = WeaponAvailableColor;
				}

				// Only called once to assign weapon textures.
				if (weaponTexturesAssigned == false)
				{
					WeaponSelectionImages [i].texture = weaponTextures [i];
				}
			}

			// Update displayed weapon.
			DisplayedWeaponImage.texture = weaponTextures [index];
			DisplayedWeaponAmmoText.text = currentAmmoAmounts [index] + " / " + maxAmmoAmounts [index];
				
			// Update current ammo selection for center of wheel.
			CurrentSelectedWeaponImage.texture = weaponTextures [index];
			currentSelectedWeaponAmmoText.text = currentAmmoAmounts [index] + " / " + maxAmmoAmounts [index];

			weaponTexturesAssigned = true; // Prevents assigning of weapon textures the next time.
		}
		#endregion

		#region Footsteps
		public void GetFootStepSound ()
		{
			if (Time.time > nextFootstepTime)
			{
				footstepSoundIndex = Random.Range (0, footstepClips.Length);
				footstepAudioSource.clip = footstepClips [footstepSoundIndex];
				footstepAudioSource.Play ();
				nextFootstepTime = Time.time + footStepRate;
			}
		}

		void OnFootStep ()
		{
			GetFootStepSound ();
		}
		#endregion

		#region Jumping
		public void GetJumpSound ()
		{
			jumpSoundIndex = Random.Range (0, jumpClips.Length);
			jumpAudioSource.clip = jumpClips [jumpSoundIndex];
			jumpAudioSource.Play ();
		}

		void OnJumpBegan ()
		{
			GetJumpSound ();
		}

		public void GetDoubleJumpSound ()
		{
			doubleJumpSoundIndex = Random.Range (0, doubleJumpClips.Length);
			doubleJumpAudioSource.clip = doubleJumpClips [doubleJumpSoundIndex];
			doubleJumpAudioSource.Play ();
		}

		void OnDoubleJumpBegan ()
		{
			GetDoubleJumpSound ();
		}

		public void GetLandingSound ()
		{
			landingSoundIndex = Random.Range (0, landingClips.Length);
			landingAudioSource.clip = landingClips [landingSoundIndex];
			landingAudioSource.Play ();
		}

		void OnLanded ()
		{
			//GetLandingSound ();
		}
		#endregion

		#region Physics

		// When player hits death barrier.
		// Hard reset on positioning and movement.
		public void DeathBarrier ()
		{
			transform.position = startingPoint.position;
			playerRb.velocity = Vector3.zero;
		}
		#endregion

		#region Override
		public void OverridePlayerPosition (Transform newPos)
		{
			transform.position = newPos.position;
		}

		public void OverridePosition (Vector3 pos)
		{
			this.transform.position = pos;
		}
		#endregion

		#region Hit stun
		public void DoHitStun ()
		{	
			if (isInHitStun == false)
			{
				StartCoroutine (Hitstun ());
			}
		}

		public IEnumerator Hitstun ()
		{
			// Get current time.
			hitStunCurrentTime = Time.time;
			isInHitStun = true;
			OnHitStunBegin.Invoke ();

			// Toggle skinned mesh renderer enabled.
			while (Time.time < hitStunCurrentTime + hitStunDuration && health > 0)
			{
				for (int i = 0; i < skinnedMeshes.Length; i++)
				{
					skinnedMeshes [i].enabled = !skinnedMeshes [i].enabled;
				}

				yield return hitStunYield;
			}

			// Always re enable skinned mesh renderers.
			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				// Always end with skinned mesh renderers enabled.
				skinnedMeshes [i].enabled = true;
			}
				
			OnHitStunEnded.Invoke ();
			isInHitStun = false;
		}
		#endregion

		public void EnableSkinnedMeshes ()
		{
			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				skinnedMeshes [i].enabled = true;
			}
		}
	}
}
