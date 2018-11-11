using UnityEngine;
using System.Collections.Generic;

public class HiResScreenShots : MonoBehaviour 
{
	public static HiResScreenShots instance { get ; private set; }

	public Vector2 baseResolution = new Vector2 (Screen.width, Screen.height); // Base resolution dimensions.
	private Vector2 ResolutionMultiplier = new Vector2 (1, 1);
	private bool processShot = false; // Is screenshot going to be processed now?

	public List<Camera> cameras; // Reference to camera.

	void Awake ()
	{
		instance = this;
	}

	public void AddCamera (Camera _cam)
	{
		cameras.Add (_cam);
	}

	public void RemoveCamera (Camera _cam)
	{
		cameras.Remove (_cam);
	}
		
	void Update ()
	{
		// 800% base resolution.
		if (Input.GetKeyDown (KeyCode.F11))
		{
			ResolutionMultiplier = new Vector2 (8, 8);
			TakeHiResShot ();
		}

		// 400% base resolution.
		if (Input.GetKeyDown (KeyCode.F10))
		{
			ResolutionMultiplier = new Vector2 (4, 4);
			TakeHiResShot ();
		}

		// 200% base resolution.
		if (Input.GetKeyDown (KeyCode.F9))
		{
			ResolutionMultiplier = new Vector2 (2, 2);
			TakeHiResShot ();
		}

		// 100% base resolution.
		if (Input.GetKeyDown (KeyCode.F8)) 
		{
			ResolutionMultiplier = new Vector2 (1, 1);
			TakeHiResShot ();
		}

		// 25% dimension size.
		if (Input.GetKeyDown (KeyCode.F7)) 
		{
			ResolutionMultiplier = new Vector2 (0.5f, 0.5f);
			TakeHiResShot ();
		}

		// Take screenshot using window size as base resolution (overwrites base resolution value).
		if (Input.GetKeyDown (KeyCode.F4))
		{
			Vector2 LastBaseRes = baseResolution;
			baseResolution = new Vector2 (Screen.height, Screen.width);
			ResolutionMultiplier = new Vector2 (1, 1);
			TakeHiResShot ();
			baseResolution = LastBaseRes;
		}
	}

	// Only want to check and take screenshots when the frame has been fully rendered.
	void LateUpdate () 
	{
		CheckProcessShot ();
	}

	void CheckProcessShot ()
	{
		if (processShot == true) 
		{
			// Create file name and directory if it doesn't exist.
			if (System.IO.Directory.Exists (Application.persistentDataPath + "/" + "Screenshots") == false)
			{
				System.IO.Directory.CreateDirectory (Application.persistentDataPath + "/" + "Screenshots");
				Debug.Log ("Screenshot folder was missing, created new one.");
			}

			// Scxreenshot directory exists? Take a screenshot.
			if (System.IO.Directory.Exists (Application.persistentDataPath + "/" + "Screenshots") == true)
			{
				if (Screen.orientation == ScreenOrientation.Landscape || Screen.width > Screen.height)
				{
					TakeScreenshot (true);
				}

				if (Screen.orientation == ScreenOrientation.Portrait && Screen.width <= Screen.height)
				{
					TakeScreenshot (false);
				}
			}

			processShot = false; // Stop taking a screenshot.
		}
	}
		
	public void TakeHiResShot () 
	{
		processShot = true;
	}

	void TakeScreenshot (bool isLandscape)
	{
		// Get new resolution.
		Vector2 newResolution = new Vector2 (
			isLandscape ? 
			Mathf.RoundToInt ((int)baseResolution.x * ResolutionMultiplier.x) : 
			Mathf.RoundToInt ((int)baseResolution.y * ResolutionMultiplier.x), 

			isLandscape ? 
			Mathf.RoundToInt ((int)baseResolution.y * ResolutionMultiplier.y) : 
			Mathf.RoundToInt ((int)baseResolution.x * ResolutionMultiplier.y)
		);

		// Create a new render texture and store it.
		RenderTexture rt = new RenderTexture (
			(int)newResolution.x, 
			(int)newResolution.y,
			24
		);

		// Set camera target texture to render texture temporaily.
		//cam.targetTexture = rt;
		foreach (Camera c in cameras)
		{
			c.targetTexture = rt;
		}

		// Create a Texture 2D with new resolution and bit depth. Don't allow mipmaps.
		Texture2D screenShot = new Texture2D (
			(int)newResolution.x, 
			(int)newResolution.y, 
			TextureFormat.RGB24, 
			false
		);

		// Manually render the camera.
		//cam.Render ();
		foreach (Camera c in cameras)
		{
			c.Render ();
		}

		// Assign currently active render texture to temporary texture.
		RenderTexture.active = rt;

		// Read all the pixels in the new image with no offset and specify width and height.
		screenShot.ReadPixels (
			new Rect (
				0, 
				0, 
				(int)newResolution.x, 
				(int)newResolution.y), 
			0, 
			0
		);

		// Unnassign target texture.
		//cam.targetTexture = null;
		foreach (Camera c in cameras)
		{
			c.targetTexture = null;
		}

		// Unnassign active render texture to avoid errors.
		RenderTexture.active = null;

		// Destroy the temporary render texture.
		Destroy(rt);

		// Encode the pixels to a .png format.
		byte[] bytes = screenShot.EncodeToPNG();

		// Give screenshot name and specify the dimensions.
		string filename = ScreenShotName (
			(int)newResolution.x, 
			(int)newResolution.y
		);

		// Write the file and specify bytes to store.
		System.IO.File.WriteAllBytes (filename, bytes);

		// Confirm that the screenshot took place.
		Debug.Log (string.Format("Took " + (isLandscape ? "landscape" : "portrait") + " screenshot to: {0}", filename));
	}

	// Creates file name.
	public static string ScreenShotName (int width, int height) 
	{
		if (Screen.orientation == ScreenOrientation.Landscape)
		{
			return string.Format ("{0}/Screenshots/screen_{1}x{2}_{3}.png", 
				Application.persistentDataPath, 
				width, height, 
				System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss"));
		}

		if (Screen.orientation == ScreenOrientation.Portrait)
		{
			return string.Format ("{0}/Screenshots/screen_{1}x{2}_{3}.png", 
				Application.persistentDataPath, 
				height, width, 
				System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss"));
		}

		return null;
	}
}