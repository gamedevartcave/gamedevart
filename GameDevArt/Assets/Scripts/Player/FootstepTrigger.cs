using UnityEngine;

public class FootstepTrigger : MonoBehaviour 
{
	public string[] tags = new string[] {"Ground", "Scenery"};

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Ground")
		{
			PlayerController.instance.OnFootstep.Invoke ();
		}
	}
}
