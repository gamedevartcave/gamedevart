using UnityEngine;
using UnityEngine.SceneManagement;

namespace CityBashers
{
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

		void Start ()
		{
			//FadeOutLoaderUI ();
			//BackgroundFader.instance.StartFade ();
		}

		public void LoadScene (string sceneName)
		{
			if (SceneLoadCommit == false)
			{
				SceneLoader.Instance.SceneName = sceneName;
				SceneLoader.Instance.SmoothProgress = 0;
				SceneLoader.Instance.LoadProgressText.text = "0%";
				SceneLoader.Instance.LoadSlider.value = 0;
				SceneLoader.Instance.SceneLoaderUI.ResetTrigger ("Disappear");
				SceneLoader.Instance.SceneLoaderUI.SetTrigger ("Appear");

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
				SceneLoader.Instance.SmoothProgress = 0;
				SceneLoader.Instance.LoadProgressText.text = "0%";
				SceneLoader.Instance.LoadSlider.value = 0;
				SceneLoader.Instance.SceneLoaderUI.ResetTrigger ("Disappear");
				SceneLoader.Instance.SceneLoaderUI.SetTrigger ("Appear");

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

		public void SetLoadScreenBackground (Texture background)
		{
			SceneLoader.Instance.loadScreenBackground.texture = background;

			if (SceneLoader.Instance.loadScreenBackground.texture == null)
			{
				SceneLoader.Instance.loadScreenBackground.color = new Color (0, 0, 0, 0);
			} 

			else
			
			{
				SceneLoader.Instance.loadScreenBackground.color = Color.white;
			}
		}

		public void FadeOutLoaderUI ()
		{
			SceneLoader.Instance.SceneLoadUIDisappear ();
		}
	}
}
