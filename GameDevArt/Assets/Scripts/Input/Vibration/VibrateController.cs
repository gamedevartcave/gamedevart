using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Input;

namespace CityBashers
{
	public class VibrateController : MonoBehaviour 
	{
		public static VibrateController Instance { get; private set; }
		[ReadOnly] int priority;
		private readonly WaitForSeconds vibrateTime;

		void Awake ()
		{
			Instance = this;
			enabled = false;
		}
		
		public void Vibrate (float _leftMotor, float _rightMotor, float _vibrationTime, int _priority)
		{
			if (_priority >= priority)
			{
				Gamepad.current.SetMotorSpeeds(_leftMotor, _rightMotor);
				StartCoroutine (VibrateTime (_vibrationTime));
			}
		}

		IEnumerator VibrateTime (float _time)
		{
			yield return new WaitForSeconds (_time);
			Gamepad.current.SetMotorSpeeds(0, 0);
			priority = 0;
		}
	}
}
