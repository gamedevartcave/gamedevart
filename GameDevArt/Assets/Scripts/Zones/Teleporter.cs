using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using CityBashers;

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

	public float teleportDelayTime = 1;
	private WaitForSeconds teleportYield;

	public UnityEvent OnTeleportEnter;
	public UnityEvent OnTeleportEnterDelayed;
	public UnityEvent OnTeleportComplete;

	void Start ()
	{
		teleportWait = new WaitForSeconds (teleportWaitTime);
		teleportYield = new WaitForSeconds (teleportDelayTime);
		StartCoroutine (UpdateTeleportLockState ());
	}

	IEnumerator UpdateTeleportLockState ()
	{
		while (true)
		{
			capsuleCollider.enabled = locked;
			teleportLockParticles.SetActive (locked);
			yield return teleportWait;
		}
	}

	IEnumerator TeleportEnterDelay ()
	{
		yield return teleportYield;
		OnTeleportEnterDelayed.Invoke ();
		yield return teleportYield;
		yield return teleportYield;
		OnTeleportComplete.Invoke ();
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject == PlayerController.instance.gameObject)
		{
			if (timesUsed < maxTimesUsed)
			{
				teleportParticles.Play ();
				timesUsed++;
				OnTeleportEnter.Invoke ();
				StartCoroutine (TeleportEnterDelay ());
			}
		}
	}
}
