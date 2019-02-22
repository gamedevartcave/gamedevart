using UnityEngine;

public class CameraCollision : MonoBehaviour
{
	[ReadOnly] public bool isInRange;
	public Vector3 SafeLocalPos = new Vector3 (0, 0, 0);
	public Vector3 NormalLocalPos = new Vector3(0, 0, -2);
	public float smoothing = 10;
	public string layerCheckName = "Scenery";
	[ReadOnly] public bool locked;
	[ReadOnly] public float delayTimeRemaining_In;
	[ReadOnly] public float delayTimeRemaining_Out;
	public float delayTimeDuration_In = 1;
	public float delayTimeDuration_Out = 1;

	public void SetInRange(bool range)
	{
		isInRange = range;

		if (isInRange == true)
		{
			delayTimeRemaining_In = delayTimeDuration_In;
		}

		else

		{
			delayTimeRemaining_Out = delayTimeDuration_Out;
		}
	}

	void Update ()
	{
		if (delayTimeRemaining_In > 0)
		{
			delayTimeRemaining_In -= Time.deltaTime;
		}

		if (delayTimeRemaining_Out > 0)
		{
			delayTimeRemaining_Out -= Time.deltaTime;
		}

		if (delayTimeRemaining_In <= 0 && delayTimeRemaining_Out <= 0)
		{
			if (locked == true)
			{
				locked = false;
			}
		}

		else

		{
			locked = true;
		}

		if (locked == false)
		{
			transform.localPosition = Vector3.Lerp(
				transform.localPosition,
				isInRange ? SafeLocalPos : NormalLocalPos,
				Time.deltaTime * smoothing);
		}
	}
}
