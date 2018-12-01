using UnityEngine;

namespace CityBashers
{
	[ExecuteInEditMode]
	public class SimpleLookAt : MonoBehaviour 
	{
		[Tooltip ("Position to look at.")]
		public Transform LookAtPos;
		public Vector3 Offset;
		[Tooltip ("How to look at the Transform.")]
		public lookType LookMethod;
		public enum lookType
		{
			LookTowards,
			LookAway
		}

		[Tooltip ("Find up direction instead of forward direction.")]
		public bool useUpDirection;
		public bool useSmoothing;
		public float SmoothingAmount;

		void LateUpdate ()
		{
			if (useSmoothing == true) 
			{
				Quaternion lookPos = Quaternion.LookRotation (LookAtPos.position - transform.position - Offset, Vector3.up);
				transform.rotation = Quaternion.Slerp (transform.rotation, lookPos, SmoothingAmount * Time.deltaTime);
				return;
			}

			if (useSmoothing == false) 
			{
				// Look towards.
				if (LookMethod == lookType.LookTowards && LookAtPos != null) 
				{
					transform.LookAt (
						LookAtPos.position, useUpDirection ? Vector3.up : transform.forward 
					);
				}

				// Look away.
				if (LookMethod == lookType.LookAway) 
				{
					transform.LookAt (
						LookAtPos.position, useUpDirection ? -Vector3.up : -transform.forward
					);
				}
			}
		}
	}
}