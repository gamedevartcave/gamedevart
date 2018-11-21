using System.Collections;
using UnityEngine;

public class VibrateController : MonoBehaviour 
{
	public static VibrateController instance { get; private set; }
	[ReadOnlyAttribute] int priority;

	private PlayerActions playerActions;

	private WaitForSeconds vibrateTime;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		playerActions = InControlActions.instance.playerActions;
	}
	
	public void Vibrate (float _leftMotor, float _rightMotor, float _vibrationTime, int _priority)
	{
		if (_priority >= priority)
		{
			playerActions.ActiveDevice.Vibrate (_leftMotor, _rightMotor);
			StartCoroutine (VibrateTime (_vibrationTime));
		}
	}

	IEnumerator VibrateTime (float _time)
	{
		yield return new WaitForSeconds (_time);
		playerActions.ActiveDevice.StopVibration ();
		priority = 0;
	}
}
