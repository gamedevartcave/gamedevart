using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleTimedSceneLoader : MonoBehaviour 
{
	public string SceneToLoad;
	public float Delay = 5;
	private WaitForSecondsRealtime WaitDelay;

	void Start ()
	{
		WaitDelay = new WaitForSecondsRealtime (Delay);
		StartCoroutine (TimedSceneLoad ());
	}

	IEnumerator TimedSceneLoad ()
	{
		yield return WaitDelay;
		SceneManager.LoadScene (SceneToLoad);
	}
}