using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CityBashers
{
	public class BackgroundFader : MonoBehaviour
	{
		public static BackgroundFader instance { get; private set; }

		public RawImage background;

		/// <summary>
		/// The amount of smoothing for the fade.
		/// </summary>
		public float fadeSmoothing = 3;

		/// <summary>
		/// The start color.
		/// </summary>
		public Color StartColor;

		/// <summary>
		/// The end color.
		/// </summary>
		public Color EndColor;

		void Awake ()
		{
			instance = this;
			this.enabled = false;
		}

		public void StartFade ()
		{
			StartCoroutine (FadeScreen ());
		}

		public IEnumerator FadeScreen ()
		{
			background.color = StartColor;
			SceneLoader.Instance.OnSceneLoadComplete.Invoke ();

			while (background.color != EndColor)
			{
				background.color = 
					Color.Lerp (
						background.color, 
						EndColor, 
						fadeSmoothing * Time.deltaTime
					);

				yield return null;
			}
		}
	}
}
