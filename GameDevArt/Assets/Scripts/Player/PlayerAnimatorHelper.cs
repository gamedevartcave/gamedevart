using UnityEngine;

namespace CityBashers
{
	public class PlayerAnimatorHelper : MonoBehaviour
	{
		private Animator PlayerAnim;

		private void Awake()
		{
			PlayerAnim = GetComponent<Animator>();
		}

		public void Footstep()
		{
			PlayerController.Instance.GetFootStepSound();
		}
	}
}
