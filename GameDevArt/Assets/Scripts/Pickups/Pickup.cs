using UnityEngine;
using UnityEngine.Events;
using CityBashers;

public class Pickup : MonoBehaviour 
{
	public int Amount = 20;
	public PickupType pickupType;
	public enum PickupType
	{
		Health,
		Magic
	}

	public GameObject objectToDestroy;
	public UnityEvent OnPickup;

	void OnTriggerEnter (Collider other)
	{
		if (other == PlayerController.instance.playerCol)
		{
			switch (pickupType)
			{
			case PickupType.Health:
				if (PlayerController.instance.health < PlayerController.instance.MaximumHealth)
				{
					PlayerController.instance.health += Amount;
					OnPickup.Invoke ();
					Destroy (objectToDestroy);
				}
				break;
			case PickupType.Magic:
				if (PlayerController.instance.magic < PlayerController.instance.MaximumMagic)
				{
					PlayerController.instance.magic += Amount;
					OnPickup.Invoke ();
					Destroy (objectToDestroy);
				}
				break;
			}
		}
	}
}
