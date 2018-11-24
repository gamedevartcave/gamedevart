using UnityEngine;

public class SimpleFollow : MonoBehaviour 
{
	public UpdateMethod updateMethod;
	public enum UpdateMethod
	{
		Update = 0,
		FixedUpdate = 1,
		LateUpdate = 2
	}

	[Header ("Position")]
	public bool FollowPosition;
	[Space (10)]
	public bool AutomaticallyFindPlayerPosObject;
	public string LookForPosName = "Player";
	public Transform OverrideTransform;
	[Space (10)]
	public Transform FollowPosX;
	public Transform FollowPosY;
	public Transform FollowPosZ;

	public followPosMethod FollowPosMethod;
	public enum followPosMethod
	{
		Lerp,
		SmoothDamp,
		NoSmoothing
	}

	private float FollowPosVelX, FollowPosVelY, FollowPosVelZ;
	public Vector3 FollowPosOffset;
	public Vector3 FollowPosSmoothTime;

	private float xPos;
	private float yPos;
	private float zPos;

	[Space (10)]
	public bool XPosTimeUnscaled = true;
	public bool YPosTimeUnscaled = true;
	public bool ZPosTimeUnscaled = true;
	[Space (10)]
	public Vector2 PosBoundsX;
	public Vector2 PosBoundsY;
	public Vector2 PosBoundsZ;

	[Header ("Rotation")]
	public bool FollowRotation;
	[Space (10)]
	public bool AutomaticallyFindPlayerRotObject;
	public string LookForRotName = "Player";
	[Space (10)]
	public Transform FollowRotX;
	public Transform FollowRotY;
	public Transform FollowRotZ;
	[Space (10)]
	public Vector3 FollowRotOffset;
	public Vector3 FollowRotSmoothTime;
	[Space (10)]
	public bool XRotTimeUnscaled = true;
	public bool YRotTimeUnscaled = true;
	public bool ZRotTimeUnscaled = true;

	public enum delta
	{
		Scaled,
		Unscaled,
		Fixed
	}

	public void Start ()
	{
		FollowPosVelY = 0;

		if (AutomaticallyFindPlayerPosObject == true)
		{
			Transform PlayerPos = GameObject.Find (LookForPosName).transform;

			FollowPosX = PlayerPos;
			FollowPosY = PlayerPos;
			FollowPosZ = PlayerPos;
		}

		if (AutomaticallyFindPlayerRotObject == true)
		{
			Transform PlayerRot = GameObject.Find (LookForRotName).transform;

			FollowRotX = PlayerRot;
			FollowRotY = PlayerRot;
			FollowRotZ = PlayerRot;
		}
	}

	void Update () 
	{
		if (updateMethod == UpdateMethod.Update) 
		{
			if (OverrideTransform != null) 
			{
				FollowPosX = OverrideTransform.transform;
				FollowPosY = OverrideTransform.transform;
				FollowPosZ = OverrideTransform.transform;
				FollowRotX = OverrideTransform.transform;
				FollowRotY = OverrideTransform.transform;
				FollowRotZ = OverrideTransform.transform;
			}

			if (FollowPosition == true && FollowPosMethod != followPosMethod.NoSmoothing) 
			{
				FollowObjectPosition ();
			}

			CheckFollowPosTransforms ();

			if (FollowRotation == true) 
			{
				FollowObjectRotation ();
			}

			if (FollowPosMethod == followPosMethod.NoSmoothing) 
			{
				Vector3 Pos = new Vector3 (FollowPosX.position.x, FollowPosY.position.y, FollowPosZ.position.z);
				transform.position = Pos;
			}
		}
	}

	void FixedUpdate () 
	{
		if (updateMethod == UpdateMethod.FixedUpdate) 
		{
			if (OverrideTransform != null) 
			{
				FollowPosX = OverrideTransform.transform;
				FollowPosY = OverrideTransform.transform;
				FollowPosZ = OverrideTransform.transform;
				FollowRotX = OverrideTransform.transform;
				FollowRotY = OverrideTransform.transform;
				FollowRotZ = OverrideTransform.transform;
			}

			if (FollowPosition == true && FollowPosMethod != followPosMethod.NoSmoothing) 
			{
				FollowObjectPosition ();
			}

			CheckFollowPosTransforms ();

			if (FollowRotation == true) 
			{
				FollowObjectRotation ();
			}

			if (FollowPosMethod == followPosMethod.NoSmoothing) 
			{
				Vector3 Pos = new Vector3 (FollowPosX.position.x, FollowPosY.position.y, FollowPosZ.position.z);
				transform.position = Pos;
			}
		}
	}

