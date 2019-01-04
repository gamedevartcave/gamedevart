using System.Collections;
using System.Collections.Generic;
using CityBashers;
using UnityEngine;

namespace CityBashers
{
    public class AirBourneState : StateMachineBehaviour
    {
        private PlayerController playerController;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerController == null)
            {
                playerController = PlayerController.instance;
            }

            HandleAirborneMovement();
        }

        void HandleAirborneMovement()
        {
            // Apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * playerController.gravityMultiplier) - Physics.gravity;
            playerController.playerRb.AddForce(extraGravityForce);

            // Handle air control.
            float airControlForce = playerController.airControl * playerController.playerActions.Move.Value.magnitude;
            playerController.playerRb.AddRelativeForce(
                new Vector3(0, 0, Mathf.Abs(airControlForce)),
                ForceMode.Acceleration);
        }
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}