using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Input;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace CityBashers
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class PlayerController : MonoBehaviour
	{
		public static PlayerController Instance { get; private set; }

		[Header("General")]
		[HideInInspector]
		public CapsuleCollider playerCol;
		public Rigidbody playerRb;
		private Animator playerAnim;
		public Transform startingPoint;

		[Header("Input")]
		public PlayerControls playerControls;
		public Vector2 MoveAxis;
		public bool aimInput;
		public bool shootInput;
		private float lastJumpValue;

		[Header("Movement")]
		[Tooltip("The world-relative desired move direction, calculated from camForward and user input.")]
		public Vector3 move;

		public float moveSpeedMultiplier = 1f;
		public float movingTurnSpeed = 360;
		public float stationaryTurnSpeed = 180;
		public float moveMultiplier;
		public float animSpeedMultiplier = 1f;
		public float turnAmount;
		public float forwardAmount;
		public Vector3 groundNormal;

		[Header("Health")]
		[HideInInspector] public bool lostAllHealth;
		public int health;
		public int StartHealth = 100;
		public int MaximumHealth = 100;
		public float healthUIVisibilityThreshold = 50;
		public Slider HealthSlider;
		public Slider HealthSlider_Smoothed;
		public float healthSliderSmoothing;
		public UnityEvent OnLostAllHealth;

		[Header("Magic")]
		public int magic;
		public int StartingMagic = 100;
		public int MaximumMagic = 100;
		public float magicUIVisibilityThreshold = 50;
		public Slider MagicSlider;
		public Slider MagicSlider_Smoothed;
		public float magicSliderSmoothing;
		public bool unlimitedMagic;

		[Header("Aiming")]
		public float normalFov = 65;
		public float aimFov = 35;
		public float aimSmoothing = 10;
		public GameObject CrosshairObject;
		public Transform AimingOrigin;
		[HideInInspector] public Vector3 AimDir;
		[HideInInspector] public Vector3 AimNoPitchDir;
		public UnityEvent OnAimRelease;

		[Header("Camera rig")]
		public SimpleFollow camRigSimpleFollow;
		public Vector3 camRigSimpleFollowRotNormal = new Vector3(25, 25, 0);
		public Vector3 camRigSimpleFollowRotAiming = new Vector3(60, 60, 0);
		[Space(10)]
		public MouseLook mouseLook;
		[HideInInspector] public Camera cam; // A reference to the main camera in the scenes transform
		private Vector3 camForwardDirection; // The current forward direction of the camera
		public Animator CrosshairAnim;

		[Header("Camera offset")]
		public Transform CamFollow; // Camera offset follow point.
		private bool isRight; // Camera horizontal offset toggle state.
		public Vector2 cameraOffset; // Camera horizontal offset values.
		public float cameraOffsetSmoothing = 5; // Smoothing between offsets.

		[Header("Camera effects")]
		public Animator PlayerUI;
		public PostProcessVolume postProcessUIVolume;

		[Header("Shooting")]
		public float currentFireRate;
		[HideInInspector] public float nextFire;
		public UnityEvent OnShoot;

		[Header("Melee Attacks")]
		public int ComboQueueSize = 3;
		public float TimeBetweenDeQueue = 0.2f;
		private Queue<int> _comboQueue;
		private float _timePassed = 0;

		[Header("Weapons")]
		public int currentWeaponIndex;
		public RawImage DisplayedWeaponImage;
		public GameObject[] Weapons;

		[Header("Using")]
		public UnityEvent OnUse;

		[Header("Footsteps")]
		public AudioSource footstepAudioSource;
		public AudioClip[] footstepClips;
		private int footstepSoundIndex;
		public UnityEvent OnFootstep;

		[Header("Jumping")]
		public int jumpState;
		public float jumpPower = 12f;
		public float jumpPower_Forward = 2f;
		[Space(10)]
		public float airControl = 5;
		public float gravityMultiplier = 2f;
		public AudioSource jumpAudioSource;
		public AudioClip[] jumpClips;
		private int jumpSoundIndex;
		public UnityEvent OnJump;

		[Header("Double jumping")]
		public float doubleJumpPower = 1.5f;
		public AudioSource doubleJumpAudioSource;
		public AudioClip[] doubleJumpClips;
		private int doubleJumpSoundIndex;
		public UnityEvent OnDoubleJump;

		[Header("Landing")]
		public float terminalVelocity = 10;
		public float fallMult = 2.5f;
		[ReadOnly] public bool isGrounded;
		public AudioSource landingAudioSource;
		public AudioClip[] landingClips;
		private int landingSoundIndex;
		public UnityEvent OnLand;

		[Header("Dodging")]
		[ReadOnly] public bool isDodging;
		[Tooltip("Time between each dodge event.")]
		public float dodgeRate = 0.5f;
		private float nextDodge;
		[Tooltip("Speed at which player moves while dodging.")]
		public float dodgeSpeed = 15;

		private float dodgeTimeRemain;
		[Tooltip("How long in unscaled time the dodge time lasts.")]
		public float DodgeTimeDuration;
		[Tooltip("The time scale during dodge time.")]
		public float dodgeTimeScale = 0.25f;
		[Tooltip("Animator speed factor while in dodge mode.")]
		public float dodgeSpeedupFactor = 20;
		[Tooltip("How much magic it costs per dodge.")]
		public int dodgeMagicCost = 10;
		[Tooltip("Layers to look out for when checking colliders for a potential dodge.")]
		public LayerMask dodgeLayerMask;
		public UnityEvent OnDodgeBegan;
		public UnityEvent OnDodgeEnded;

		[Header("Hit stun")]
		[ReadOnly] public bool isInHitStun;
		public Renderer[] stunMeshes;
		[ReadOnly] [SerializeField] private float hitStunCurrentTime;
		public float hitStunDuration = 2;
		private WaitForSeconds hitStunYield;
		public float HitStunRenderToggleWait = 0.07f;
		public UnityEvent OnHitStunBegin;
		public UnityEvent OnHitStunEnded;

		void Awake()
		{
			Instance = this;

			for (int i = 0; i < stunMeshes.Length; i++)
			{
				stunMeshes[i].enabled = false;
			}

			// Find some components.
			playerAnim = GetComponent<Animator>();
			playerRb = GetComponent<Rigidbody>();
			playerCol = GetComponent<CapsuleCollider>();

			enabled = false;
		}

		void Start()
		{
			// Get the transform of the main camera
			if (Camera.main != null) cam = Camera.main;
			else // Camera was not found.
			{
				Debug.LogWarning("Warning: no main camera found.\n" +
					"Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.",
					gameObject);
			}

			// Set start health attributes.
			health = StartHealth;
			HealthSlider.value = health;
			HealthSlider_Smoothed.value = health;

			// Set start magic attributes.
			magic = StartingMagic;
			MagicSlider.value = magic;
			MagicSlider_Smoothed.value = magic;

			// Add listeners for events.
			OnFootstep.AddListener(OnFootStep);
			OnJump.AddListener(OnJumpBegan);
			OnDoubleJump.AddListener(OnDoubleJumpBegan);
			OnLand.AddListener(OnLanded);
			isGrounded = true;
			_comboQueue = new Queue<int>(ComboQueueSize);

			// Add yield times here.
			hitStunYield = new WaitForSeconds(HitStunRenderToggleWait);
		}

		void OnEnable()
		{
			RegisterControls();
		}

		void RegisterControls()
		{
			// Set up new input system events.
			playerControls.Player.Move.performed += HandleMove;
			playerControls.Player.Move.Enable();

			playerControls.Player.Jump.performed += HandleJump;
			playerControls.Player.Jump.Enable();

			playerControls.Player.Aim.performed += HandleAim;
			playerControls.Player.Aim.Enable();

			playerControls.Player.Dodge.performed += HandleDodge;
			playerControls.Player.Dodge.Enable();

			playerControls.Player.Attack.performed += HandleAttack;
			playerControls.Player.Attack.Enable();

			playerControls.Player.HeavyAttack.performed += HandleHeavyAttack;
			playerControls.Player.HeavyAttack.Enable();

			playerControls.Player.Shoot.performed += HandleShoot;
			playerControls.Player.Shoot.Enable();

			playerControls.Player.Use.performed += HandleUse;
			playerControls.Player.Use.Enable();

			playerControls.Player.CameraChange.performed += HandleCameraChange;
			playerControls.Player.CameraChange.Enable();

			playerControls.Player.Ability.performed += HandleAbility;
			playerControls.Player.Ability.Enable();

			playerControls.Player.Pause.performed += HandlePause;
			playerControls.Player.Pause.Enable();
		}

		void OnDisable()
		{
			DeregisterControls();
		}

		void DeregisterControls()
		{
			// Deregister from new input system events.
			playerControls.Player.Move.performed -= HandleMove;
			playerControls.Player.Move.Disable();

			playerControls.Player.Jump.performed -= HandleJump;
			playerControls.Player.Jump.Disable();

			playerControls.Player.Aim.performed -= HandleAim;
			playerControls.Player.Aim.Disable();

			playerControls.Player.Dodge.performed -= HandleDodge;
			playerControls.Player.Dodge.Disable();

			playerControls.Player.Attack.performed -= HandleAttack;
			playerControls.Player.Attack.Disable();

			playerControls.Player.HeavyAttack.performed -= HandleHeavyAttack;
			playerControls.Player.HeavyAttack.Disable();

			playerControls.Player.Shoot.performed -= HandleShoot;
			playerControls.Player.Shoot.Disable();

			playerControls.Player.Use.performed -= HandleUse;
			playerControls.Player.Use.Disable();

			playerControls.Player.CameraChange.performed -= HandleCameraChange;
			playerControls.Player.CameraChange.Disable();

			playerControls.Player.Ability.performed -= HandleAbility;
			playerControls.Player.Ability.Disable();

			playerControls.Player.Pause.performed -= HandlePause;
			playerControls.Player.Pause.Disable();
		}

		#region InputActions
		/// <summary>
		/// Handles movement.
		/// </summary>
		void HandleMove(InputAction.CallbackContext context)
		{
			MoveAxis = context.ReadValue<Vector2>();
			//Debug.Log(context.ReadValue<Vector2> ());
		}

		void HandleJump(InputAction.CallbackContext context)
		{
			var value = context.ReadValue<float>();

			// Button held down.
			if (value == 1)
			{
				if (lastJumpValue != 1)
				{
					JumpAction();
				}
			}

			lastJumpValue = value;
		}

		void HandleAim(InputAction.CallbackContext context)
		{
			var value = context.ReadValue<float>();
			aimInput = value == 1 ? true : false;

			if (aimInput == false)
			{
				if (CameraLockOnController.Instance.lockedOn == true)
				{
					OnAimRelease.Invoke();
				}
			}
		}

		void HandleDodge(InputAction.CallbackContext context)
		{
			if (magic > dodgeMagicCost || unlimitedMagic)
			{
				// Bypass dodging if near scenery collider. That way we cannot pass through it.
				if (MoveAxis.sqrMagnitude > 0)
				{
					// Cannot dodge, show line of sight.
					if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, 3,
						dodgeLayerMask))
					{
						Debug.DrawRay(transform.position + new Vector3(0, 1, 0), transform.forward * 3, Color.red, 1);
						return;
					}
				}

				else // No movement

				{
					// Cannot dodge, show line of sight.
					if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), -transform.forward, 3,
						dodgeLayerMask))
					{
						Debug.DrawRay(transform.position + new Vector3(0, 1, 0), -transform.forward * 3, Color.red, 1);
						return;
					}
				}

				// Get dodge angle.
				// Assign to player animation.
				if (Time.time > nextDodge)
				{
					// Set dodge state.
					isDodging = true;
					playerAnim.SetBool("Dodging", true);
					playerAnim.SetTrigger("Dodge");

					// Update UI elements.
					magic -= dodgeMagicCost;
					PlayerUI.SetTrigger("Show");

					// Tweak movement amounts.
					moveMultiplier *= dodgeSpeedupFactor;
					animSpeedMultiplier *= dodgeSpeedupFactor;
					movingTurnSpeed *= dodgeSpeedupFactor;
					stationaryTurnSpeed *= dodgeSpeedupFactor;

					playerAnim.updateMode = AnimatorUpdateMode.UnscaledTime;

					// Set dodge time.
					dodgeTimeRemain = DodgeTimeDuration;
					TimescaleController.Instance.targetTimeScale = dodgeTimeScale;

					// Call events.
					OnDodgeBegan.Invoke();
					nextDodge = Time.time + dodgeRate;
				}
			}			
		}

		/// <summary>
		/// Updates dodge timing.
		/// </summary>
		void DodgeUpdate()
		{
			// Dodge time ran out.
			if (dodgeTimeRemain <= 0)
			{
				// If is dodging.
				if (isDodging == true)
				{
					// Game is not paused.
					if (GameController.Instance.isPaused == false)
					{
						TimescaleController.Instance.targetTimeScale = 1; // Reset time scale.

						// Reset dodging animation parameters.
						playerAnim.ResetTrigger("Dodge");
						playerAnim.SetBool("Dodging", false);
						playerAnim.updateMode = AnimatorUpdateMode.Normal;

						// Reset movement amounts.
						moveMultiplier /= dodgeSpeedupFactor;
						animSpeedMultiplier /= dodgeSpeedupFactor;
						movingTurnSpeed /= dodgeSpeedupFactor;
						stationaryTurnSpeed /= dodgeSpeedupFactor;

						// Call end dodge event.
						OnDodgeEnded.Invoke();
						isDodging = false;
					}
				}
			}

			else // There is dodge time.

			{
				// Game is not paused.
				if (GameController.Instance.isPaused == false)
				{
					// Decrease time left of dodging.
					dodgeTimeRemain -= Time.unscaledDeltaTime;

					Vector3 relativeDodgeDir = transform.InverseTransformDirection(
						transform.forward *
						(MoveAxis.sqrMagnitude > 0 ? 1 : -1) *
						dodgeSpeed * Time.unscaledDeltaTime);

					transform.Translate(relativeDodgeDir, Space.Self);

					playerAnim.SetBool("Dodging", true);
				}
			}
		}

		void HandleAttack(InputAction.CallbackContext context)
		{
			AttackAction();
		}

		void HandleHeavyAttack(InputAction.CallbackContext context)
		{
			HeavyAttackAction();
		}

		void HandleShoot(InputAction.CallbackContext context)
		{
			shootInput = context.ReadValue<float>() == 1 ? true : false;
		}

		void HandleUse(InputAction.CallbackContext context)
		{
			UseAction();
		}

		void HandleCameraChange(InputAction.CallbackContext context)
		{
			CameraChangeAction();
		}

		void HandleAbility(InputAction.CallbackContext context)
		{
			AbilityAction();
		}

		void HandlePause(InputAction.CallbackContext context)
		{
			if (lostAllHealth == false)
			{
				GameController.Instance.CheckPause();
			}
		}
		#endregion

		void Update ()
		{
			// Movement calculations.
			CalculateRelativeMoveDirection();
			ApplyMoveAndTurn();
		    ApplyExtraTurnRotation();

			// Actions.
			AimAction();
			ShootAction();
			MeleeActionUpdate();
			GetCameraChangeSmoothing();
			DodgeUpdate();

			// UI updates.
			CheckHealthSliders ();
			CheckMagicSliders ();
			CheckHealthMagicIsLow ();
        }

		void FixedUpdate ()
		{
			playerRb.velocity = new Vector3 (
				transform.forward.x * forwardAmount * moveSpeedMultiplier * Time.deltaTime,
				playerRb.velocity.y,
				transform.forward.z * forwardAmount * moveSpeedMultiplier * Time.deltaTime
			);

			GetBetterJumpVelocity();
			ClampVelocity (playerRb, terminalVelocity);
		}

		void OnCollisionEnter(Collision col)
		{
			// Check for walkable scenery layer.
			if (col.collider.gameObject.layer == 9)
			{
				OnLand.Invoke();
			}
		}

		private void OnCollisionStay(Collision col)
		{
			if (col.collider.gameObject.layer == 9)
			{
				isGrounded = true;
				playerAnim.SetBool("OnGround", isGrounded);
			}
		}

		#region Movement
		/// <summary>
		/// Calculates the relative move direction.
		/// </summary>
		void CalculateRelativeMoveDirection()
		{
			// Using self-relative controls.
			// Calculate camera relative direction to move.
			if (cam != null)
			{
				camForwardDirection = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;
				move = MoveAxis.y * camForwardDirection + MoveAxis.x * cam.transform.right;
			}

			else // Calculate move direction in world space.

			{
				// Use world-relative directions in the case of no main camera.
				move = MoveAxis.y * Vector3.forward + MoveAxis.x * Vector3.right;
			}
		}

		/// <summary>
		/// Applies movement and turning amounts.
		/// </summary>
		void ApplyMoveAndTurn()
		{
			move = transform.InverseTransformDirection(move);
			move = Vector3.ProjectOnPlane(move, groundNormal);
			turnAmount = Mathf.Atan2(move.x, move.z);
			forwardAmount = move.z;
		}

		/// <summary>
		/// Applies extra turn rotation when turning.
		/// </summary>
		void ApplyExtraTurnRotation()
	    {
	        // Help the character turn faster (this is in addition to root rotation in the animation).
	        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed,
	            forwardAmount);
	        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
	    }

		/// <summary>
		/// Clamps the velocity on a given Rigidbody.
		/// </summary>
		/// <param name="rb">Rb.</param>
		/// <param name="_terminalVelocity">Terminal velocity.</param>
		void ClampVelocity (Rigidbody rb, float _terminalVelocity)
		{
			rb.velocity = Vector3.ClampMagnitude (rb.velocity, _terminalVelocity);
		}

		#endregion

		#region Actions
		/// <summary>
		/// Action for jump.
		/// </summary>
		void JumpAction ()
		{
			isGrounded = false;
			playerAnim.SetBool("OnGround", isGrounded);

			// 0: none, 1: jump, 2: double jump
			if (jumpState < 2)
			{
				jumpState++;

				switch (jumpState)
				{
				case 0:
					isGrounded = false;
					break;

				case 1: // Jump.
						
					playerRb.velocity = new Vector3 (playerRb.velocity.x, jumpPower, playerRb.velocity.z);
					isGrounded = false;
					playerAnim.applyRootMotion = false;
					OnJump.Invoke ();

					break;

				case 2: // Double jump.

					// Override vertical velocity.
					playerRb.velocity = new Vector3 (playerRb.velocity.x, doubleJumpPower, playerRb.velocity.z);

					// Add forward force.
					playerRb.AddRelativeForce (0, 0, jumpPower_Forward, ForceMode.Acceleration);

					playerAnim.applyRootMotion = false;
					playerAnim.SetTrigger ("DoubleJump");

					OnDoubleJump.Invoke ();

					break;
				}
			}
			
		}

		/// <summary>
		/// Gets the better jump velocity.
		/// </summary>
		/// 
		void GetBetterJumpVelocity ()
		{
			// If we are falling.
			if (playerRb.velocity.y < 0)
			{
				playerRb.velocity += Vector3.up * Physics.gravity.y * (fallMult - 1) * Time.deltaTime;
			}

			// If we are moving up and not holding jump.
			else if (playerRb.velocity.y > 0 && lastJumpValue == 0)
			{
				playerRb.velocity += Vector3.up * Physics.gravity.y * (jumpPower - 1) * Time.deltaTime;
			}
		}
		
		/// <summary>
		/// Aim action.
		/// </summary>
		void AimAction ()
		{
			// Aiming.
			if (aimInput)
			{
				cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, aimFov, aimSmoothing * Time.deltaTime);
				camRigSimpleFollow.FollowRotSmoothTime = camRigSimpleFollowRotAiming;
			}

			else    // Not aiming.

			{
				cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, normalFov, aimSmoothing * Time.deltaTime);
				camRigSimpleFollow.FollowRotSmoothTime = camRigSimpleFollowRotNormal;
			}
		}

		/// <summary>
		/// Light attack melee action.
		/// </summary>
		void AttackAction()
		{
			if (aimInput)
			{
				return;
			}

			else // Not aiming.

			{
				if (_comboQueue.Count < ComboQueueSize)
				{
					_comboQueue.Enqueue(1);
				}
			}			
		}

		/// <summary>
		/// Heavy attack melee action.
		/// </summary>
		void HeavyAttackAction()
		{
			if (aimInput)
			{
				return;
			}

			else // Not aiming.

			{
				if (_comboQueue.Count < ComboQueueSize)
				{
					_comboQueue.Enqueue(2);
				}	
			}
		}

		/// <summary>
		/// Updates for combo.
		/// </summary>
		void MeleeActionUpdate()
		{
			if (isGrounded && !playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Dodging"))
			{
				if (_comboQueue.Count > 0 && _timePassed >= TimeBetweenDeQueue)
				{
					switch (_comboQueue.Dequeue())
					{
						case 1:
							// Light attack 
							playerAnim.SetTrigger("LiteAttack");
							break;
						case 2:
							// heavy attack 
							playerAnim.SetTrigger("HeavyAttack");
							break;
						default:
							Debug.Log("Combo Queue Error");
							break;

					}

					_timePassed = 0f;
				}

				_timePassed += Time.deltaTime;
			}

			else

			{
				_comboQueue.Clear();
				_timePassed = 0f;
			}
		}

		/// <summary>
		/// Shoot action.
		/// </summary>
		void ShootAction()
		{
			if (shootInput)
			{
				if (Time.time > nextFire && GameController.Instance.isPaused == false)
				{
					transform.rotation = Quaternion.LookRotation(AimNoPitchDir, Vector3.up);

					Shoot();
					nextFire = Time.time + currentFireRate;

					GameController.Instance.camShakeScript.shakeDuration = 1;
					GameController.Instance.camShakeScript.shakeAmount = 0.1f;
					GameController.Instance.camShakeScript.Shake();

					VibrateController.Instance.Vibrate(0.25f, 0.25f, 0.25f, 1);

					//Debug.Log ("Shooting from weapon " + currentWeaponIndex);
				}
			}
		}

		/// <summary>
		/// Use action.
		/// </summary>
		void UseAction ()
		{
			OnUse.Invoke ();
		}

		/// <summary>
		/// Camera change action.
		/// </summary>
		void CameraChangeAction ()
		{
			isRight = !isRight;
		}

		/// <summary>
		/// Gets the camera change smoothing.
		/// </summary>
		void GetCameraChangeSmoothing ()
		{
			// Is left.
			if (isRight == false)
			{
				CamFollow.localPosition = Vector3.Lerp (CamFollow.localPosition, 
					new Vector3 (cameraOffset.x, CamFollow.localPosition.y, CamFollow.localPosition.z), 
					cameraOffsetSmoothing * Time.deltaTime);

				return;
			} 

			else // Is right.

			{
				CamFollow.localPosition = Vector3.Lerp (CamFollow.localPosition,
					new Vector3 (cameraOffset.y, CamFollow.localPosition.y, CamFollow.localPosition.z),
					cameraOffsetSmoothing * Time.deltaTime);

				return;
			}
		}

		/// <summary>
		/// Ability action.
		/// </summary>
		void AbilityAction ()
		{
			Debug.Log ("Ability pressed.");
		}
		#endregion

		#region Health and Magic
		/// <summary>
		/// Checks the health slider values.
		/// </summary>
		void CheckHealthSliders ()
		{
			// Update health slider values.
			HealthSlider.value = Mathf.Clamp (health, 0, MaximumHealth);
			HealthSlider_Smoothed.value = Mathf.Lerp (HealthSlider_Smoothed.value, health, 
				healthSliderSmoothing * Time.deltaTime);

			// Update post process UI effects.
			float vignetteVal = -0.005f * HealthSlider.value + 0.3f;
			postProcessUIVolume.profile.GetSetting <Vignette> ().intensity.value = 
				vignetteVal * Mathf.Sin ((0.05f * (-health + MaximumHealth)) * Time.time); 

			postProcessUIVolume.profile.GetSetting <MotionBlur> ().shutterAngle.value = -3.6f * HealthSlider.value + 360;

			// Update main post process effects.
			SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting<ChromaticAberration> ().intensity.value = 
				-0.005f * HealthSlider.value + 0.5f;

			SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting<LensDistortion> ().intensity.value = 
				0.5f * HealthSlider.value - 50f;
			
			// On lost all health.
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

		/// <summary>
		/// Checks the magic slider values.
		/// </summary>
		void CheckMagicSliders ()
		{
			MagicSlider.value = Mathf.Clamp (magic, 0, MaximumMagic);
			MagicSlider_Smoothed.value = Mathf.Lerp (MagicSlider_Smoothed.value, magic, 
				magicSliderSmoothing * Time.deltaTime);
		}

		/// <summary>
		/// Checks the health magic is low. Shows UI if it is.
		/// </summary>
		void CheckHealthMagicIsLow ()
		{
			if (health < healthUIVisibilityThreshold || magic < magicUIVisibilityThreshold)
			{
				if (PlayerUI.GetBool ("Low") == false)
				{
					PlayerUI.SetBool ("Low", true);
				}
			} 

			else // Either is too low, show UI for it.

			{
				if (PlayerUI.GetBool ("Low") == true)
				{
					PlayerUI.SetBool ("Low", false);
				}
			}
		}
		#endregion

		#region Shooting
		/// <summary>
		/// Shoot.
		/// </summary>
		public void Shoot ()
		{
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
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

		#region Footsteps
		/// <summary>
		/// Gets the foot step sound from a list.
		/// </summary>
		private void GetFootStepSound ()
		{
			footstepSoundIndex = Random.Range (0, footstepClips.Length);
			footstepAudioSource.clip = footstepClips [footstepSoundIndex];
			footstepAudioSource.Play ();
		}

		/// <summary>
		/// Raises the foot step event.
		/// </summary>
		void OnFootStep ()
		{
			if (isGrounded == true)
			{
				GetFootStepSound();
			}
		}
		#endregion

		#region Jumping
		/// <summary>
		/// Gets the jump sound.
		/// </summary>
		public void GetJumpSound ()
		{
			jumpSoundIndex = Random.Range (0, jumpClips.Length);
			jumpAudioSource.clip = jumpClips [jumpSoundIndex];
			jumpAudioSource.Play ();
		}

		/// <summary>
		/// Raises the jump began event.
		/// </summary>
		void OnJumpBegan ()
		{
			GetJumpSound ();
		}

		/// <summary>
		/// Gets the double jump sound.
		/// </summary>
		public void GetDoubleJumpSound ()
		{
			doubleJumpSoundIndex = Random.Range (0, doubleJumpClips.Length);
			doubleJumpAudioSource.clip = doubleJumpClips [doubleJumpSoundIndex];
			doubleJumpAudioSource.Play ();
		}

		/// <summary>
		/// Raises the double jump began event.
		/// </summary>
		void OnDoubleJumpBegan ()
		{
			GetDoubleJumpSound ();
		}

		/// <summary>
		/// Gets the landing sound.
		/// </summary>
		public void GetLandingSound ()
		{
			landingSoundIndex = Random.Range (0, landingClips.Length);
			landingAudioSource.clip = landingClips [landingSoundIndex];
			landingAudioSource.Play ();
		}

		/// <summary>
		/// Raises the landed event.
		/// </summary>
		void OnLanded ()
		{
			jumpState = 0;
			isGrounded = true;
			playerAnim.SetBool("OnGround", isGrounded);
			playerAnim.applyRootMotion = true;
		}
		#endregion

		#region Physics
		/// <summary>
		/// When player hits death barrier, hard reset on positioning and movement.
		/// </summary>
		public void DeathBarrier ()
		{
			transform.position = startingPoint.position;
			playerRb.velocity = Vector3.zero;
		}
		#endregion

		#region Override
		/// <summary>
		/// Overrides the player position.
		/// </summary>
		/// <param name="newPos">New position.</param>
		public void OverridePlayerPosition (Transform newPos)
		{
			transform.position = newPos.position;
		}

		/// <summary>
		/// Overrides a Vector3 position.
		/// </summary>
		/// <param name="pos">Position.</param>
		public void OverridePosition (Vector3 pos)
		{
			this.transform.position = pos;
		}
		#endregion

		#region Hit stun
		/// <summary>
		/// Does a hit stun.
		/// </summary>
		public void DoHitStun ()
		{	
			if (isInHitStun == false)
			{
				StartCoroutine (Hitstun ());
			}
		}

		/// <summary>
		/// Hitstun player.
		/// </summary>
		public IEnumerator Hitstun ()
		{
			// Get current time.
			hitStunCurrentTime = Time.time;
			isInHitStun = true;
			OnHitStunBegin.Invoke ();

			// Toggle skinned mesh renderer enabled.
			while (Time.time < hitStunCurrentTime + hitStunDuration && health > 0)
			{
				for (int i = 0; i < stunMeshes.Length; i++)
				{
					stunMeshes [i].enabled = !stunMeshes [i].enabled;
				}

				yield return hitStunYield;
			}

			// Always re enable skinned mesh renderers.
			for (int i = 0; i < stunMeshes.Length; i++)
			{
				// Always end with skinned mesh renderers enabled.
				stunMeshes [i].enabled = true;
			}
				
			OnHitStunEnded.Invoke ();
			isInHitStun = false;
		}
		#endregion

		/// <summary>
		/// Enables the skinned meshes from skinned meshes list.
		/// </summary>
		public void EnableSkinnedMeshes ()
		{
			for (int i = 0; i < stunMeshes.Length; i++)
			{
				stunMeshes [i].enabled = true;
			}
		}
	}
}
