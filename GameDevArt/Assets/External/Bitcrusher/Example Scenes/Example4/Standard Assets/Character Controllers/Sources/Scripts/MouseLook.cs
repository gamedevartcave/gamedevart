using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour 
{
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;

	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	/// Minimum and Maximum values can be used to constrain the possible rotation/// 

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;
	public float rotYDeadZone = 0.25f;

	private PlayerActions playerActions;

	void Start ()
	{
		playerActions = InControlActions.instance.playerActions;
	}

	public void SetInvertAxis (bool invert)
	{
		SaveAndLoadScript.Instance.invertYAxis = invert;
	}

	void Update ()
	{
		if (axes == RotationAxes.MouseXAndY && GameController.instance.isPaused == false)
		{
			float rotationX = transform.localEulerAngles.y + playerActions.CamRot.Value.x * sensitivityX;

			// While aiming.
			if (playerActions.Aim.Value > 0.5f)
			{
				// Don't use deadzone.
				rotationY += 
					playerActions.CamRot.Value.y * 
					(SaveAndLoadScript.Instance.invertYAxis ? -sensitivityY : sensitivityY);
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
						(SaveAndLoadScript.Instance.invertYAxis ? -sensitivityY : sensitivityY);
				}
			}

			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
		}

		else 

		if (axes == RotationAxes.MouseX && GameController.instance.isPaused == false)
		{
			transform.Rotate (0, playerActions.CamRot.Value.x * sensitivityX, 0);
		}
		
		else
		
		{
			if (GameController.instance.isPaused == false)
			{
				rotationY += playerActions.CamRot.Value.y * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
				transform.localEulerAngles = new Vector3 (-rotationY, transform.localEulerAngles.y, 0);
			}
		}
	}
}