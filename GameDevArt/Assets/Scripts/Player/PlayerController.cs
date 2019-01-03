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
		public float capsuleHeight;
		public Vector3 capsuleCenter;
		[HideInInspector] public CapsuleCollider playerCol;
		private Rigidbody playerRb;
		private Animator playerAnim;
		public Animator PlayerUI;
		public Transform startingPoint;

		[Header ("Movement")]
		[ReadOnlyAttribute]
		public Vector3 move; // The world-relative desired move direction, calculated from camForward and user input.
		private float hInput;
		private float vInput;

		public float movingTurnSpeed = 360;
		public float stationaryTurnSpeed = 180;
		public float moveMultiplier;

		public float runCycleLegOffset = 0.2f; // Specific to the character in sample assets, will need to be modified to work with others
		public float moveSpeedMultiplier = 1f;
		public float animSpeedMultiplier = 1f;

		private float origGroundCheckDistance;
		private float turnAmount;
		private float forwardAmount;
		public Vector3 groundNormal;

		[Header ("Health")]
		[ReadOnlyAttribute] 
		public bool lostAllHealth;
		public int health;
		public int StartHealth = 100;
		public int MaximumHealth = 100;
		public Slider HealthSlider;
		public Slider HealthSlider_Smoothed;
		public float healthSliderSmoothing;
		public UnityEvent OnLostAllHealth;
		public PostProcessVolume postProcessUIVolume;
		public float healthUIVisibilityThreshold = 50;

		[Header ("Magic")]
		public int magic;
		public int StartingMagic = 100;
		public int MaximumMagic = 100;
		public Slider MagicSlider;
		public Slider MagicSlider_Smoothed;
		public float magicSliderSmoothing;
		public float magicUIVisibilityThreshold = 50;

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
		public Camera cam; // A reference to the main camera in the scenes transform
		private Vector3 camForwardDirection; // The current forward direction of the camera
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
		//public TextMeshProUGUI DisplayedWeaponAmmoText;
		/*
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
		*/
		/*
		[Header ("Weapon changing")]
		[ReadOnlyAttribute] public bool isChangingWeapon;
		public float weaponChangeRate =  0.25f;
		private float nextWeaponChange;
		public float OnWeaponChangeTimeScale = 0.05f;
		[ReadOnlyAttribute] public float WeaponChangeModeTime = 1;
		public float WeaponChangeDuration = 1;
		public UnityEvent OnWeaponChange;
		public UnityEvent WeaponChangeEnded;
		*/
		
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
		public float jumpPower = 12f;
		public float jumpPower_Forward = 2f;
		[Space (10)]
		public float airControl = 5;
		public float gravityMultiplier = 2f;
		public AudioSource jumpAudioSource;
		public AudioClip[] jumpClips;
		private int jumpSoundIndex;
		[SerializeField] [ReadOnlyAttribute] public bool jump; // Jump state.
		[SerializeField] [ReadOnlyAttribute] public bool doubleJumped; // Double jump state playerRb.velocity.z
		public UnityEvent OnJump;

		[Header ("Double jumping")]
		public float doubleJumpPower = 1.5f;
		public AudioSource doubleJumpAudioSource;
		public AudioClip[] doubleJumpClips;
		private int doubleJumpSoundIndex;
		public UnityEvent OnDoubleJump;

		[Header ("Landing")]
		public float groundCheckDistance = 0.1f;
		public float terminalVelocity = 10;
		public float fallMult = 2.5f;
		[ReadOnlyAttribute] public bool isGrounded;
		public AudioSource landingAudioSource;
		public AudioClip[] landingClips;
		private int landingSoundIndex;
		public UnityEvent OnLand;

		[Header ("Dodging")]
		[ReadOnlyAttribute] public bool isDodging;
		[Tooltip ("Time between each dodge event.")]
		public float dodgeRate = 0.5f;
		[Tooltip ("Speed at which player moves while dodging.")]
		public float dodgeSpeed = 15;
		private float nextDodge;
		private float dodgeTimeRemain;
		[Tooltip ("How long in unscaled time the dodge time lasts.")]
		public float DodgeTimeDuration;
		[Tooltip ("The time scale during dodge time.")]
		public float dodgeTimeScale = 0.25f;
		[Tooltip ("Animator speed factor while in dodge mode.")]
		public float dodgeSpeedupFactor = 20;
		[Tooltip ("How much magic it costs per dodge.")]
		public int dodgeMagicCost = 10;
		[Tooltip ("Layers to look out for when checking colliders for a potential dodge.")]
		public LayerMask dodgeLayerMask;
		public UnityEvent OnDodgeBegan;
		public UnityEvent OnDodgeEnded;

		[Header ("Hit stun")]
		[ReadOnlyAttribute] public bool isInHitStun;
		public Renderer[] stunMeshes;
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

			for (int i = 0; i < stunMeshes.Length; i++)
			{
				stunMeshes [i].enabled = false;
			}

			this.enabled = false;
		}

		void Start ()
		{
			// Find some components.
			playerAnim = GetComponent<Animator> ();
			playerRb = GetComponent<Rigidbody> ();
			playerCol = GetComponent<CapsuleCollider> ();
			capsuleHeight = playerCol.height;
			capsuleCenter = playerCol.center;

			// Set Rigidbody constraints.
			playerRb.constraints = 
				RigidbodyConstraints.FreezeRotationX | 
				RigidbodyConstraints.FreezeRotationY | 
				RigidbodyConstraints.FreezeRotationZ;
			
			origGroundCheckDistance = groundCheckDistance; // Log original ground check distance.

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
			OnFootstep.AddListener (OnFootStep);
			OnJump.AddListener (OnJumpBegan);
			OnDoubleJump.AddListener (OnDoubleJumpBegan);
			OnLand.AddListener (OnLanded);

			// Add yield times here.
			hitStunYield = new WaitForSeconds (HitStunRenderToggleWait);

			// Add quick reference for InControl player actions.
			playerActions = InControlActions.instance.playerActions;
		}

		void Update ()
		{
			ReadMovementInput ();

			// Actions.
			JumpAction ();
			DodgeAction ();
			AimAction ();
			ShootAction ();
			MeleeAction ();
			UseAction ();
			CameraChangeAction ();
			AbilityAction ();

			// UI updates.
			CheckHealthSliders ();
			CheckMagicSliders ();
			CheckHealthMagicIsLow ();
		}

		void FixedUpdate ()
		{
			GetBetterJumpVelocity ();
			ClampVelocity (playerRb, terminalVelocity);
			CalculateRelativeMoveDirection ();
	
			// Pass all parameters to the character control script.
			Move (move, false, jump, doubleJumped);
		}

		#region Movement

		/// <summary>
		/// Reads input movement.
		/// </summary>
		void ReadMovementInput ()
		{
			// Read input shorthand.
			hInput = 
				//playerActions.Shoot.Value > 0.5f ? 0 : 
				playerActions.Move.Value.x;
			vInput = 
				//playerActions.Shoot.Value > 0.5f ? 0 : 
				playerActions.Move.Value.y;
		}

		/// <summary>
		/// Calculates the relative move direction.
		/// </summary>
		void CalculateRelativeMoveDirection ()
		{
			// Using self-relative controls.
			// Calculate camera relative direction to move.
			if (cam != null)
			{
				camForwardDirection = Vector3.Scale (cam.transform.forward, new Vector3 (1, 0, 1)).normalized;
				move = vInput * camForwardDirection + hInput * cam.transform.right;
			}

			else // Calculate move direction in world space.

			{
				// Use world-relative directions in the case of no main camera.
				move = vInput * Vector3.forward + hInput * Vector3.right;
			}
		}

		/// <summary>
		/// Move for the specified move, crouch, jump and doubleJump.
		/// </summary>
		/// <param name="move">Move.</param>
		/// <param name="crouch">If set to <c>true</c> crouch.</param>
		/// <param name="jump">If set to <c>true</c> jump.</param>
		/// <param name="doubleJump">If set to <c>true</c> double jump.</param>
		public void Move (Vector3 move, bool crouch, bool jump, bool doubleJump)
		{
			// Vonvert the world relative moveInput vector into a local-relative
			// Turn amount and forward amount required to head in the desired direction.
			if (move.sqrMagnitude > 1f) move.Normalize ();
			move = transform.InverseTransformDirection (move);
			move = Vector3.ProjectOnPlane (move, groundNormal);

			// Update turning.
			turnAmount = Mathf.Atan2 (move.x, move.z);
			ApplyExtraTurnRotation ();
			forwardAmount = move.z;
	
			// Check ground status.
			CheckGroundStatus ();

			// Control and velocity handling is different when grounded and airborne.
			if (isGrounded == true)
			{
				if (doubleJumped == true) doubleJumped = false;
			}

			else // Is airborne.

			{
				HandleAirborneMovement (doubleJump);
			}

			// Send input and other state parameters to the animator
			UpdateAnimator (move);
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

		/// <summary>
		/// Updates the animator.
		/// </summary>
		/// <param name="move">Move.</param>
		void UpdateAnimator (Vector3 move)
		{
			// Calculate which leg is behind, so as to leave that leg trailing in the jump animation.
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle = Mathf.Repeat (
				playerAnim.GetCurrentAnimatorStateInfo (0).normalizedTime + runCycleLegOffset, 1);

			float jumpLeg = (runCycle < 0.5f ? 1 : -1) * forwardAmount;

			// Update the animator parameters.
			playerAnim.SetFloat ("Forward", forwardAmount, 0.1f, Time.deltaTime);
			playerAnim.SetFloat ("Turn", turnAmount, 0.1f, Time.deltaTime);

			// Update grounded state.
			playerAnim.SetBool ("OnGround", isGrounded);

			if (isGrounded == true)
			{
				playerAnim.SetFloat ("JumpLeg", jumpLeg);
				playerAnim.SetFloat ("Jump", 0);

				CamPosBasedOnAngle.instance.offset = new Vector2 (
					CamPosBasedOnAngle.instance.offset.x, 
					Mathf.Lerp (CamPosBasedOnAngle.instance.offset.y, 0, 2 * Time.deltaTime) 
				);
			} 

			else // Is in mid air.

			{
				playerAnim.SetFloat ("Jump", playerRb.velocity.y);

				CamPosBasedOnAngle.instance.offset = new Vector2 (
					CamPosBasedOnAngle.instance.offset.x, 
					Mathf.Lerp (CamPosBasedOnAngle.instance.offset.y, Mathf.Min (playerRb.velocity.y * CamPosBasedOnAngle.instance.offsetMult, 0), 2 * Time.deltaTime)
				);
			}
				
			// The anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// Which affects the movement speed because of the root motion.
			if (playerAnim.GetBool ("Dodging") == false)
			{
				if (isGrounded == true && move.sqrMagnitude > 0)
				{
					playerAnim.speed = animSpeedMultiplier;
				}

				else // Don't use anim speed while airborne.

				{
					playerAnim.speed = 1;
				}
			}
		}

		/// <summary>
		/// Handles the airborne movement.
		/// </summary>
		/// <param name="doubleJump">If set to <c>true</c> double jump.</param>
		void HandleAirborneMovement (bool doubleJump)
		{
			// Apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
			playerRb.AddForce (extraGravityForce);

			// Handle air control.
			float airControlForce = airControl * playerActions.Move.Value.magnitude;
			playerRb.AddRelativeForce (
				new Vector3 (0, 0, Mathf.Abs (airControlForce)),
				ForceMode.Acceleration);

			// Check ground distance based on velocity.
			groundCheckDistance = playerRb.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
		}

		/// <summary>
		/// Applies extra turn rotation when turning.
		/// </summary>
		void ApplyExtraTurnRotation ()
		{
			// Help the character turn faster (this is in addition to root rotation in the animation).
			float turnSpeed = Mathf.Lerp (stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate (0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}

		/// <summary>
		/// Essential for movement.
		/// </summary>
		public void OnAnimatorMove ()
		{
			// Overrides the default root motion.
			// Allows us to modify the positional speed before it's applied.
			if (isGrounded == true && Time.deltaTime > 0)
			{
				Vector3 v = (playerAnim.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

				// Preserve the existing y part of the current velocity.
				v.y = playerRb.velocity.y;
				playerRb.velocity = v;
			}
		}

		/// <summary>
		/// Checks the current ground status.
		/// </summary>
		void CheckGroundStatus ()
		{
			RaycastHit hitInfo;

			#if UNITY_EDITOR
			Debug.DrawRay (transform.position, Vector3.down * groundCheckDistance, Color.yellow);
			#endif

			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast (transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
			{
				// Get grounded.
				if (isGrounded == false && jump == true)
				{
					jump = false;
					OnLand.Invoke ();
					playerAnim.applyRootMotion = true;
				}

				isGrounded = true;
			}

			else // Is in air.

			{
				isGrounded = false;
				playerAnim.applyRootMotion = false;
			}
		}

		#endregion

		#region Actions
		/// <summary>
		/// Action for jump.
		/// </summary>
		void JumpAction ()
		{
			if (playerActions.Jump.WasPressed == true)
			{
				// Is already in jump state.
				if (jump == true)
				{
					// jump was pressed.
					if (playerActions.Jump.WasPressed == true)
					{
						// has not double jumped while in air.
						if (doubleJumped == false && isGrounded == false)
						{
							doubleJumped = true;

							// Override vertical velocity.
							playerRb.velocity = new Vector3 (playerRb.velocity.x, doubleJumpPower, playerRb.velocity.z);

							// Add forward force.
							playerRb.AddRelativeForce (0, 0, jumpPower_Forward, ForceMode.Acceleration);

							playerAnim.applyRootMotion = false;
							groundCheckDistance = 0.1f;

							OnDoubleJump.Invoke ();
						}
					}
				} 

				else // Is not already in jump state.
				
				{
					jump = true;
					playerRb.velocity = new Vector3 (playerRb.velocity.x, jumpPower, playerRb.velocity.z);
					isGrounded = false;
					playerAnim.applyRootMotion = false;
					OnJump.Invoke ();
				}
			}
		}

		/// <summary>
		/// Gets the better jump velocity.
		/// </summary>
		void GetBetterJumpVelocity ()
		{
			// If we are falling.
			if (playerRb.velocity.y < 0)
			{
				playerRb.velocity += Vector3.up * Physics.gravity.y * (fallMult - 1) * Time.deltaTime;
			}

			// If we are moving up and not holding jump.
			else if (playerRb.velocity.y > 0 && InControlActions.instance.playerActions.Jump.IsPressed == false)
			{
				playerRb.velocity += Vector3.up * Physics.gravity.y * (jumpPower - 1) * Time.deltaTime;
			}
		}

		/// <summary>
		/// Dodges action.
		/// </summary>
		void DodgeAction ()
		{
			if (magic > dodgeMagicCost &&
				(playerActions.DodgeLeft.WasPressed || playerActions.DodgeRight.WasPressed))
			{
				// Bypass dodging if near scenery collider. That way we cannot pass through it.
				if (playerActions.Move.Value.sqrMagnitude > 0)
				{
					if (Physics.Raycast (transform.position + new Vector3 (0, 1, 0), transform.forward, 3, dodgeLayerMask))
					{
						Debug.DrawRay (transform.position + new Vector3 (0, 1, 0), transform.forward * 3, Color.red, 1);
						return;
					}
				} 

				else // Not moving, check backwards.
				
				{
					if (Physics.Raycast (transform.position + new Vector3 (0, 1, 0), -transform.forward, 3, dodgeLayerMask))
					{
						Debug.DrawRay (transform.position + new Vector3 (0, 1, 0), transform.forward * 3, Color.red, 1);
						return;
					}

					transform.eulerAngles = new Vector3 (
						transform.eulerAngles.x, 
						transform.eulerAngles.y + 180, 
						transform.eulerAngles.z);
				}

				// Get dodge angle.
				// Assign to player animation.
				if (Time.time > nextDodge)
				{
					isDodging = true;

					playerAnim.SetFloat ("DodgeDir", playerActions.Dodge.Value);
					playerAnim.SetTrigger ("Dodge");
					playerAnim.SetBool ("Dodging", true);
					magic -= dodgeMagicCost;
					PlayerUI.SetTrigger ("Show");

					moveMultiplier *= dodgeSpeedupFactor;
					animSpeedMultiplier *= dodgeSpeedupFactor;
					movingTurnSpeed *= dodgeSpeedupFactor;
					stationaryTurnSpeed *= dodgeSpeedupFactor;

					playerAnim.updateMode = AnimatorUpdateMode.UnscaledTime;

					dodgeTimeRemain = DodgeTimeDuration;
					TimescaleController.instance.targetTimeScale = dodgeTimeScale;

					OnDodgeBegan.Invoke ();
					nextDodge = Time.time + dodgeRate;

					//Debug.Log ("Dodged " + playerActions.Dodge.Value);
				}

				else // Not able to dodge yet.

				{
				}
			}

			// Dodge time ran out.
			if (dodgeTimeRemain <= 0)
			{
				// If is dodging.
				if (isDodging == true)
				{
					// Game is not paused.
					if (GameController.instance.isPaused == false)
					{
						TimescaleController.instance.targetTimeScale = 1; // Reset time scale.

						// Reset dodging animation parameters.
						playerAnim.SetFloat ("DodgeDir", 0);
						playerAnim.ResetTrigger ("Dodge");
						playerAnim.SetBool ("Dodging", false);

						moveMultiplier /= dodgeSpeedupFactor;
						animSpeedMultiplier /= dodgeSpeedupFactor;
						movingTurnSpeed /= dodgeSpeedupFactor;
						stationaryTurnSpeed /= dodgeSpeedupFactor;

						playerAnim.updateMode = AnimatorUpdateMode.Normal;

						OnDodgeEnded.Invoke ();
						isDodging = false;
					}
				}
			} 

			else // There is dodge time.

			{
				// Game is not paused.
				if (GameController.instance.isPaused == false)
				{
					// Decrease time left of dodging.
					dodgeTimeRemain -= Time.unscaledDeltaTime;

					Vector3 relativeDodgeDir = transform.InverseTransformDirection (
						transform.forward *
						(playerActions.Move.Value.sqrMagnitude > 0 ? 1 : -1) * 
						dodgeSpeed * Time.unscaledDeltaTime);

					transform.Translate (relativeDodgeDir, Space.Self);

					playerAnim.SetBool ("Dodging", true);
				}
			}
		}

		/// <summary>
		/// Aim action.
		/// </summary>
		void AimAction ()
		{
			// Aiming.
			if (playerActions.Aim.Value > 0.5f)
			{
				cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, aimFov, aimSmoothing * Time.deltaTime);
				camRigSimpleFollow.FollowRotSmoothTime = camRigSimpleFollowRotAiming;
			}

			// Not aiming.
			if (playerActions.Aim.Value <= 0.5f)
			{
				cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, normalFov, aimSmoothing * Time.deltaTime);
				camRigSimpleFollow.FollowRotSmoothTime = camRigSimpleFollowRotNormal;
			}
		}

		/// <summary>
		/// Shoot action.
		/// </summary>
		void ShootAction ()
		{
			if (playerActions.Shoot.Value > 0.5f)
			{
				if (Time.time > nextFire && GameController.instance.isPaused == false)
				{
					transform.rotation = Quaternion.LookRotation (AimNoPitchDir, Vector3.up);

					Shoot ();
					nextFire = Time.time + currentFireRate;

					GameController.instance.camShakeScript.shakeDuration = 1;
					GameController.instance.camShakeScript.shakeAmount = 0.1f;
					GameController.instance.camShakeScript.Shake ();

					VibrateController.instance.Vibrate (0.25f, 0.25f, 0.25f, 1);

					Debug.Log ("Attacking from weapon " + currentWeaponIndex);
				}
			}
		}

		/// <summary>
		/// Melee action.
		/// </summary>
		void MeleeAction ()
		{
			if (playerActions.Melee.WasPressed)
			{
				// TODO: Make combos.

				//Debug.Log ("Melee action was pressed.");
			}
		}

		/// <summary>
		/// Weapon change action.
		/// </summary>
		/*
		void WeaponChangeAction ()
		{
			if (playerActions.NextWeapon.IsPressed == true && 
				GameController.instance.isPaused == false)
			{
				if (Time.unscaledTime > nextWeaponChange && GameController.instance.isPaused == false)
				{
					// Change to next weapon.
					if (currentWeaponIndex < Weapons.Length - 1)
					{
						currentWeaponIndex++;
					} 

					else

					{
						currentWeaponIndex = 0;
					}
				}

				isChangingWeapon = true;
				SetWeaponIndex (currentWeaponIndex, true);
				OnWeaponChange.Invoke ();

				WeaponChangeModeTime = WeaponChangeDuration;
				TimescaleController.instance.targetTimeScale = OnWeaponChangeTimeScale;
				nextWeaponChange = Time.unscaledTime + weaponChangeRate;
			}

			if (playerActions.PreviousWeapon.IsPressed == true && 
				GameController.instance.isPaused == false)
			{
				if (Time.unscaledTime > nextWeaponChange && GameController.instance.isPaused == false)
				{
					// Change to previous weapon.
					if (currentWeaponIndex > 0)
					{
						currentWeaponIndex--;
					} 

					else

					{
						currentWeaponIndex = Weapons.Length - 1;
					}
				}

				isChangingWeapon = true;
				SetWeaponIndex (currentWeaponIndex, false);
				OnWeaponChange.Invoke ();

				WeaponChangeModeTime = WeaponChangeDuration;
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
		*/

		/// <summary>
		/// Use action.
		/// </summary>
		void UseAction ()
		{
			if (playerActions.Use.WasPressed == true)
			{
				OnUse.Invoke ();
			}
		}

		/// <summary>
		/// Camera change action.
		/// </summary>
		void CameraChangeAction ()
		{
			if (playerActions.CameraChange.WasPressed)
			{
				isRight = !isRight;
			}

			GetCameraChangeSmoothing ();
		}

		/// <summary>
		/// Gets the camera change smoothing.
		/// </summary>
		void GetCameraChangeSmoothing ()
		{
			if (isRight == false)
			{
				CamFollow.localPosition = Vector3.Lerp (CamFollow.localPosition, 
					new Vector3 (cameraOffset.x, CamFollow.localPosition.y, CamFollow.localPosition.z), 
					cameraOffsetSmoothing * Time.deltaTime);

				return;
			} 

			else

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
			if (playerActions.Ability.WasPressed)
			{
				Debug.Log ("Ability pressed.");
			}
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

			// Update regular post process effects.
			//SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting<ColorGrading> ().mixerGreenOutGreenIn.value =
			//	Mathf.Clamp (3 * HealthSlider.value, 0, 100);
			
			//SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting<ColorGrading> ().mixerBlueOutBlueIn.value =
			//	Mathf.Clamp (3 * HealthSlider.value, 0, 100);

			SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting<ChromaticAberration> ().intensity.value = 
				-0.005f * HealthSlider.value + 0.5f;

			SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting<LensDistortion> ().intensity.value = 
				0.5f * HealthSlider.value - 50f;
			
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

		#region Weapons
		/// <summary>
		/// Sets the current weapon index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="reverse">If set to <c>true</c> reverse.</param>
		/*
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
		*/
		#endregion

		#region Footsteps
		/// <summary>
		/// Gets the foot step sound from a list.
		/// </summary>
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

		/// <summary>
		/// Raises the foot step event.
		/// </summary>
		void OnFootStep ()
		{
			GetFootStepSound ();
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
