using System.Collections;
using System.Collections.Generic;
using CityBashers;
using UnityEngine;

namespace CityBashers
{
    public class AirBourneState : StateMachineBehaviour
    {
        private PlayerController pc;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // TODO fix hack
            if (pc == null)
            {
                pc = PlayerController.instance;
            }
            
            HandleAirborneMovement();
            AirBourneAnimation(animator);
        }

        void HandleAirborneMovement()
        {
            // Apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * pc.gravityMultiplier) - Physics.gravity;
            pc.playerRb.AddForce(extraGravityForce);

			// Handle air control.
			float airControlForce = pc.airControl * pc.MoveAxis.magnitude;
			pc.playerRb.AddRelativeForce(
                new Vector3(0, 0, Mathf.Abs(airControlForce)),
                ForceMode.Acceleration);
        }

        void AirBourneAnimation(Animator playerAnim)
        {
            playerAnim.SetFloat("Jump", pc.playerRb.velocity.y);

            CamPosBasedOnAngle.instance.offset = new Vector2(
                CamPosBasedOnAngle.instance.offset.x,
                Mathf.Lerp(CamPosBasedOnAngle.instance.offset.y,
                    Mathf.Min(pc.playerRb.velocity.y * CamPosBasedOnAngle.instance.offsetMult, 0),
                    2 * Time.deltaTime)
            );
        }
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //if (Time.deltaTime > 0)
            //{
            //    Vector3 v = (animator.deltaPosition * pc.moveSpeedMultiplier) / Time.deltaTime;

            //    // Preserve the existing y part of the current velocity.
            //    v.y = pc.playerRb.velocity.y;
            //    pc.playerRb.velocity = v;
            //}
        }

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}