using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;

public class GameController : MonoBehaviour 
{
	public static GameController instance { get; private set; }
	[ReadOnlyAttribute] public bool isPaused;
	public UnityEvent OnPause;
	public UnityEvent OnUnpause;

	private bool trackTime;
	private float finalTime;
	private float startTime;
	public UnityEvent OnTimerTrackingStarted;
	public UnityEvent OnTimerTrackingStopped;

	public CameraShake camShakeScript;

	private PlayerActions playerActions;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		playerActions = InControlActions.instance.playerActions;
	}

	void Update ()
	{
		if (playerActions.Pause.WasPressed)
		{
			CheckPause ();
		}
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
			Time.timeScale = 0;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			OnPause.Invoke ();
		}

		if (!isPaused)
		{
			Time.timeScale = TimescaleController.instance.targetTimeScale;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			OnUnpause.Invoke ();
		}
	}
}
