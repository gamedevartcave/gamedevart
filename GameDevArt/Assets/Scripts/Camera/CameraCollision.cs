using UnityEngine;

namespace CityBashers
{
	public class CameraCollision : MonoBehaviour
	{
		public Transform ReferencePos;
		[ReadOnly] public bool isInRange;
		public Transform NormalPos;
		public Transform SafePos;
		public float smoothing = 10;
		public string layerCheckName = "Scenery";
		[ReadOnly] public bool locked;
		[ReadOnly] public float delayTimeRemaining_In;
		[ReadOnly] public float delayTimeRemaining_Out;
		public float delayTimeDuration_In = 1;
		public float delayTimeDuration_Out = 1;
		private Vector3 smoothvel;

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

		void Update()
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
					delayTimeRemaining_In = 0;
					delayTimeRemaining_Out = 0;
					locked = false;
				}
			}

			else

			{
				locked = true;
			}

			if (CameraLockOnController.Instance.lockedOn == true)
			{
				delayTimeRemaining_In = 0;
				delayTimeRemaining_Out = 0;
				isInRange = false;
				locked = false;
			}

			if (locked == false)
			{
				ReferencePos.position = Vector3.Lerp(
					ReferencePos.position,
					isInRange ? SafePos.position : NormalPos.position,
					Time.deltaTime * smoothing);
			}
		}
	}
}
