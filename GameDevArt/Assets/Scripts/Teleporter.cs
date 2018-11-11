using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{
	public bool locked;
	[ReadOnlyAttribute] public int timesUsed;
	public int maxTimesUsed = 1;
	public ParticleSystem teleportParticles;
	public GameObject teleportLockParticles;
	private WaitForSeconds teleportWait;
	public float teleportWaitTime = 1;

	public Collider capsuleCollider;

	void Start ()
	{
		teleportWait = new WaitForSeconds (teleportWaitTime);
		StartCoroutine (UpdateTeleportLockState ());
	}

	IEnumerator UpdateTeleportLockState ()
	{
		yield return teleportWait;

		// Check whether this teleporter should be locked.
		// locked = save script level unlocked?

		//if (capsuleCollider.enabled != locked)
		//{
			capsuleCollider.enabled = locked;
			teleportLockParticles.SetActive (locked);
		//}

		StartCoroutine (UpdateTeleportLockState ());
	}

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
