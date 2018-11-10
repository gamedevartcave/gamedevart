using UnityEngine;
using UnityEngine.Events;

public class HealthPickup : MonoBehaviour 
{
	public int healthAmount = 20;
	public GameObject objectToDestroy;
	public UnityEvent OnHealthPickup;

	void OnTriggerEnter (Collider other)
	{
		if (other == PlayerController.instance.playerCol)
		{
			if (PlayerController.instance.health < PlayerController.instance.MaximumHealth)
			{
				PlayerController.instance.health += healthAmount;
				OnHealthPickup.Invoke ();
				Destroy (objectToDestroy);
			}
		}
	}
}
