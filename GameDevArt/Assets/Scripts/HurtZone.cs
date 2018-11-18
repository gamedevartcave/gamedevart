using UnityEngine;
using System.Collections;

public class HurtZone : MonoBehaviour 
{
	[ReadOnlyAttribute] public bool isInsideHurtZone;
	public Collider playerCol;
	public bool damageOnEnter;

	public int damageAmount = 10;

	public float damageRate;
	private WaitForSeconds damageWait;

	public float damageStartWaitTime = 1;
	private WaitForSeconds damageStartWait;

	public Animator PlayerUI;

	void Start ()
	{
		damageStartWait = new WaitForSeconds (damageStartWaitTime);
		damageWait = new WaitForSeconds (damageRate);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other == playerCol)
		{
			isInsideHurtZone = true;
			StartCoroutine (TakeDamage ());
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other == playerCol)
		{
			isInsideHurtZone = false;
			PlayerUI.SetTrigger ("Show");
			StopCoroutine (TakeDamage ());
		}
	}

	IEnumerator TakeDamage ()
	{
		if (damageOnEnter == false) // Inflict damage immediately.
		{
			yield return damageStartWait; // Give start wait.

		} 

		// Inflict inside hurt zone periodically.
		while (isInsideHurtZone == true)
		{
			PlayerController.instance.health -= damageAmount;
			PlayerUI.SetTrigger ("Show");
			yield return damageWait;
		}
	}
}
