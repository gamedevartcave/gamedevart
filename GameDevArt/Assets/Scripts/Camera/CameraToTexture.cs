using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CityBashers
{
	public class CameraToTexture : MonoBehaviour 
	{
		public static CameraToTexture Instance { get; private set; }

		public RawImage OutputGraphic;
		private Texture2D renderedTexture;
		public Camera[] cameras;

		public UnityEvent OnCameraRenderedToTexture;

		private void Awake()
		{
			Instance = this;
		}

		/// <summary>
		/// Starts render camera to texture process.
		/// </summary>
		public void RenderCamToTexture ()
		{
			StartCoroutine (RenderCamera ());
		}

		/// <summary>
		/// Renders the camera and assigns it to graphic.
		/// </summary>
		/// <returns>The camera.</returns>
		IEnumerator RenderCamera ()
		{
			yield return new WaitForEndOfFrame ();
			renderedTexture = ScreenCapture.CaptureScreenshotAsTexture ();
			OutputGraphic.texture = renderedTexture;
			OutputGraphic.enabled = true;
			OnCameraRenderedToTexture.Invoke ();
		}

		/// <summary>
		/// Destroys the texture that was created.
		/// </summary>
		public void DestroyTexture ()
		{
			OutputGraphic.enabled = false;
			OutputGraphic.texture = null;
			Destroy (renderedTexture);

			SetCamerasState(true);
		}

		/// <summary>
		/// Enables the cameras in cameras array.
		/// </summary>
		public void SetCamerasState(bool active)
		{
			for (int i = 0; i < cameras.Length; i++)
			{
				cameras[i].enabled = active;
			}
		}
	}
}
