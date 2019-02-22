using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Input;

namespace CityBashers
{
	public class CameraLockOnController : MonoBehaviour 
	{
		public static CameraLockOnController Instance { get; private set; }
		public PlayerControls playerControls;

		[ReadOnly] public bool lockedOn;
		public int activeLockOnIndex;
		public WorldToScreenPoint target;
		public List<Transform> lockOnPoints;
		public Transform cameraRig;
		public Transform cam;
		public float lockOnRotationSmoothing = 5;
		public float resetRotationSmoothing = 5;

		[Header ("Lock on events")]
		public UnityEvent OnLockOnBegan;
		public UnityEvent OnLockOnRelease;

		private float lockOnVal;

		void Awake()
		{
			Instance = this;
			enabled = false;
		}

		private void OnEnable()
		{
			playerControls.Player.LockOn.performed += HandleLockOn;
			playerControls.Player.LockOn.Enable();
		}

		private void OnDisable()
		{
			playerControls.Player.LockOn.performed -= HandleLockOn;
			playerControls.Player.LockOn.Disable();
		}

		void HandleLockOn(InputAction.CallbackContext context)
		{
			lockOnVal = context.ReadValue<float>();

			lockedOn = !lockedOn;

			if (lockedOn == true)
			{
				SetLockOnPoint();
			}

			else

			{
				OnAimRelease();
			}
		}

		/// <summary>
		/// Called when aim is released.
		/// </summary>
		void OnAimRelease()
		{
			OnLockOnRelease.Invoke();
			cameraRig.rotation = Quaternion.LookRotation(cam.transform.forward, Vector3.up);
		}

		void LateUpdate()
		{
			if (lockedOn == false)
			{
				// While not locked on.
				cam.localRotation = Quaternion.Slerp(
					cam.localRotation,
					Quaternion.identity,
					resetRotationSmoothing * Time.deltaTime
				);
			}

			else

			{
				//cameraRig.transform.LookAt(lockOnPoints[activeLockOnIndex]); // Has no smoothing.
				cameraRig.rotation = Quaternion.Slerp(
					cameraRig.rotation,
					Quaternion.LookRotation(lockOnPoints[activeLockOnIndex].transform.position - cameraRig.position),
					lockOnRotationSmoothing * Time.deltaTime
				);
			}
		}

		/// <summary>
		/// Sets lock on point.
		/// </summary>
		void SetLockOnPoint ()
		{
			OnLockOnBegan.Invoke ();

			// Use current lock on index to lock on to a target.
			target.WorldObject   = lockOnPoints [activeLockOnIndex];
			target.worldMeshRend = lockOnPoints [activeLockOnIndex].GetComponent<Renderer> ();			
		}
	}
}
