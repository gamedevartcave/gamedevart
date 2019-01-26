using UnityEngine;

namespace CityBashers
{
	public class BackgroundFader : MonoBehaviour
	{
		public static BackgroundFader Instance { get; private set; }

		public Animator fader;
	}
}
