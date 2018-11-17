using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using TMPro;

public class SceneLoader : MonoBehaviour 
{
	public static SceneLoader Instance { get; private set; }
	public AsyncOperation async = null; // The async operation variable. 

	public bool isLoading;
	public float delay; // How long before the actual loading of the next scene starts.
	public string SceneName; // The name of the scene that other scripts can modify. The next scene should load by this name.
	public float ProgressBarSmoothTime = 1; // The progress bar value smoothing amount.
	private float SmoothProgress; // The actual smoothed scene load progress value.

	[Header ("UI Elements")]
	public Canvas LevelLoadUICanvas;
	public TextMeshProUGUI LoadProgressText;
	public Animator SceneLoaderUI;
	public ParticleSystem[] LoadingParticles;
	public Animator LoadParticlesAnim;
	public Slider LoadSlider;

	private WaitForSecondsRealtime WaitDelay;

	public UnityEvent OnSceneLoadBegin;
	public UnityEvent OnSceneLoadComplete;

	#region Singleton
	private void Awake ()
	{
		Instance = this;
	}
	#endregion

	void Start () 
	{
		WaitDelay = new WaitForSecondsRealtime (delay);

		// Checks if the currently loaded scene is the init scene.
		if (SceneManager.GetActiveScene ().name == "init") 
		{
			InitManager.Instance.LoadingMissionText.gameObject.SetActive (false);

			if (SceneManager.GetSceneByName (SceneName).isLoaded == false)
			{
				OnSceneLoadBegin.Invoke ();
		
				StartLoadSequence ();
			}
		}
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
		SceneLoaderUI.gameObject.SetActive (true); // Turn on the scene loading UI
		SmoothProgress = 0;
		LoadProgressText.text = "";
		LoadSlider.value = 0;

		if (SceneLoaderUI.gameObject.activeInHierarchy == true)
		{
			//SceneLoaderUI.Play ("SceneLoaderUIAppear");
		}

		if (LoadParticlesAnim.gameObject.activeInHierarchy == true)
		{
			//LoadParticlesAnim.Play ("LoadingParticlesLoop");
		}

		foreach (ParticleSystem loadParticle in LoadingParticles) 
		{
			loadParticle.Play (true);
		}

		yield return WaitDelay;

		GC.Collect ();

		async = SceneManager.LoadSceneAsync (SceneName, LoadSceneMode.Single);
		async.allowSceneActivation = false; // Prevents the loading scene from activating.

		while (!async.isDone) 
		{
			float asyncprogress = Mathf.Round (1.1111f * async.progress * 100);
			AudioListener.volume -= 0.2f * Time.unscaledDeltaTime;

			// UI checks load progress and displays for the player.
			SmoothProgress = Mathf.Lerp (SmoothProgress, async.progress, ProgressBarSmoothTime * Time.unscaledDeltaTime);

			foreach (ParticleSystem loadParticle in LoadingParticles) 
			{
				var ParticleStartLifetimeMain = loadParticle.main;
				ParticleStartLifetimeMain.startLifetime = async.progress + 0.5f;
			}
				
			if (asyncprogress < 100) 
			{
				Debug.Log ("Scene load async progress: " + Mathf.Round (1.1111f * (async.progress * 100)) + "%");
			}

			// Somehow async operations load up to 90% before loading the next scene,
			// we have to compensate by adding 10% to the progress text.
			LoadProgressText.text = Mathf.Round (1.1111f * (SmoothProgress * 100)) + "%";
			LoadSlider.value = 1.1111f * (SmoothProgress * 100);

			// Checks if the scene has been completely loaded into memory. 
			if (LoadProgressText.text == "100%")
			{
				OnLoadThisScene ();
			}

			yield return null;
		}

		// Checks if the scene has been completely loaded into memory. 
		if (LoadProgressText.text == "100%")
		{
			OnLoadThisScene ();
		}
	}

	public void OnLoadThisScene ()
	{
		StartCoroutine (LoadThisScene ());
	}

	IEnumerator LoadThisScene ()
	{
		foreach (ParticleSystem loadParticle in LoadingParticles) 
		{
			loadParticle.Stop (true, ParticleSystemStopBehavior.StopEmitting);
		}
			
		yield return new WaitForEndOfFrame ();

		isLoading = false;

		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			Debug.LogWarning ("No internet connection...");
			ActivateScene ();
		}

		//if (SceneManager.GetActiveScene ().name == "init")
		//{
			//
		//}

		ActivateScene ();
	}

	public void ActivateScene ()
	{
		StartCoroutine (LoadSceneDelay ());
	}

	IEnumerator LoadSceneDelay ()
	{
		yield return WaitDelay;

		// Finally, we can activate the newly loaded scene.
		async.allowSceneActivation = true;

		//OnSceneLoadComplete.Invoke ();

		if (SceneLoaderUI.gameObject.activeInHierarchy == true)
		{
			if (SceneLoaderUI.GetAnimatorTransitionInfo (0).IsName ("SceneLoaderUIDisappear") == false)
			{
				//SceneLoaderUI.Play ("SceneLoaderUIDisappear");
			}
		}
	}

	public void LoadingUICheck ()
	{
		if (SceneLoaderUI.gameObject.activeInHierarchy == true)
		{
			SceneLoaderUI.Play ("SceneLoaderUIDisappear");
		}
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