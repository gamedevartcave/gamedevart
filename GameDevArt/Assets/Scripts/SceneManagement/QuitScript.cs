using System.Collections;
using UnityEngine;

namespace CityBashers
{
	public class QuitScript : MonoBehaviour 
	{
		private float delayQuitTime;
		private WaitForSecondsRealtime QuitWait;

		void Start ()
		{
			QuitWait = new WaitForSecondsRealtime (delayQuitTime);
		}

		public void DelayAndQuit (float delay)
		{
			SaveAndLoadScript.Instance.SavePlayerData ();
			SaveAndLoadScript.Instance.SaveSettingsData ();
			delayQuitTime = delay;
			StartCoroutine (DelayQuitApp ());
		}

		IEnumerator DelayQuitApp ()
		{
			yield return QuitWait;
			QuitGame ();
		}

		public void QuitGame ()
		{
			Application.Quit ();
		}
	}
}