using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CityBashers
{
	public class CameraLockOnController : MonoBehaviour 
	{
		public static CameraLockOnController instance { get; private set; }
		[ReadOnlyAttribute] public bool lockedOn;
		public float lockOnRate = 0.3f;

		[Header ("Lock on points")]
		public int activeLockOnIndex;
		public WorldToScreenPoint target;
		public List<Transform> lockOnPoints;

		[Header ("Main camera")]
		public Transform cam;
		public SimpleLookAt camLookAt;
		public float resetRotationSmoothing = 5;

		[Header ("Camera rig")]
		public MouseLook mouseLook;
		public Transform cameraRig;
		public float lockOnPitchAngle = -40;

		[Header ("Lock on events")]
		public UnityEvent OnLockOnBegan;
		public UnityEvent OnLockOnRelease;

		public PlayerActions playerActions;

		void Awake ()
		{
			instance = this;
			this.enabled = false;
		}

		void Start ()
		{
			playerActions = InControlActions.instance.playerActions;
		}

		void Update ()
		{
			// While aiming.
			if (playerActions.Aim.Value > 0.5f)
			{
				// Pressed lock on button (left).
				if (playerActions.LockOnLeft.WasPressed)
				{	
					// Already locked on to something.
					if (lockedOn == true)
					{
						// There are lock on points.
						if (lockOnPoints.Count > 0)
						{
							// Move down a lock on index.
							if (activeLockOnIndex > 0)
							{
								activeLockOnIndex--;
							} 

							else // Go to last lock on index.

							{
								activeLockOnIndex = lockOnPoints.Count - 1;
							}
						}
					}

					else
					
					{
						// Uses last index.
					}

					SetLockOnPoint (); // Lock on to something.
				}

				// Pressed lock on button (right).
				if (playerActions.LockOnRight.WasPressed)
				{
					// Already locked on to something.
					if (lockedOn == true)
					{
						// There are lock on points.
						if (lockOnPoints.Count > 0)
						{
							// Move up a lock on index.
							if (activeLockOnIndex < lockOnPoints.Count - 1)
							{
								activeLockOnIndex++;
							} 

							else // Go to first lock on index.

							{
								activeLockOnIndex = 0;
							}
						}
					} 

					else
					
					{
						// Uses last index.
					}

					SetLockOnPoint (); // Lock on to something.
				}
			}

			else // Stopped aiming, stop lock on.
			
			{
				// On lock on release.
				if (lockedOn == true)
				{
					lockedOn = false;
					OnLockOnRelease.Invoke ();
				}

				if (camLookAt.enabled == true)
				{
					camLookAt.enabled = false;
					cameraRig.rotation = Quaternion.LookRotation (cam.transform.forward, Vector3.up);
				}

				// While not locked on.
				cam.localRotation = Quaternion.Slerp (
					cam.localRotation, 
					Quaternion.identity, 
					resetRotationSmoothing * Time.deltaTime
				);
			}	
		}

		void SetLockOnPoint ()
		{
			OnLockOnBegan.Invoke ();

			// Set pitch of camera rig so we don't aim from such a height based on camera rig angle.
			cameraRig.rotation = Quaternion.Euler (lockOnPitchAngle, cameraRig.rotation.y, cameraRig.rotation.z);

			// Use current lock on index to lock on to a target.
			camLookAt.LookAtPos  = lockOnPoints [activeLockOnIndex];
			target.WorldObject   = lockOnPoints [activeLockOnIndex];
			target.worldMeshRend = lockOnPoints [activeLockOnIndex].GetComponent<Renderer> ();

			if (camLookAt.enabled == false)
			{
				camLookAt.enabled = true; // Allow camera to look at locked on target.
			}

			if (lockedOn == false)
			{
				lockedOn = true;
			}
		}
	}
}
