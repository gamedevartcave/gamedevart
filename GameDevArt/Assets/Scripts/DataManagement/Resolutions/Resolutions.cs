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
				(int)resolutions [index].y, 
				Screen.fullScreen, 
				Screen.currentResolution.refreshRate
			);

			SaveAndLoadScript.Instance.targetResolutionWidth  = (int)resolutions [index].x;
			SaveAndLoadScript.Instance.targetResolutionHeight = (int)resolutions [index].y;
			SaveAndLoadScript.Instance.targetFrameRate = Screen.currentResolution.refreshRate;

			//Debug.Log ("New screen resolution: " + Screen.currentResolution);
			Debug.Log ("New screen resolution: " + (int)resolutions [index].x + " x " + (int)resolutions [index].y);
		}

		public void SetMaximumResolution ()
		{
			Screen.SetResolution (
				Display.main.systemWidth, 
				Display.main.systemHeight, 
				Screen.fullScreen, 
				Screen.currentResolution.refreshRate
			);

			SaveAndLoadScript.Instance.targetResolutionWidth  = Display.main.systemWidth;
			SaveAndLoadScript.Instance.targetResolutionHeight = Display.main.systemHeight;
			SaveAndLoadScript.Instance.targetFrameRate = Screen.currentResolution.refreshRate;

			Debug.Log ("New screen resolution: " + Screen.currentResolution);
		}
	}
}
