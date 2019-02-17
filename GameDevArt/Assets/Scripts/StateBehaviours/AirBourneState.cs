using UnityEngine;

namespace CityBashers
{
    public class AirBourneState : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
		}

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AirBourneAnimation(animator);
        }


        void AirBourneAnimation(Animator playerAnim)
        {
            playerAnim.SetFloat("Jump", PlayerController.Instance.playerRb.velocity.y);

			/*
            CamPosBasedOnAngle.Instance.offset = new Vector2(
                CamPosBasedOnAngle.Instance.offset.x,
                Mathf.Lerp(CamPosBasedOnAngle.Instance.offset.y,
                   // Mathf.Min(PlayerController.Instance.playerRb.velocity.y * CamPosBasedOnAngle.Instance.offsetMult, 0),
					 PlayerController.Instance.playerRb.velocity.y,
					2 * Time.deltaTime)
            );
			*/
        }
       
		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {  
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        // OnStateIK is called right after Animator.OnAnimatorIK()
        override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Implement code that sets up animation IK (inverse kinematics)
        }
    }
}