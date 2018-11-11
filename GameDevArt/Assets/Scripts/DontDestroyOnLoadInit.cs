using UnityEngine;
using UnityEngine.Events;

public class DontDestroyOnLoadInit : MonoBehaviour 
{
	public static DontDestroyOnLoadInit Instance { get; private set; }

	[Tooltip ("Managers Prefab.")]
	public GameObject ManagersPrefab;
	public float Delay;
	private GameObject managers;
	public UnityEvent OnInitialize;

	void Awake ()
	{
		Instance = this;
		Time.timeScale = 1;
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

		OnInitialize.Invoke ();
	}
}