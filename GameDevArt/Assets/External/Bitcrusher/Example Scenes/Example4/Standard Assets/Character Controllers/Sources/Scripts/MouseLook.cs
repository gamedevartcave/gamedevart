using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour 
{
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;

	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

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

		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody> () != null)
		{
			GetComponent<Rigidbody> ().freezeRotation = true;
		}
	}

	void Update ()
	{
		if (axes == RotationAxes.MouseXAndY && GameController.instance.isPaused == false)
		{
			float rotationX = transform.localEulerAngles.y + playerActions.CamRot.Value.x * sensitivityX;

			//rotationY += playerActions.CamRot.Value.y * 
			//	(playerActions.Aim.Value > 0.5f ? -sensitivityY : sensitivityY);

			// While aiming.
			if (playerActions.Aim.Value > 0.5f)
			{
				// Don't use deadzone.
				rotationY += playerActions.CamRot.Value.y * -sensitivityY;
			}

			// Not aiming.
			if (playerActions.Aim.Value <= 0.5f)
			{
				// Use deadzone.
				if (playerActions.CamRot.Value.y > rotYDeadZone ||
				    playerActions.CamRot.Value.y < -rotYDeadZone)
				{
					rotationY += playerActions.CamRot.Value.y * -sensitivityY;
				}
			}

			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
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