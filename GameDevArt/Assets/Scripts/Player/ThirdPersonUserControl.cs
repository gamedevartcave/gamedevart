using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
		public static ThirdPersonUserControl instance { get; private set; }

        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object

		public Camera m_Cam; // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward; // The current forward direction of the camera
		private Vector3 m_Move; // the world-relative desired move direction, calculated from camForward and user input.
		private bool m_Jump; // Jump state.
		[HideInInspector] public bool m_DoubleJump; // Double jump input state.
		[SerializeField] [ReadOnlyAttribute] public bool doubleJumped; // Double jump state.m_Rigidbody.velocity.z.
		[Space (10)]
		public Transform CamFollow; // Camera offset follow point.
		private bool isRight; // Camera horizontal offset toggle state.
		public Vector2 cameraOffset; // Camera horizontal offset values.
		public float cameraOffsetSmoothing = 5; // Smoothing between offsets.

		private PlayerActions playerActions;

		void Awake ()
		{
			instance = this;
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
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }
			
        private void Update()
        {
			// Jumping.
            if (!m_Jump)
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

			// Aiming.
			if (playerActions.Aim.Value > 0.5f)
			{
				m_Cam.fieldOfView = Mathf.Lerp (
					m_Cam.fieldOfView, 
					PlayerController.instance.aimFov, 
					PlayerController.instance.aimSmoothing * Time.deltaTime
				);

				if (PlayerController.instance.CrosshairObject.activeSelf == false)
				{
					PlayerController.instance.CrosshairObject.SetActive (true);
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

				if (PlayerController.instance.CrosshairObject.activeSelf == true)
				{
					PlayerController.instance.CrosshairObject.SetActive (false);
				}
			}
				
			// Shooting.
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
				}
			}

			// Using.
			if (playerActions.Use.WasPressed == true)
			{
				PlayerController.instance.OnUse.Invoke ();
			}

			// Camera change.
			if (playerActions.CameraChange.WasPressed)
			{
				isRight = !isRight;
			}
        }
			
        private void FixedUpdate()
        {
			float h = playerActions.Shoot.Value > 0.5f ? 0 : playerActions.Move.Value.x;
			float v = playerActions.Shoot.Value > 0.5f ? 0 : playerActions.Move.Value.y;

			bool crouch = playerActions.Crouch.IsPressed;

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
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
			m_Character.Move(m_Move, crouch, m_Jump, m_DoubleJump);
            m_Jump = false;
			m_DoubleJump = false;
        }

		void LateUpdate ()
		{
			GetCameraChangeSmoothing ();
		}

		void GetCameraChangeSmoothing ()
		{
			// Camera change = Left.
			if (!isRight)
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
			}

			// Camera change = Right.
			if (isRight)
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
			}
		}
    }
}
