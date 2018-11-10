using UnityEngine;

public class GameController : MonoBehaviour 
{
	private bool trackTime;
	private float finalTime;
	private float startTime;

	void Start ()
	{
		StartTrackingTime ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.T))
		{
			StopTrackingTime ();
		}
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
}
