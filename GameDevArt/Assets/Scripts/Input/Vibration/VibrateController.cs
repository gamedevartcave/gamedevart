using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Input;

namespace CityBashers
{
	public class VibrateController : MonoBehaviour 
	{
		public static VibrateController Instance { get; private set; }
		[ReadOnly] public int priority;

		private Coroutine coroutineInst;

		void Awake ()
		{
			Instance = this;
			enabled = false;
		}

		/// <summary>
		/// Gamepad vibrate with custom motor intensity, time, and priority.
		/// </summary>
		/// <param name="_leftMotor"></param>
		/// <param name="_rightMotor"></param>
		/// <param name="_vibrationTime"></param>
		/// <param name="_priority"></param>
		public void Vibrate (float _leftMotor, float _rightMotor, float _vibrationTime, int _priority)
		{
			if (_priority >= priority)
			{
				if (Gamepad.current != null)
				{
					Gamepad.current.SetMotorSpeeds(_leftMotor, _rightMotor);
					coroutineInst = StartCoroutine(VibrateTime(_vibrationTime));
				}
			}
		}

		/// <summary>
		/// Time taken before vibration stops.
		/// </summary>
		/// <param name="_time"></param>
		/// <returns></returns>
		IEnumerator VibrateTime (float _time)
		{
			yield return new WaitForSeconds (_time);
			StopVibrating();
		}

		private void OnApplicationQuit()
		{
			StopVibrating();
		}

		/// <summary>
		/// Halts vibrating on current gamepad.
		/// </summary>
		public void StopVibrating()
		{
			if (Gamepad.current != null)
			{
				Gamepad.current.SetMotorSpeeds(0, 0);
			}

			if (coroutineInst != null)
			{
				StopCoroutine(coroutineInst);
			}

			priority = 0;
		}
	}
}
