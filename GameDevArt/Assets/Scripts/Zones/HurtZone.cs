using UnityEngine;
using System.Collections;
using CityBashers;

namespace CityBashers
{
	public class HurtZone : MonoBehaviour 
	{
		[ReadOnlyAttribute] public bool isInsideHurtZone;
		public Collider playerCol;
		public Animator PlayerUI;
		public bool damageOnEnter;

		public int damageAmount = 10;

		public float damageRate;
		private WaitForSeconds damageWait;

		public float damageStartWaitTime = 1;
		private WaitForSeconds damageStartWait;

		[Space (10)]
		public bool isHitStunZone;

		void Start ()
		{
			damageStartWait = new WaitForSeconds (damageStartWaitTime);
			damageWait = new WaitForSeconds (damageRate);
		}

		#region Enter
		void OnCollisionEnter (Collision other)
		{
			if (other.collider == playerCol)
			{
				isInsideHurtZone = true;
				PlayerUI.SetTrigger ("Show");
				HitStunSequence ();
				StartCoroutine (TakeDamage ());
			}
		}
			
		void OnTriggerEnter (Collider other)
		{
			if (other == playerCol)
			{
				isInsideHurtZone = true;
				PlayerUI.SetTrigger ("Show");
				HitStunSequence ();
				StartCoroutine (TakeDamage ());
			}
		}
		#endregion

		#region Stay
		void OnCollisionStay (Collision other)
		{
			if (other.collider == playerCol)
			{
				isInsideHurtZone = true;
				PlayerUI.SetTrigger ("Show");
				HitStunSequence ();
			}
		}

		void OnTriggerStay (Collider other)
		{
			if (other == playerCol)
			{
				isInsideHurtZone = true;
				PlayerUI.SetTrigger ("Show");
				HitStunSequence ();
			}
		}
		#endregion
			
		#region Exit
		void OnCollisionExit (Collision other)
		{
			if (other.collider == playerCol)
			{
				isInsideHurtZone = false;
				PlayerUI.SetTrigger ("Show");
				HitStunSequence ();
				StopCoroutine (TakeDamage ());
			}
		}

		void OnTriggerExit (Collider other)
		{
			if (other == playerCol)
			{
				isInsideHurtZone = false;
				PlayerUI.SetTrigger ("Show");
				HitStunSequence ();
				StopCoroutine (TakeDamage ());
			}
		}
		#endregion

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

		void HitStunSequence ()
		{
			if (isHitStunZone == true)
			{
				PlayerController.instance.DoHitStun ();
			}
		}
	}
}
