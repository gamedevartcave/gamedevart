using UnityEngine;
using UnityEngine.UI;

namespace CityBashers
{
	[AddComponentMenu("Camera-Control/Mouse Look")]
	public class MouseLook : MonoBehaviour 
	{
		public static MouseLook instance { get; private set; }

		public Vector2 sensitivity = new Vector2 (15, 15);

		// Minimum and Maximum values can be used to constrain the possible rotation
		public Vector2 minimum = new Vector2 (-360, 360);
		public Vector2 maximum = new Vector2 (-60, 60);

		public float rotationX = 0;
		public float rotationY = 0;
		[Range (0.0f, 1.0f)]
		public float rotYDeadZone = 0.25f;

		public Slider MouseSensitivitySlider;

		private PlayerActions playerActions;

		void Awake ()
		{
			instance = this;
			this.enabled = false;
		}

		void Start ()
		{
			playerActions = InControlActions.instance.playerActions;
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

		void Update ()
		{
			rotationX = 
				transform.localEulerAngles.y + 
				playerActions.CamRot.Value.x * sensitivity.x * SaveAndLoadScript.Instance.MouseSensitivityMultplier;

			// While aiming.
			if (playerActions.Aim.Value > 0.5f)
			{
				// Don't use deadzone.
				rotationY += 
					playerActions.CamRot.Value.y * 
					(SaveAndLoadScript.Instance.invertYAxis ? -sensitivity.x : sensitivity.y) 
					* SaveAndLoadScript.Instance.MouseSensitivityMultplier;
			}

			// Not aiming.
			if (playerActions.Aim.Value <= 0.5f)
			{
				// Use deadzone.
				if (playerActions.CamRot.Value.y > rotYDeadZone ||
				    playerActions.CamRot.Value.y < -rotYDeadZone)
				{
					rotationY += 
						playerActions.CamRot.Value.y * 
						(SaveAndLoadScript.Instance.invertYAxis ? -sensitivity.x : sensitivity.y);
				}
			}

			rotationY = Mathf.Clamp (rotationY, minimum.y, maximum.y);
			
			transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
		}
	}
}