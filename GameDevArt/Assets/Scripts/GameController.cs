using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class GameController : MonoBehaviour 
{
	public static GameController instance { get; private set; }
	[ReadOnlyAttribute] public bool isPaused;

	private bool trackTime;
	private float finalTime;
	private float startTime;

	public CameraShake camShakeScript;

	private PlayerActions playerActions;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		playerActions = InControlActions.instance.playerActions;
		StartTrackingTime ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.T))
		{
			StopTrackingTime ();
		}

		if (Input.GetKeyDown (KeyCode.K))
		{
			camShakeScript.Shake ();
		}

		if (playerActions.Pause.WasPressed)
		{
			CheckPause ();
		}
	}

	public void Vibrate ()
	{
		
	}

	public void StartTrackingTime ()
	{
		if (trackTime == false)
		{
			startTime = Time.time;
			trackTime = true;
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
		}
	}

	void CheckPause ()
	{
		isPaused = !isPaused;

		if (isPaused)
		{
			Time.timeScale = 0;
		}

		if (!isPaused)
		{
			Time.timeScale = TimescaleController.instance.targetTimeScale;
		}
	}
}
