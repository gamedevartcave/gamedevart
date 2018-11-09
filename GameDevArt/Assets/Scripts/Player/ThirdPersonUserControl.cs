using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using InControl;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
		public Transform m_CamRigRotX;
		public Vector2 CamXRotBounds;
		public Transform m_CamRigRotY;
		public float rotateSens;
		public Transform m_Cam;                   // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
		private bool m_DoubleJump;
		[HideInInspector]public bool doubleJumped;


		public PlayerActions playerActions;


        private void Start()
        {
			playerActions = PlayerActions.CreateWithDefaultBindings ();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
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
            if (!m_Jump)
            {
                //m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
				m_Jump = playerActions.Jump.WasPressed;
		
            }

			if (m_Jump && doubleJumped == false)
			{
				//m_DoubleJump = CrossPlatformInputManager.GetButtonDown("Jump");
				m_DoubleJump = playerActions.Jump.WasPressed;

				if (playerActions.Jump.WasPressed)
				{
					doubleJumped = true;
				}
			}
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            //float h = CrossPlatformInputManager.GetAxis("Horizontal");
            //float v = CrossPlatformInputManager.GetAxis("Vertical");
            //bool crouch = Input.GetKey(KeyCode.C);

			float h = playerActions.Move.Value.x;
			float v = playerActions.Move.Value.y;

			float ch = playerActions.CamRot.Value.x;
			float cv = playerActions.CamRot.Value.y;

			//m_CamRigRotX.Rotate (cv * rotateSens, 0, 0, Space.World);
		
				
			//m_CamRigRotY.Rotate (0, ch * rotateSens, 0, Space.World);

			bool crouch = playerActions.Crouch.IsPressed;

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            }
            
			else
            
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
    }
}
