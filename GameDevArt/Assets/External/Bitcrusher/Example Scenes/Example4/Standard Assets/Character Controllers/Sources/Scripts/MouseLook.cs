using UnityEngine;

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

		// Settings Manager.
		public void SetInvertAxis (bool invert)
		{
			SaveAndLoadScript.Instance.invertYAxis = invert;
		}

		void Update ()
		{
			rotationX = transform.localEulerAngles.y + playerActions.CamRot.Value.x * sensitivity.x;

			// While aiming.
			if (playerActions.Aim.Value > 0.5f)
			{
				// Don't use deadzone.
				rotationY += 
					playerActions.CamRot.Value.y * 
					(SaveAndLoadScript.Instance.invertYAxis ? -sensitivity.x : sensitivity.y);
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