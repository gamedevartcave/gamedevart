using UnityEngine;

public class FootstepTrigger : MonoBehaviour 
{
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Ground")
		{
			PlayerController.instance.OnFootstep.Invoke ();
		}
	}
}
