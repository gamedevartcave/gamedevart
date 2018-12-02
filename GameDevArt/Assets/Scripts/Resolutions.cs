using UnityEngine;

namespace CityBashers
{
	public class Resolutions : MonoBehaviour 
	{
		public Vector2[] resolutions;

		// Sets screen resolution with current setting of full screen and screen refresh rate.
		public void SetResolutionIndex (int index)
		{
			Screen.SetResolution (
				(int)resolutions [index].x, 
				(int)resolutions[index].y, 
				Screen.fullScreen, 
				Screen.currentResolution.refreshRate
			);

			SaveAndLoadScript.Instance.targetResolutionWidth = (int)resolutions [index].x;
			SaveAndLoadScript.Instance.targetResolutionHeight = (int)resolutions [index].y;
			SaveAndLoadScript.Instance.targetFrameRate = Screen.currentResolution.refreshRate;
			SaveAndLoadScript.Instance.SaveSettingsData ();

			Debug.Log ("New screen resolution: " + Screen.currentResolution);
		}

		public void SetMaximumResolution ()
		{
			Screen.SetResolution (
				Screen.width, 
				Screen.height, 
				Screen.fullScreen, 
				Screen.currentResolution.refreshRate
			);

			SaveAndLoadScript.Instance.targetResolutionWidth = Screen.width;
			SaveAndLoadScript.Instance.targetResolutionHeight = Screen.height;
			SaveAndLoadScript.Instance.targetFrameRate = Screen.currentResolution.refreshRate;
			SaveAndLoadScript.Instance.SaveSettingsData ();

			Debug.Log ("New screen resolution: " + Screen.currentResolution);
		}
	}
}
