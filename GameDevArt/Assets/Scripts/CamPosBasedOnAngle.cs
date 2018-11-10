using UnityEngine;

[ExecuteInEditMode]
public class CamPosBasedOnAngle : MonoBehaviour
{
	public MouseLook mouseLookScript;
	public Transform Cam;
	public Transform Checker;

	public AnimationCurve yPos;
	public AnimationCurve zPos;

	public Vector2 angleRange;

	public float evalY;

	public float evalZ; 

	void Awake ()
	{
		angleRange = new Vector2 (mouseLookScript.minimumY, mouseLookScript.maximumY);

	}
		
	void Update ()
	{
		SetPosByRotation ();
	}

	void SetPosByRotation ()
	{
		// Get the local rotation.


		// Evaluate Y and Z
		evalY = yPos.Evaluate (Checker.transform.localEulerAngles.x);
		evalZ = zPos.Evaluate (Checker.transform.localEulerAngles.x);

		// Set new pos.
		Cam.transform.localPosition = new Vector3 (
			Cam.transform.localPosition.x,
			evalY,
			-evalZ
		);
	}

	public float Remap (float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
}
