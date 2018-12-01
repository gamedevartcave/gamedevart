using UnityEngine;

namespace CityBashers
{
	public class AudioSync : MonoBehaviour 
	{
		public AudioSource master; // The AudioSource to follow.
		public AudioSource slave; // The following AudioSource.

		// Syncronizes audio source timing with another audio source.
		void Update () 
		{
			// Checks if either audio source is playing.
			if (slave.isPlaying == true || master.isPlaying == true) 
			{
				if (master.timeSamples > slave.timeSamples) 
				{
					slave.timeSamples = slave.timeSamples % master.timeSamples;
				}

				else
					
				{
					slave.timeSamples = master.timeSamples; // Match time samples.
				}
			}
		}
	}
}