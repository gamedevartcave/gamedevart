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
		[ReadOnly] public bool initialized;
		[Tooltip ("Managers Prefab.")]
		public GameObject ManagersPrefab;
		public float initializeWaitTime = 0.5f;
		public UnityEvent OnInitialize;

		private WaitForSeconds initializeWait;

		public GameObject EventSystemGameObject;

		void Awake ()
		{
			Instance = this;
			Time.timeScale = 1;
			initializeWait = new WaitForSeconds (initializeWaitTime);
			DetectManagers();
			Destroy (EventSystemGameObject);
			EventSystemGameObject = null;
		}

		public void DetectManagers ()
		{
			// If there is no MANAGERS GameObject present,
			// Load init scene.
			if (InitManager.Instance == null)
			{
				SceneManager.LoadSceneAsync (0, LoadSceneMode.Additive);
			}
		}

		IEnumerator Initialize ()
		{
			yield return initializeWait;
			OnInitialize.Invoke ();
			initialized = true;
			//SceneLoader.Instance.OnSceneLoadComplete.Invoke ();
			Time.timeScale = 1;
			SceneLoader.Instance.backgroundFader.fader.SetTrigger("FadeOut");
			gameObject.SetActive (false);
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