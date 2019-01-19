using UnityEngine;

namespace CityBashers
{
	public class FootstepTrigger : MonoBehaviour 
	{
		/// <summary>
		/// The tags which allow the player to make sounds when walking on them.
		/// </summary>
		public string[] tags = new string[] {"Ground", "Scenery"};

		void OnTriggerEnter (Collider other)
		{
			for (int i = 0; i < tags.Length; i++)
			{
				if (other.tag == tags[i])
				{
					PlayerController.Instance.OnFootstep.Invoke ();
					break;
				}
			}
		}
	}
}
