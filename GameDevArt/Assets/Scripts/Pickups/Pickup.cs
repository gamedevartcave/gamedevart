using UnityEngine;
using UnityEngine.Events;

namespace CityBashers
{
	public class Pickup : MonoBehaviour 
	{
		/// <summary>
		/// The amount of value this pickup adds.
		/// </summary>
		public int Amount = 20;

		/// <summary>
		/// The type of pickup.
		/// </summary>
		public PickupType pickupType;
		public enum PickupType
		{
			Health,
			Magic
		}

		/// <summary>
		/// The object to destroy on pickup (optional).
		/// </summary>
		public GameObject objectToDestroy;

		/// <summary>
		/// Event for pickup.
		/// </summary>
		public UnityEvent OnPickup;

		/// <summary>
		/// As player collider enters trigger of pickup.
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerEnter (Collider other)
		{
			if (other == PlayerController.Instance.playerCol)
			{
				switch (pickupType)
				{
					case PickupType.Health:
						
						if (PlayerController.Instance.health < PlayerController.Instance.MaximumHealth)
						{
							PlayerController.Instance.health += Amount;
							OnPickup.Invoke ();
							Destroy (objectToDestroy);
						}

						break;

					case PickupType.Magic:
					
						if (PlayerController.Instance.magic < PlayerController.Instance.MaximumMagic)
						{
							PlayerController.Instance.magic += Amount;
							OnPickup.Invoke ();
							Destroy (objectToDestroy);
						}

						break;
				}
			}
		}
	}
}