	void LateUpdate () 
	{
		if (updateMethod == UpdateMethod.LateUpdate) 
		{
			if (OverrideTransform != null) 
			{
				FollowPosX = OverrideTransform.transform;
				FollowPosY = OverrideTransform.transform;
				FollowPosZ = OverrideTransform.transform;
				FollowRotX = OverrideTransform.transform;
				FollowRotY = OverrideTransform.transform;
				FollowRotZ = OverrideTransform.transform;
			}

			if (FollowPosition == true && FollowPosMethod != followPosMethod.NoSmoothing) 
			{
				FollowObjectPosition ();
			}

			CheckFollowPosTransforms ();

			if (FollowRotation == true) 
			{
				FollowObjectRotation ();
			}

			if (FollowPosMethod == followPosMethod.NoSmoothing) 
			{
				Vector3 Pos = new Vector3 (FollowPosX.position.x, FollowPosY.position.y, FollowPosZ.position.z);
				transform.position = Pos;
			}
		}
	}

	void FollowObjectPosition ()
	{
		if (FollowPosMethod == followPosMethod.Lerp) 
		{
			xPos = Mathf.Clamp (
				Mathf.Lerp (
					transform.position.x, 
					FollowPosX.position.x + FollowPosOffset.x, 
					FollowPosSmoothTime.x * (XPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

				PosBoundsX.x, 
				PosBoundsX.y
			);

			yPos = Mathf.Clamp (
	             Mathf.Lerp (
		             transform.position.y, 
		             FollowPosY.position.y + FollowPosOffset.y, 
		             FollowPosSmoothTime.y * (YPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

	             PosBoundsY.x, 
	             PosBoundsY.y
             );

			zPos = Mathf.Clamp (
	             Mathf.Lerp (
		             transform.position.z, 
		             FollowPosZ.position.z + FollowPosOffset.z, 
		             FollowPosSmoothTime.z * (ZPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

	             PosBoundsZ.x, 
	             PosBoundsZ.y
             );
				
			Vector3 newPos = new Vector3 (xPos, yPos, zPos);
			transform.position = newPos;
		}

		if (FollowPosMethod == followPosMethod.SmoothDamp) 
		{
			xPos = Mathf.Clamp (
				Mathf.SmoothDamp (
					transform.position.x, 
					FollowPosX.position.x + FollowPosOffset.x, 
					ref FollowPosVelX, 
					FollowPosSmoothTime.x * (XPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

				PosBoundsX.x, 
				PosBoundsX.y
			);

			yPos = Mathf.Clamp (
	             Mathf.SmoothDamp (
		             transform.position.y, 
		             FollowPosY.position.y + FollowPosOffset.y, 
		             ref FollowPosVelY, 
		             FollowPosSmoothTime.y * (YPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

	             PosBoundsY.x, 
	             PosBoundsY.y
             );

			zPos = Mathf.Clamp (
				Mathf.SmoothDamp (
					transform.position.z, 
					FollowPosZ.position.z + FollowPosOffset.z, 
					ref FollowPosVelZ, 
					FollowPosSmoothTime.z * (ZPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

				PosBoundsZ.x, 
				PosBoundsZ.y
			);

			Vector3 newPos = new Vector3 (xPos, yPos, zPos);
			transform.position = newPos;
		}
	}

	void CheckFollowPosTransforms ()
	{
		if (FollowPosX.transform == null) 
		{
			FollowPosX = this.transform;
		}

		if (FollowPosY.transform == null) 
		{
			FollowPosY = this.transform;
		}

		if (FollowPosZ.transform == null) 
		{
			FollowPosZ = this.transform;
		}
	}

	void FollowObjectRotation ()
	{
		Vector3 RotationAngle = new Vector3 
			(
				Mathf.LerpAngle (
					transform.eulerAngles.x, 
					FollowRotX.eulerAngles.x + FollowRotOffset.x, 
					FollowRotSmoothTime.x * (XRotTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)),
				
				Mathf.LerpAngle (
					transform.eulerAngles.y, 
					FollowRotY.eulerAngles.y + FollowRotOffset.y, 
					FollowRotSmoothTime.y * (YRotTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)),
				
				Mathf.LerpAngle (
					transform.eulerAngles.z, 
					FollowRotZ.eulerAngles.z + FollowRotOffset.z, 
					FollowRotSmoothTime.z * (ZRotTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime))
			);

		transform.rotation = Quaternion.Euler(RotationAngle);
	}

	public void OverridePos (Transform pos)
	{
		OverrideTransform = pos;
	}

	public void SetFollowPos (bool follow)
	{
		FollowPosition = follow;
	}

	public void SetFollowRot (bool follow)
	{
		FollowRotation = follow;
	}
}
