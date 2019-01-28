using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
		[HideInInspector] public float SmoothProgress; // The actual smoothed scene load progress value.

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
		
		// Unity events.
		public UnityEvent OnInitialize;
		public UnityEvent OnSceneLoadBegin;
		public UnityEvent OnSceneLoadComplete;

		private void Awake ()
		{
			Instance = this;
			OnInitialize.Invoke ();

			if (DontDestroyOnLoadInit.Instance != null)
			{
				DontDestroyOnLoadInit.Instance.OnInitialized ();
			}
		}

		void Start () 
		{
			// Checks if the currently loaded scene is the init scene.
			if (SceneManager.GetActiveScene ().name == "init") 
			{
				if (SceneManager.GetSceneByName (SceneName).isLoaded == false)
				{
					OnSceneLoadBegin.Invoke ();
				}
			}
		}

		/// <summary>
		/// Clears text for mission text.
		/// </summary>
		public void ClearMissionText ()
		{
			missionText = string.Empty;
			LoadingMissionText.text = string.Empty;
		}

		/// <summary>
		/// Called by local scene loader to start the loading process.
		/// </summary>
		public void StartLoadSequence ()
		{
			isLoading = true;
			StartCoroutine (LoadProgress ());
		}

		/// <summary>
		/// Gathers all GameObjects in the loaded scene as an array, destroys them all.
		/// </summary>
		public void DestroyObjectsInScene ()
		{
			GameObject[] rootObjectsInScene = SceneManager.GetActiveScene ().GetRootGameObjects ();

			foreach (GameObject RootObject in rootObjectsInScene)
			{
				Destroy (RootObject);
			}
		}

		/// <summary>
		/// Main scene loading process.
		/// </summary>
		IEnumerator LoadProgress ()
		{
			SmoothProgress = 0;
			LoadProgressText.text = "0%";
			LoadSlider.value = 0;

			if (LocalSceneLoader.Instance != null)
			{
				AsyncOperation unloadAsync = 
					SceneManager.UnloadSceneAsync (LocalSceneLoader.Instance.gameObject.scene);
				yield return unloadAsync;
			}

			GC.Collect ();
			asyncOp = SceneManager.LoadSceneAsync (SceneName, LoadSceneMode.Additive);
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
					backgroundFader.fader.SetBool("Active", true);
					yield break;
				}
				
				yield return null;
			}

			// Checks if the scene has been completely loaded into memory. 
			if (LoadProgressText.text == "100%")
			{
				backgroundFader.fader.SetBool("Active", true);
				yield break;
			}
		}

		/// <summary>
		/// Event fired when scene is ready to load.
		/// </summary>
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

		/// <summary>
		/// Event fired when scene is ready to activate.
		/// </summary>
		public void ActivateScene ()
		{
			StartCoroutine (LoadSceneDelay ());
		}

		IEnumerator LoadSceneDelay ()
		{
			// Finally, we can activate the newly loaded scene.
			if (asyncOp != null)
			{
				
			}

			GC.Collect ();
			Shader.WarmupAllShaders ();
			OnSceneLoadComplete.Invoke ();

			if (DontDestroyOnLoadInit.Instance != null)
			{
				if (DontDestroyOnLoadInit.Instance.gameObject.activeInHierarchy == true)
				{
					DontDestroyOnLoadInit.Instance.OnInitialized ();
				}
			}

			if (asyncOp != null)
			{
				asyncOp.allowSceneActivation = true;
			}

			yield return null;
		}

		/// <summary>
		/// Sets parameters for scene load fades.
		/// </summary>
		public void SceneLoadUIDisappear ()
		{
			SceneLoaderUI.ResetTrigger ("Appear");
			SceneLoaderUI.SetTrigger ("Disappear");
		}

		/// <summary>
		/// Actviates the loaded scene.
		/// </summary>
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