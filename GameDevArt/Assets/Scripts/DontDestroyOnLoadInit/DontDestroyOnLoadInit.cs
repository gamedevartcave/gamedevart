using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

namespace CityBashers
{
	public class DontDestroyOnLoadInit : MonoBehaviour 
	{
		public static DontDestroyOnLoadInit Instance { get; private set; }

		[ReadOnly] public bool initialized;
		[Tooltip ("Managers Prefab.")]
		public GameObject ManagersPrefab;
		public GameObject EventSystemGameObject;
		public float initializeWaitTime = 0.5f;
		public UnityEvent OnInitialize;
		private WaitForSeconds initializeWait;

		void Awake ()
		{
			Instance = this;
			Physics.autoSimulation = false;
			Time.timeScale = 0;
			initializeWait = new WaitForSeconds (initializeWaitTime);

			DetectManagers();

			Destroy (EventSystemGameObject);
			EventSystemGameObject = null;
			OnInitialized(); // Essential for scenes to initialize correctly.
		}

		/// <summary>
		/// Detects InitManager Instance, loads init scene if not found.
		/// </summary>
		public void DetectManagers ()
		{
			// If there is no MANAGERS GameObject present,
			// Load init scene.
			if (InitManager.Instance == null)
			{
				SceneManager.LoadSceneAsync (0, LoadSceneMode.Additive);
			}
		}

		/// <summary>
		/// Main initialization sequence.
		/// </summary>
		/// <returns></returns>
		IEnumerator Initialize ()
		{
			yield return initializeWait;

			OnInitialize.Invoke ();
			initialized = true;
			SceneLoader.Instance.OnSceneLoadComplete.Invoke ();
			Time.timeScale = 1;
			SceneLoader.Instance.backgroundFader.fader.SetBool("Active", false);
			Physics.autoSimulation = true;
			gameObject.SetActive (false);
		}

		/// <summary>
		/// Loads data from save files.
		/// </summary>
		public void LoadData ()
		{
			SaveAndLoadScript.Instance.InitializeLoad ();
		}

		/// <summary>
		/// Event to initialize the scene.
		/// </summary>
		public void OnInitialized ()
		{
			StartCoroutine (Initialize ());
		}

		/// <summary>
		/// Assigns post process volume to manipulate at runtime.
		/// </summary>
		/// <param name="newPostProcessVolume"></param>
		public void AssignPostProcessVolume (PostProcessVolume newPostProcessVolume)
		{
			SaveAndLoadScript.Instance.postProcessVolume = newPostProcessVolume;
		}
	}
}