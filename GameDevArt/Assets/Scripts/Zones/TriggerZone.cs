using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour 
{
	public Collider PlayerCol;

	public UnityEvent OnTriggerEntered;
	public UnityEvent OnTriggerStaying;
	public UnityEvent OnTriggerExited;

	void OnTriggerEnter (Collider other)
	{
		OnTriggerEntered.Invoke ();
	}
		
	void OnTriggerStay (Collider other)
	{
		OnTriggerStaying.Invoke ();
	}

	void OnTriggerExit (Collider other)
	{
		OnTriggerExited.Invoke ();
	}
}
