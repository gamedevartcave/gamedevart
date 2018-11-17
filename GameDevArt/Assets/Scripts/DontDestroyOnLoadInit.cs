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
		Instance = this;
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

		StartCoroutine (Initialize ());
	}

	IEnumerator Initialize ()
	{
		yield return initializeWait;
		OnInitialize.Invoke ();
		SceneLoader.Instance.OnSceneLoadComplete.Invoke ();
	}
}