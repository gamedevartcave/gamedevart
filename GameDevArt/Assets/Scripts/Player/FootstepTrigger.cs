﻿using UnityEngine;

namespace CityBashers
{
	public class FootstepTrigger : MonoBehaviour 
	{
		public string[] tags = new string[] {"Ground", "Scenery"};

		void OnTriggerEnter (Collider other)
		{
			for (int i = 0; i < tags.Length; i++)
			{
				if (other.tag == tags[i])
				{
					PlayerController.instance.OnFootstep.Invoke ();
					break;
				}
			}
		}
	}
}
