using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

namespace CityBashers
{
	public class DontDestroyOnLoadInit : MonoBehaviour 
	{
		public static DontDestroyOnLoadInit Instance { get; private set; }
		[ReadOnlyAttribute] public bool initialized;
		[Tooltip ("Managers Prefab.")]
		public GameObject ManagersPrefab;
		public float Delay;
		public float initializeWaitTime = 0.5f;
		private GameObject managers;
		public UnityEvent OnInitialize;

		private WaitForSeconds initializeWait;

		public GameObject EventSystemGameObject;

		void Awake ()
		{
			Instance = this;
			Time.timeScale = 1;
			initializeWait = new WaitForSeconds (initializeWaitTime);
			Invoke ("DetectManagers", Delay);

			Destroy (EventSystemGameObject);
		}

		public void DetectManagers ()
		{
			// If there is no MANAGERS GameObject present,
			// Create one and make it not destory on load.
			if (InitManager.Instance == null)
			{
				//managers = Instantiate (ManagersPrefab); // Includes the InitManager.
				//managers.name = "MANAGERS";
				//DontDestroyOnLoad (managers.gameObject); 
				SceneManager.LoadSceneAsync (0, LoadSceneMode.Additive);
			}
		}

		IEnumerator Initialize ()
		{
			yield return initializeWait;
			OnInitialize.Invoke ();
			initialized = true;
			SceneLoader.Instance.OnSceneLoadComplete.Invoke ();
			Time.timeScale = 1;
			this.gameObject.SetActive (false);
		}

		public void LoadData ()
		{
			SaveAndLoadScript.Instance.InitializeLoad ();
		}

		public void OnInitialized ()
		{
			StartCoroutine (Initialize ());
		}

		public void AssignPostProcessVolume (PostProcessVolume newPostProcessVolume)
		{
			SaveAndLoadScript.Instance.postProcessVolume = newPostProcessVolume;
		}
	}
}