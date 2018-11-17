using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalSceneLoader : MonoBehaviour 
{
	public static LocalSceneLoader Instance { get; private set; }
	public bool SceneLoadCommit;
	private string ActiveSceneName
	{
		get 
		{ 
			return SceneManager.GetActiveScene ().name; 
		}
	}
		
	private void Awake ()
	{
		Instance = this;
	}

	public void LoadScene (string sceneName)
	{
		if (SceneLoadCommit == false)
		{
			SceneLoader.Instance.SceneName = sceneName;
			SceneLoadSequence ();

			if (sceneName == "menu") 
			{
				InitManager.Instance.LoadingMissionText.text = "";
			}

			SceneLoadCommit = true;
		}
	}

	public void ReloadActiveScene ()
	{
		if (SceneLoadCommit == false)
		{
			SceneLoader.Instance.SceneName = ActiveSceneName;
			SceneLoadSequence ();

			if (ActiveSceneName == "menu") 
			{
				InitManager.Instance.LoadingMissionText.text = "";
			}

			SceneLoadCommit = true;
		}
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.R))
		{
			LoadScene (ActiveSceneName);
		}
	}

	void SceneLoadSequence ()
	{
		SceneLoader.Instance.StartLoadSequence ();
		InitManager.Instance.LoadingMissionText.gameObject.SetActive (true);
	}

	public void ActivateScene ()
	{
		if (SceneLoader.Instance != null)
		{
			if (SceneLoader.Instance.async != null)
			{
				SceneLoader.Instance.ActivateScene ();
			}
		}
	}

	public void DestroyCurrentLoadedObjects ()
	{
		SceneLoader.Instance.DestroyObjectsInScene ();
	}
}