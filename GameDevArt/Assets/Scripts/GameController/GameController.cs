using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.PostProcessing;
using CityBashers;

public class GameController : MonoBehaviour 
{
	public static GameController instance { get; private set; }

	public CameraShake camShakeScript;

	[Header ("Post processing")]
	public PostProcessingProfile postProcessing;
	public float targetDofDistance;
	public float dofSmoothing = 5.0f;

	[Header ("Pausing")]
	[ReadOnlyAttribute] public bool isPaused;
	public MenuNavigation activeMenu;
	public UnityEvent OnPause;
	public UnityEvent OnUnpause;

	[Header ("Game timer")]
	public UnityEvent OnTimerTrackingStarted;
	public UnityEvent OnTimerTrackingStopped;
	private bool trackTime;
	private float finalTime;
	private float startTime;

	private PlayerActions playerActions;

	void Awake ()
	{
		instance = this;
		this.enabled = false;
	}

	void Start ()
	{
		playerActions = InControlActions.instance.playerActions;
	}

	void Update ()
	{
		if (playerActions.Pause.WasPressed)
		{
			if (PlayerController.instance.lostAllHealth == false)
			{
				CheckPause ();
			}
		}

		if (isPaused == false)
		{
			GetDepthOfField ();
		}
	}

	void GetDepthOfField ()
	{
		var dofSettings = postProcessing.depthOfField.settings;

		RaycastHit hit;

		if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit, 1000))
		{
			if (playerActions.CamRot.Value.magnitude > 0.25f || playerActions.Move.Value.magnitude > 0.25f)
			{
				targetDofDistance = Vector3.Distance (Camera.main.transform.position, hit.point);
			} 

			else
			
			{
				targetDofDistance = Vector3.Distance (
					Camera.main.transform.position, 
					PlayerController.instance.transform.position);
			}
		}

		dofSettings.focusDistance = Mathf.Lerp (
			dofSettings.focusDistance, 
			targetDofDistance, 
			Mathf.Clamp (Time.deltaTime * dofSmoothing, 0, 0.2f)
		);

		postProcessing.depthOfField.settings = dofSettings;

		#if UNITY_EDITOR
		Debug.DrawRay (Camera.main.transform.position, Camera.main.transform.forward, Color.blue);
		Debug.DrawLine (Camera.main.transform.position, hit.point, Color.gray);
		#endif
	}

	public void StartTrackingTime ()
	{
		if (trackTime == false)
		{
			startTime = Time.time;
			trackTime = true;
			Debug.Log ("Started tracking time.");
			OnTimerTrackingStarted.Invoke ();
		}
	}

	public void StopTrackingTime ()
	{
		if (trackTime == true)
		{
			finalTime = Time.time - startTime;
			trackTime = false;

			int hours = Mathf.FloorToInt (finalTime / 3600);
			int minutes = Mathf.FloorToInt (finalTime / 60);
			int seconds = Mathf.FloorToInt (finalTime % 60);
			int milliseconds = 
				Mathf.RoundToInt ((finalTime - Mathf.FloorToInt (finalTime)) * 1000);

			Debug.Log ("Tracked time is: " + 
				hours + " hours, " + 
				minutes + " minutes, " + 
				seconds + " seconds, " + 
				milliseconds + " milliseconds"
			);

			OnTimerTrackingStopped.Invoke ();
		}
	}

	void CheckPause ()
	{
		isPaused = !isPaused;

		if (isPaused)
		{
			DoPause ();
			OnPause.Invoke ();
		}

		if (!isPaused)
		{
			DoUnpause ();
			OnUnpause.Invoke ();
		}
	}

	public void SetActiveMenu (MenuNavigation newActiveMenu)
	{
		activeMenu = newActiveMenu;
	}

	public void DoPause ()
	{
		isPaused = true;
		Time.timeScale = 0;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	public void DoUnpause ()
	{
		Time.timeScale = TimescaleController.instance.targetTimeScale;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		isPaused = false;
	}
}
