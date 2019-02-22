using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
	public UnityEvent OnEnter;
	public UnityEvent OnExit;

	private void OnTriggerEnter(Collider other)
	{
		OnEnter.Invoke();
	}

	private void OnTriggerExit(Collider other)
	{
		OnExit.Invoke();
	}
}
