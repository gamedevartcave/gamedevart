using UnityEngine;

public class DontDestroyOnLoadInit : MonoBehaviour 
{
	public static DontDestroyOnLoadInit Instance { get; private set; }

	[Tooltip ("Managers Prefab.")]
	public GameObject ManagersPrefab;
	public float Delay;
	private GameObject managers;

	void Awake ()
	{
		Instance = this;
		Time.timeScale = 1;
		Invoke ("DetectManagers", Delay);
	}

	public void DetectManagers ()
	{
		// If there is no MANAGERS GameObject present,
		// Create one and make it not destory on load.
		if (InitManager.Instance == null) 
		{
			managers = Instantiate (ManagersPrefab);
			managers.name = "MANAGERS";
			DontDestroyOnLoad (managers.gameObject); 
		}
	}
}