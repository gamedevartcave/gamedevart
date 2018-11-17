using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundFader : MonoBehaviour
{
	public static BackgroundFader instance { get; private set; }
	public RawImage background;
	public float fadeSmoothing = 3;
	public Color StartColor;
	public Color EndColor;

	void Awake ()
	{
		instance = this;
	}

	/*
	void Start ()
	{
		StartCoroutine (FadeScreen ());
	}
	*/

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
