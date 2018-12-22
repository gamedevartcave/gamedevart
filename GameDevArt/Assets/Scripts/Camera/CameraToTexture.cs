using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering;
using System.Collections;
using UnityEngine.Events;

public class CameraToTexture : MonoBehaviour 
{
	private Camera cam;
	public RawImage OutputGraphic;
	private Texture2D renderedTexture;

	public Camera[] cameras;

	public UnityEvent OnCameraRenderedToTexture;

	void Start ()
	{
		cam = GetComponent<Camera> ();
	}

	public void RenderCamToTexture ()
	{
		StartCoroutine (RenderCamera ());
	}

	IEnumerator RenderCamera ()
	{
		yield return new WaitForEndOfFrame ();
		renderedTexture = ScreenCapture.CaptureScreenshotAsTexture ();
		OutputGraphic.texture = renderedTexture;
		OutputGraphic.enabled = true;
		OnCameraRenderedToTexture.Invoke ();
	}

	public void DestroyTexture ()
	{
		OutputGraphic.enabled = false;
		OutputGraphic.texture = null;
		Destroy (renderedTexture);

		for (int i = 0; i < cameras.Length; i++)
		{
			cameras [i].enabled = true;
		}
	}

	public void DisableCameras ()
	{
		for (int i = 0; i < cameras.Length; i++)
		{
			cameras [i].enabled = false;
		}
	}
}
