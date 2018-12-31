using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace CityBashers
{
	public class CameraOverride : MonoBehaviour 
	{
		public Collider playerCol;
		public Camera playerCam;
		public Camera overrideCam;
		public float releaseDelay = 1;

		void OnTriggerEnter (Collider other)
		{
			if (other == playerCol)
			{
				if (IsInvoking ("ReleaseOverride") == true)
				{
					CancelInvoke ("ReleaseOverride");
				}

				PlayerController.instance.cam = overrideCam;
				playerCam.enabled = false;
				overrideCam.enabled = true;
			}
		}

		void OnTriggerExit (Collider other)
		{
			if (other == playerCol)
			{
				Invoke ("ReleaseOverride", releaseDelay);
			}
		}

		void ReleaseOverride ()
		{
			PlayerController.instance.cam = playerCam;
			playerCam.enabled = true;
			overrideCam.enabled = false;
		}
	}
}
