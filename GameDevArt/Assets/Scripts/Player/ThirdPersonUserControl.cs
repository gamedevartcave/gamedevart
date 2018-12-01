using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Events;
using System.Collections;
using CityBashers;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
		public static ThirdPersonUserControl instance { get; private set; }
        
		private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
		private Vector3 m_Move; // the world-relative desired move direction, calculated from camForward and user input.

		[Header ("Weapon changing")]
		[ReadOnlyAttribute] public bool isChangingWeapon;
		public float weaponChangeRate =  0.25f;
		private float nextWeaponChange;
		public float OnWeaponChangeTimeScale = 0.05f;
		[ReadOnlyAttribute] public float WeaponChangeModeTime = 1;
		public float WeaponChangeDuration = 1;
		public UnityEvent OnWeaponChange;
		public UnityEvent WeaponChangeEnded;

		[Header ("Camera rig")]
		public SimpleFollow camRigSimpleFollow;
		public Vector3 camRigSimpleFollowRotNormal = new Vector3 (5, 15, 0);
		public Vector3 camRigSimpleFollowRotAiming;
		public MouseLook mouseLook;
		public Camera m_Cam; // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward; // The current forward direction of the camera
		public Animator CrosshairAnim;

		private bool m_Jump; // Jump state.
		[HideInInspector] public bool m_DoubleJump; // Double jump input state.
		[SerializeField] [ReadOnlyAttribute] public bool doubleJumped; // Double jump state.m_Rigidbody.velocity.z.

		[Header ("Camera offset")]
		public Transform CamFollow; // Camera offset follow point.
		private bool isRight; // Camera horizontal offset toggle state.
		public Vector2 cameraOffset; // Camera horizontal offset values.
		public float cameraOffsetSmoothing = 5; // Smoothing between offsets.

		[Header ("Dodging")]
		[ReadOnlyAttribute] public bool isDodging;
		public float dodgeRate = 0.5f;
		public float dodgeSpeed = 15;
		private float nextDodge;
		private float dodgeTimeRemain;
		public float DodgeTimeDuration;
		public float dodgeTimeScale = 0.25f;
		public UnityEvent OnDodgeBegan;
		public UnityEvent OnDodgeEnded;

		private PlayerActions playerActions;

		void Awake ()
		{
			instance = this;
			this.enabled = false;
		}

        private void Start()
        {
			playerActions = InControlActions.instance.playerActions;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

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

            // get the third person character.
			// This should never be null due to require component.
            m_Character = GetComponent<ThirdPersonCharacter>();
        }
			
        private void Update()
        {
			JumpAction ();

			AimAction ();
				
			ShootAction ();

			MeleeAction ();

			WeaponChangeAction ();

			UseAction ();

			CameraChangeAction ();

			DodgeAction ();

			AbilityAction ();
        }
			
        private void FixedUpdate ()
        {
			float h = playerActions.Shoot.Value > 0.5f ? 0 : playerActions.Move.Value.x;
			float v = playerActions.Shoot.Value > 0.5f ? 0 : playerActions.Move.Value.y;

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
			//m_Character.Move(m_Move, crouch, m_Jump, m_DoubleJump);
			m_Character.Move (m_Move, false, m_Jump, m_DoubleJump);
            m_Jump = false;
			m_DoubleJump = false;
        }
			
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

				if (CrosshairAnim.GetCurrentAnimatorStateInfo (0).IsName ("CrosshairOut") == true)
				{
					CrosshairAnim.ResetTrigger ("Out");
					CrosshairAnim.SetTrigger ("In");
					//mouseLook.rotationY = 0;
				}
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

				if (CrosshairAnim.GetCurrentAnimatorStateInfo (0).IsName ("CrosshairIn") == true)
				{
					CrosshairAnim.ResetTrigger ("In");
					CrosshairAnim.SetTrigger ("Out");
				}
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
			if (playerActions.NextWeapon.IsPressed == true)
			{
				if (Time.time > nextWeaponChange && GameController.instance.isPaused == false)
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
				PlayerController.instance.SetWeaponIndex (PlayerController.instance.currentWeaponIndex);
				OnWeaponChange.Invoke ();
				TimescaleController.instance.targetTimeScale = OnWeaponChangeTimeScale;
				nextWeaponChange = Time.unscaledTime + weaponChangeRate;
			}

			if (playerActions.PreviousWeapon.IsPressed == true)
			{
				if (Time.time > nextWeaponChange && GameController.instance.isPaused == false)
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
				PlayerController.instance.SetWeaponIndex (PlayerController.instance.currentWeaponIndex);
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

		void DodgeAction ()
		{
			if (playerActions.DodgeLeft.WasPressed || playerActions.DodgeRight.WasPressed)
			{
				// Get dodge value to determine left or right dodge.
				// Assign to player animation.

				if (Time.time > nextDodge)
				{
					isDodging = true;
					m_Character.m_Animator.SetFloat ("DodgeDir", playerActions.Dodge.Value);
					m_Character.m_Animator.SetTrigger ("Dodge");
					m_Character.m_Animator.SetBool ("Dodging", true);
					m_Character.moveMultiplier *= 10;
					m_Character.m_AnimSpeedMultiplier *= 10;
					m_Character.m_MovingTurnSpeed *= 10;
					m_Character.m_StationaryTurnSpeed *= 10;
					m_Character.m_Animator.updateMode = AnimatorUpdateMode.UnscaledTime;

					dodgeTimeRemain = DodgeTimeDuration;
					TimescaleController.instance.targetTimeScale = dodgeTimeScale;

					OnDodgeBegan.Invoke ();
					nextDodge = Time.time + dodgeRate;
					Debug.Log ("Dodged " + playerActions.Dodge.Value);
				}

				else // Not able to dodge yet.

				{
					//m_Character.m_Animator.SetFloat ("DodgeDir", 0);
					//m_Character.m_Animator.ResetTrigger ("Dodge");
					//m_Character.m_Animator.SetBool ("Dodging", false);
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
						m_Character.m_Animator.SetFloat ("DodgeDir", 0);
						m_Character.m_Animator.ResetTrigger ("Dodge");
						m_Character.m_Animator.SetBool ("Dodging", false);

						m_Character.moveMultiplier /= 10;
						m_Character.m_AnimSpeedMultiplier /= 10;
						m_Character.m_MovingTurnSpeed /= 10;
						m_Character.m_StationaryTurnSpeed /= 10;
						m_Character.m_Animator.updateMode = AnimatorUpdateMode.Normal;

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
						transform.InverseTransformDirection (Camera.main.transform.right * m_Character.m_Animator.GetFloat ("DodgeDir") * dodgeSpeed * Time.unscaledDeltaTime);

					transform.Translate (relativeDodgeDir, Space.Self);

					m_Character.m_Animator.SetBool ("Dodging", true);
				}
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
    }
}
