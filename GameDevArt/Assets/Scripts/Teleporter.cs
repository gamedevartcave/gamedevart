using UnityEngine;

public class Teleporter : MonoBehaviour
{
	[ReadOnlyAttribute] public int timesUsed;
	public int maxTimesUsed = 1;
	public ParticleSystem teleportParticles;

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject == PlayerController.instance.gameObject)
		{
			if (timesUsed < maxTimesUsed)
			{
				teleportParticles.Play ();
				timesUsed++;
			}
		}
	}
}
