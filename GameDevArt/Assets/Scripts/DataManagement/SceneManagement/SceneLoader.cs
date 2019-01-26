using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using TMPro;

namespace CityBashers
{
	public class SceneLoader : MonoBehaviour 
	{
		public static SceneLoader Instance { get; private set; }
		public AsyncOperation asyncOp = null; // The async operation variable. 

		public bool isLoading;
		public float delay; // How long before the actual loading of the next scene starts.
		public string SceneName; // The name of the scene that other scripts can modify. The next scene should load by this name.
		public float ProgressBarSmoothTime = 1; // The progress bar value smoothing amount.
		[HideInInspector]public float SmoothProgress; // The actual smoothed scene load progress value.

		[Header ("UI Elements")]
		public Canvas LevelLoadUICanvas;
		public TextMeshProUGUI LoadProgressText;
		public Animator SceneLoaderUI;
		public ParticleSystem[] LoadingParticles;
		public Animator LoadParticlesAnim;
		public Slider LoadSlider;
		public RawImage loadScreenBackground;
		public TextMeshProUGUI LoadingMissionText;
		public string missionText;

		public BackgroundFader backgroundFader;

		//private WaitForSecondsRealtime WaitDelay;
		public UnityEvent OnInitialize;
		public UnityEvent OnSceneLoadBegin;
		public UnityEvent OnSceneLoadComplete;

		#region Singleton
		private void Awake ()
		{
			Instance = this;
			OnInitialize.Invoke ();

			if (DontDestroyOnLoadInit.Instance != null)
			{
				DontDestroyOnLoadInit.Instance.OnInitialized ();
			}
		}
		#endregion

		void Start () 
		{
			//WaitDelay = new WaitForSecondsRealtime (delay);

			// Checks if the currently loaded scene is the init scene.
			if (SceneManager.GetActiveScene ().name == "init") 
			{
				//InitManager.Instance.LoadingMissionText.gameObject.SetActive (false);

				if (SceneManager.GetSceneByName (SceneName).isLoaded == false)
				{
					OnSceneLoadBegin.Invoke ();
				}
			}
		}

		public void ClearMissionText ()
		{
			missionText = string.Empty;
			LoadingMissionText.text = string.Empty;
		}

		public void StartLoadSequence ()
		{
			isLoading = true;
			StartCoroutine (LoadProgress ());
		}

		// Gathers all GameObjects in the loaded scene as an array, destroys them all.
		public void DestroyObjectsInScene ()
		{
			GameObject[] rootObjectsInScene = SceneManager.GetActiveScene ().GetRootGameObjects ();

			foreach (GameObject RootObject in rootObjectsInScene)
			{
				Destroy (RootObject);
			}
		}

		// Main scene loading process.
		IEnumerator LoadProgress ()
		{
			SmoothProgress = 0;
			LoadProgressText.text = "0%";
			LoadSlider.value = 0;

			if (LocalSceneLoader.Instance != null)
			{
				AsyncOperation unloadAsync = SceneManager.UnloadSceneAsync (LocalSceneLoader.Instance.gameObject.scene);

				yield return unloadAsync;
			}

			GC.Collect ();

			//asyncOp = SceneManager.LoadSceneAsync (SceneName, LoadSceneMode.Single);
			asyncOp = SceneManager.LoadSceneAsync (SceneName, LoadSceneMode.Additive);

			//yield return WaitDelay;
			asyncOp.allowSceneActivation = false; // Prevents the loading scene from activating.

			while (!asyncOp.isDone) 
			{
				float asyncprogress = Mathf.Round (1.1111f * asyncOp.progress * 100);
				//AudioListener.volume -= 0.2f * Time.unscaledDeltaTime;

				// UI checks load progress and displays for the player.
				SmoothProgress = Mathf.Lerp (SmoothProgress, asyncOp.progress, ProgressBarSmoothTime * Time.unscaledDeltaTime);
					
				if (asyncprogress < 100) 
				{
					//Debug.Log ("Scene load async progress: " + Mathf.Round (1.1111f * (asyncOp.progress * 100)) + "%");
				}

				// Somehow async operations load up to 90% before loading the next scene,
				// we have to compensate by adding 10% to the progress text.
				LoadProgressText.text = Mathf.Round (1.1111f * (SmoothProgress * 100)) + "%";
				LoadSlider.value = 1.1111f * (SmoothProgress * 100);

				
				// Checks if the scene has been completely loaded into memory. 
				if (LoadProgressText.text == "100%")
				{
					OnLoadThisScene ();
					SceneLoadUIDisappear();
					//BackgroundFader.Instance.fader.SetTrigger("FadeIn");
				}
				
				yield return null;
			}

			// Checks if the scene has been completely loaded into memory. 
			if (LoadProgressText.text == "100%")
			{
				OnLoadThisScene ();
				//OnSceneLoadComplete.Invoke();
				SceneLoadUIDisappear();
				backgroundFader.fader.SetTrigger("FadeIn");
			}
		}

		public void OnLoadThisScene ()
		{
			StartCoroutine (LoadThisScene ());
		}

		IEnumerator LoadThisScene ()
		{
			yield return new WaitForEndOfFrame ();

			isLoading = false;

			if (SceneManager.GetActiveScene ().name == "init")
			{
				
			}

			ActivateScene ();
		}

		public void ActivateScene ()
		{
			StartCoroutine (LoadSceneDelay ());
		}

		IEnumerator LoadSceneDelay ()
		{
			//yield return WaitDelay;

			// Finally, we can activate the newly loaded scene.
			if (asyncOp != null)
			{
				asyncOp.allowSceneActivation = true;
			}

			GC.Collect ();
			//Shader.WarmupAllShaders ();
			OnSceneLoadComplete.Invoke ();

			/*
			if (SceneName == "menu")
			{
				SceneLoadUIDisappear ();
			}
			*/

			SceneLoadUIDisappear ();

			if (DontDestroyOnLoadInit.Instance != null)
			{
				if (DontDestroyOnLoadInit.Instance.gameObject.activeInHierarchy == true)
				{
					DontDestroyOnLoadInit.Instance.OnInitialized ();
				}
			}

			if (asyncOp != null)
			{
				//asyncOp = null;
			}

			yield return null;
		}

		public void SceneLoadUIDisappear ()
		{
			SceneLoaderUI.ResetTrigger ("Appear");
			SceneLoaderUI.SetTrigger ("Disappear");
			//SceneLoaderUI.ResetTrigger ("Disappear");
		}

		public void SetLoadedSceneActive ()
		{
			if (SceneManager.GetSceneByName (SceneName).IsValid () == true)
			{
				if (SceneManager.GetSceneByName (SceneName).isLoaded == true)
				{
					SceneManager.SetActiveScene (SceneManager.GetSceneByName (SceneName));
				}
			}
		}
	}
}