using UnityEngine;

public class TimescaleController : MonoBehaviour 
{
	public static TimescaleController instance { get; private set; }

	public float timeStep = 0.01f;

	public void LateUpdate ()
	{
		SetFixedUpdateOnTimeScale ();
	}

	void SetFixedUpdateOnTimeScale ()
	{
		Time.fixedDeltaTime = Time.timeScale * timeStep;
	}
}
