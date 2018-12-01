using UnityEngine;

public class SimpleFollow : MonoBehaviour 
{
	public UpdateMethod posUpdateMethod;
	public UpdateMethod rotUpdateMethod;
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
	[Space (10)]
	[ReadOnlyAttribute] [SerializeField] private Vector3 targetPos;
	public Transform[] FollowPosX;
	public Transform[] FollowPosY;
	public Transform[] FollowPosZ;

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

			FollowPosX = new Transform[] {PlayerPos};
			FollowPosY = new Transform[] {PlayerPos};
			FollowPosZ = new Transform[] {PlayerPos};
		}

		if (AutomaticallyFindPlayerRotObject == true)
		{
			Transform PlayerRot = GameObject.Find (LookForRotName).transform;

			FollowRotX = PlayerRot;
			FollowRotY = PlayerRot;
			FollowRotZ = PlayerRot;
		}
	}

	float GetAverageXPoint (Transform[] xPoints)
	{
		float accumulativeXPoints = 0;
		float averageXPoints = 0;

		for (int i = 0; i < xPoints.Length; i++)
		{
			accumulativeXPoints += xPoints [i].position.x;
		}

		averageXPoints = accumulativeXPoints / xPoints.Length;
		return averageXPoints;
	}

	float GetAverageYPoint (Transform[] yPoints)
	{
		float accumulativeYPoints = 0;
		float averageYPoints = 0;

		for (int i = 0; i < yPoints.Length; i++)
		{
			accumulativeYPoints += yPoints [i].position.y;
		}

		averageYPoints = accumulativeYPoints / yPoints.Length;
		return averageYPoints;
	}

	float GetAverageZPoint (Transform[] zPoints)
	{
		float accumulativeZPoints = 0;
		float averageZPoints = 0;

		for (int i = 0; i < zPoints.Length; i++)
		{
			accumulativeZPoints += zPoints [i].position.z;
		}

		averageZPoints = accumulativeZPoints / zPoints.Length;
		return averageZPoints;
	}

	void Update () 
	{
		// Positioning
		if (posUpdateMethod == UpdateMethod.Update)
		{
			CheckFollowPosTransforms ();
						
			if (FollowPosition == true)
			{
				switch (FollowPosMethod)
				{
				case followPosMethod.NoSmoothing:
					Vector3 targetPos = new Vector3 (
						GetAverageXPoint (FollowPosX), GetAverageYPoint(FollowPosY), GetAverageZPoint (FollowPosZ));
					transform.position = targetPos;
					break;
				case followPosMethod.Lerp:
					FollowObjectPosition ();
					break;
				case followPosMethod.SmoothDamp:
					FollowObjectPosition ();
					break;
				}
			}
		}

		// Rotation
		if (rotUpdateMethod == UpdateMethod.Update)
		{
			if (FollowRotation == true) 
			{
				FollowObjectRotation ();
			}
		}
	}

	void FixedUpdate () 
	{
		// Positioning
		if (posUpdateMethod == UpdateMethod.FixedUpdate) 
		{
			CheckFollowPosTransforms ();

			if (FollowPosition == true)
			{
				switch (FollowPosMethod)
				{
				case followPosMethod.NoSmoothing:
					Vector3 targetPos = new Vector3 (
						GetAverageXPoint (FollowPosX), GetAverageYPoint(FollowPosY), GetAverageZPoint (FollowPosZ));
					transform.position = targetPos;
					break;
				case followPosMethod.Lerp:
					FollowObjectPosition ();
					break;
				case followPosMethod.SmoothDamp:
					FollowObjectPosition ();
					break;
				}
			}
		}

		// Rotation
		if (rotUpdateMethod == UpdateMethod.FixedUpdate)
		{
			if (FollowRotation == true) 
			{
				FollowObjectRotation ();
			}
		}
	}

	void LateUpdate () 
	{
		// Positioning
		if (posUpdateMethod == UpdateMethod.LateUpdate) 
		{
			CheckFollowPosTransforms ();

			if (FollowPosition == true)
			{
				switch (FollowPosMethod)
				{
				case followPosMethod.NoSmoothing:
					Vector3 targetPos = new Vector3 (
						GetAverageXPoint (FollowPosX), GetAverageYPoint(FollowPosY), GetAverageZPoint (FollowPosZ));
					transform.position = targetPos;
					break;
				case followPosMethod.Lerp:
					FollowObjectPosition ();
					break;
				case followPosMethod.SmoothDamp:
					FollowObjectPosition ();
					break;
				}
			}
		}

		// Rotation
		if (rotUpdateMethod == UpdateMethod.LateUpdate)
		{
			if (FollowRotation == true) 
			{
				FollowObjectRotation ();
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
					GetAverageXPoint (FollowPosX) + FollowPosOffset.x, 
					FollowPosSmoothTime.x * (XPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

				PosBoundsX.x, 
				PosBoundsX.y
			);

			yPos = Mathf.Clamp (
	             Mathf.Lerp (
		             transform.position.y, 
					 GetAverageYPoint (FollowPosY) + FollowPosOffset.y, 
		             FollowPosSmoothTime.y * (YPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

	             PosBoundsY.x, 
	             PosBoundsY.y
             );

			zPos = Mathf.Clamp (
	             Mathf.Lerp (
		             transform.position.z, 
					 GetAverageZPoint (FollowPosZ) + FollowPosOffset.z, 
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
					GetAverageXPoint (FollowPosX) + FollowPosOffset.x, 
					ref FollowPosVelX, 
					FollowPosSmoothTime.x * (XPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

				PosBoundsX.x, 
				PosBoundsX.y
			);

			yPos = Mathf.Clamp (
	             Mathf.SmoothDamp (
		             transform.position.y, 
					 GetAverageYPoint (FollowPosY) + FollowPosOffset.y, 
		             ref FollowPosVelY, 
		             FollowPosSmoothTime.y * (YPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

	             PosBoundsY.x, 
	             PosBoundsY.y
             );

			zPos = Mathf.Clamp (
				Mathf.SmoothDamp (
					transform.position.z, 
					GetAverageZPoint (FollowPosZ) + FollowPosOffset.z, 
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
		if (FollowPosX == null) 
		{
			FollowPosX = new Transform[] {this.transform};
		}

		if (FollowPosY == null) 
		{
			FollowPosY = new Transform[] {this.transform};
		}

		if (FollowPosZ == null) 
		{
			FollowPosZ = new Transform[] {this.transform};
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

		transform.rotation = Quaternion.Euler (RotationAngle);
	}

	public void SetFollowPos (bool follow)
	{
		FollowPosition = follow;
	}

	public void SetFollowRot (bool follow)
	{
		FollowRotation = follow;
	}

	public void SetFollowRotX (Transform newFollowRotX)
	{
		FollowRotX = newFollowRotX;
	}

	public void SetFollowRotY (Transform newFollowRotY)
	{
		FollowRotY = newFollowRotY;
	}

	public void SetFollowRotZ (Transform newFollowRotZ)
	{
		FollowRotZ = newFollowRotZ;
	}
}
