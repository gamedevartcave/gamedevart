using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraLockOnController : MonoBehaviour 
{
	public static CameraLockOnController instance { get; private set; }
	[ReadOnlyAttribute] public bool lockedOn;
	public float lockOnRate = 0.3f;
	public Transform cam;
	public SimpleLookAt camLookAt;
	public float resetRotationSmoothing = 5;
	public int activeLockOnIndex;
	public List<Transform> lockOnPoints;

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
		if (playerActions.Aim.Value > 0.5f)
		{
			if (playerActions.LockOnLeft.WasPressed)
			{
				if (lockedOn == true)
				{
					if (lockOnPoints.Count > 0)
					{
						if (activeLockOnIndex > 0)
						{
							activeLockOnIndex--;
						} 

						else

						{
							activeLockOnIndex = lockOnPoints.Count - 1;
						}
					}
				}

				else
				
				{
					// Uses last index.
				}

				SetLockOnPoint ();
			}

			if (playerActions.LockOnRight.WasPressed)
			{
				if (lockedOn == true)
				{
					if (lockOnPoints.Count > 0)
					{
						if (activeLockOnIndex < lockOnPoints.Count - 1)
						{
							activeLockOnIndex++;
						} 

						else

						{
							activeLockOnIndex = 0;
						}
					}
				} 

				else
				
				{
					// Uses last index.
				}

				SetLockOnPoint ();
			}
		}

		else
		
		{
			if (camLookAt.enabled == true)
			{
				camLookAt.enabled = false;
				lockedOn = false;
				OnLockOnRelease.Invoke ();
			}

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
		camLookAt.enabled = true;
		lockedOn = true;
		camLookAt.LookAtPos = lockOnPoints [activeLockOnIndex];
	}
}
