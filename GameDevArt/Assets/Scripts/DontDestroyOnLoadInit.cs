using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DontDestroyOnLoadInit : MonoBehaviour 
{
	public static DontDestroyOnLoadInit Instance { get; private set; }

	[Tooltip ("Managers Prefab.")]
	public GameObject ManagersPrefab;
	public float Delay;
	public float initializeWaitTime = 0.5f;
	private GameObject managers;
	public UnityEvent OnInitialize;

	private WaitForSeconds initializeWait;

	void Awake ()
	{
		Instance = this;
		Time.timeScale = 1;
		initializeWait = new WaitForSeconds (initializeWaitTime);
		Invoke ("DetectManagers", Delay);
	}

	public void SetInstance ()
	{
		//Instance = this;
		//SceneLoader.Instance.OnSceneLoadComplete.AddListener (OnSceneLoadComplete);
		//SceneLoader.Instance.OnInitialize.AddListener (OnInitialized);
	}

	public void DetectManagers ()
	{
		// If there is no MANAGERS GameObject present,
		// Create one and make it not destory on load.
		if (InitManager.Instance == null)
		{
			managers = Instantiate (ManagersPrefab); // Includes the InitManager.
			managers.name = "MANAGERS";
			DontDestroyOnLoad (managers.gameObject); 

		}

		//SceneLoader.Instance.OnInitialize.AddListener (OnInitialized);

		//StartCoroutine (Initialize ());
	}

	IEnumerator Initialize ()
	{
		yield return initializeWait;
		OnInitialize.Invoke ();
		SceneLoader.Instance.OnSceneLoadComplete.Invoke ();
	}

	public void LoadData ()
	{
		SaveAndLoadScript.Instance.InitializeLoad ();
	}

	public void OnInitialized ()
	{
		StartCoroutine (Initialize ());
	}
}