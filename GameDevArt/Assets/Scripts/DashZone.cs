using UnityEngine;

namespace CityBashers
{
	public class DashZone : MonoBehaviour
	{
		public static DashZone Instance { get; private set; }
		[ReadOnly] public bool enemyNearby;

		private void Awake()
		{
			Instance = this;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Enemy")
			{
				enemyNearby = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.tag == "Enemy")
			{
				enemyNearby = false;
			}
		}
	}
}
