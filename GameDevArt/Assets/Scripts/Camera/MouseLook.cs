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

		public Slider MouseSensitivitySlider;

		void Awake ()
		{
			Instance = this;
			this.enabled = false;
		}

		private void OnEnable()
		{
			playerControls.Player.Look.performed += HandleLook;
			playerControls.Player.Look.Enable();
		}

		private void OnDisable()
		{
			playerControls.Player.Look.performed -= HandleLook;
			playerControls.Player.Look.Disable();
		}

		void HandleLook(InputAction.CallbackContext context)
		{
			LookAxis = context.ReadValue<Vector2>();
			UpdateLook();
			//Debug.Log("Mouse Delta: " + LookAxis);
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

		void UpdateLook ()
		{
			rotationX = 
				transform.localEulerAngles.y + 
				LookAxis.x * sensitivity.x * SaveAndLoadScript.Instance.MouseSensitivityMultplier;

			// While aiming.
			if (PlayerController.Instance.aimInput == true)
			{
				// Don't use deadzone.
				rotationY += 
					LookAxis.y * 
					(SaveAndLoadScript.Instance.invertYAxis ? -sensitivity.x : sensitivity.y) 
					* SaveAndLoadScript.Instance.MouseSensitivityMultplier;
			}

			// Not aiming.
			else
			{
				// Use deadzone.
				if (LookAxis.y > rotYDeadZone ||
				    LookAxis.y < -rotYDeadZone)
				{
					rotationY += 
						LookAxis.y * 
						(SaveAndLoadScript.Instance.invertYAxis ? -sensitivity.x : sensitivity.y);
				}
			}

			rotationY = Mathf.Clamp (rotationY, minimum.y, maximum.y);
			
			transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
		}
	}
}