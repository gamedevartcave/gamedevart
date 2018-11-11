using System.Collections;
using UnityEngine;

public class VibrateController : MonoBehaviour 
{
	public static VibrateController instance { get; private set; }
	[ReadOnlyAttribute] public float vibrationTime;
	[ReadOnlyAttribute] public float leftMotor;
	[ReadOnlyAttribute] public float rightMotor;
	[ReadOnlyAttribute] int priority;

	private PlayerActions playerActions;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		playerActions = InControlActions.instance.playerActions;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.V))
		{
			GameController.instance.camShakeScript.shakeDuration = 1;
			GameController.instance.camShakeScript.Shake ();
			Vibrate (0.25f, 0.25f, 0.25f, 1);
		}
	}
	
	public void Vibrate (float _leftMotor, float _rightMotor, float _vibrationTime, int _priority)
	{
		if (_priority >= priority)
		{
			vibrationTime = _vibrationTime;
			playerActions.ActiveDevice.Vibrate (_leftMotor, _rightMotor);
			StartCoroutine (VibrateTime ());
		}
	}

	IEnumerator VibrateTime ()
	{
		yield return new WaitForSeconds (vibrationTime);
		playerActions.ActiveDevice.StopVibration ();
	}
}
