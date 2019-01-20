using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Input;

namespace CityBashers
{
	[AddComponentMenu("Camera-Control/Mouse Look")]
	public class MouseLook : MonoBehaviour 
	{
		public static MouseLook Instance { get; private set; }
		public PlayerControls playerControls;
		[ReadOnly] public Vector2 LookAxis;

		public Vector2 sensitivity = new Vector2 (15, 15);

		// Minimum and Maximum values can be used to constrain the possible rotation
		public Vector2 minimum = new Vector2 (-360, 360);
		public Vector2 maximum = new Vector2 (-60, 60);

		public float rotationX = 0;
		public float rotationY = 0;
		[Range (0.0f, 1.0f)]
		public float rotYDeadZone = 0.25f;

		public float rotationZSensitivity = 1;
		public Vector2 rotationZBounds = new Vector2 (-10, 10);

		public Slider MouseSensitivitySlider;

		void Awake ()
		{
			Instance = this;
			DontDestroyOnLoadInit.Instance.OnInitialize.AddListener(OnInitialized);
			enabled = false;
		}

		void OnDestroy()
		{
			DeregisterControls();
		}

		/// <summary>
		/// Called when scene is fully initialized.
		/// </summary>
		void OnInitialized()
		{
			RegisterControls();
		}

		/// <summary>
		/// Registers input controls.
		/// </summary>
		void RegisterControls()
		{
			playerControls.Player.Look.performed += HandleLook;
			playerControls.Player.Look.Enable();
		}

		/// <summary>
		/// Deregisters input controls.
		/// </summary>
		void DeregisterControls()
		{
			playerControls.Player.Look.performed -= HandleLook;
			playerControls.Player.Look.Disable();
		}

		/// <summary>
		/// Handle Look input.
		/// </summary>
		/// <param name="context"></param>
		void HandleLook(InputAction.CallbackContext context)
		{
			LookAxis = context.ReadValue<Vector2>();

			// Could be used to optimize look code but as value will not change if no input changes, 
			// the camera rig will stop moving.
			//UpdateLook(); 
		}

		/// <summary>
		/// Sets the invert axis.
		/// </summary>
		/// <param name="invert">If set to <c>true</c> invert.</param>
		public void SetInvertAxis (bool invert)
		{
			SaveAndLoadScript.Instance.invertYAxis = invert;
		}

		/// <summary>
		/// Sets the mouse sensitivity multiplier.
		/// </summary>
		public void SetMouseSensitivityMultiplier ()
		{
			SaveAndLoadScript.Instance.MouseSensitivityMultplier = MouseSensitivitySlider.value;
		}

		/// <summary>
		/// Refreshes the mouse sensitivity value.
		/// </summary>
		public void RefreshMouseSensitivityValue ()
		{
			MouseSensitivitySlider.value = SaveAndLoadScript.Instance.MouseSensitivityMultplier;
		}

		/// <summary>
		/// To update the rotation of an object so the camera rig will smoothly follow it.
		/// </summary>
		void Update ()
		{
			// Horizontal rotation.
			rotationX = 
				transform.localEulerAngles.y + 
				LookAxis.x * sensitivity.x * SaveAndLoadScript.Instance.MouseSensitivityMultplier;

			// While aiming.
			if (PlayerController.Instance.aimInput == true)
			{
				// Don't use deadzone.
				// Vertical rotation.
				rotationY += 
					LookAxis.y * 
					(SaveAndLoadScript.Instance.invertYAxis ? -sensitivity.y : sensitivity.y) 
					* SaveAndLoadScript.Instance.MouseSensitivityMultplier;
			}

			// Not aiming.
			else
			{
				// Use deadzone.
				if (LookAxis.y > rotYDeadZone ||
				    LookAxis.y < -rotYDeadZone)
				{
					// Vertical rotation.
					rotationY += 
						LookAxis.y * 
						(SaveAndLoadScript.Instance.invertYAxis ? -sensitivity.y : sensitivity.y) 
						* SaveAndLoadScript.Instance.MouseSensitivityMultplier;
				}
			}

			// Vertical rotation.
			rotationY = Mathf.Clamp (rotationY, minimum.y, maximum.y);

			// Roll rotation.
			float rotationZ = PlayerController.Instance.aimInput ? 0 : 
				Mathf.Clamp (-LookAxis.x * rotationZSensitivity, rotationZBounds.x, rotationZBounds.y);
			
			// Assign rotation amounts to object.
			transform.localEulerAngles = new Vector3 (-rotationY, rotationX, rotationZ);
		}
	}
}