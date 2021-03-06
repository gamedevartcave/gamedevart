﻿using UnityEngine;

namespace CityBashers
{
    public class GroundState : StateMachineBehaviour
    {
        public float  runCycleLegOffset = 0.2f; // Specific to the character in sample assets, will need to be modified to work with others

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineEnter(animator, stateMachinePathHash);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
			if (PlayerController.Instance != null)
			{
				PlayerController.Instance.jumpState = 0;
			}
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Move(pc.move, animator);
			Move(PlayerController.Instance.move, animator);
        }

        public void Move(Vector3 move, Animator playerAnim)
        {
            UpdateAnimator(move, playerAnim);
        }

        void UpdateAnimator(Vector3 move, Animator playerAnim)
        {
            // Calculate which leg is behind, so as to leave that leg trailing in the jump animation.
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle = Mathf.Repeat(
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);

			float jumpLeg = (runCycle < 0.5f ? 1 : -1) * PlayerController.Instance.forwardAmount;

            // Update the animator parameters.
			playerAnim.SetFloat("Forward", PlayerController.Instance.forwardAmount, 0.1f, Time.deltaTime);
			//playerAnim.SetFloat("Turn", PlayerController.Instance.turnAmount, 0.1f, Time.deltaTime);

			// Update grounded state.
			playerAnim.SetBool("OnGround", PlayerController.Instance.isGrounded);

			if (PlayerController.Instance.isGrounded == true)
            {
                playerAnim.SetFloat("JumpLeg", jumpLeg);
                playerAnim.SetFloat("Jump", 0);

                CamPosBasedOnAngle.Instance.offset = new Vector2(
                    CamPosBasedOnAngle.Instance.offset.x,
                    Mathf.Lerp(CamPosBasedOnAngle.Instance.offset.y, 0, 2 * Time.deltaTime));
            }

            // The anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // Which affects the movement speed because of the root motion.
            if (playerAnim.GetBool("Dodging") == false)
            {
				if (PlayerController.Instance.isGrounded == true && move.sqrMagnitude > 0)
                {
					playerAnim.speed = PlayerController.Instance.animSpeedMultiplier;
                }

                else // Don't use anim speed while airborne.

                {
                    playerAnim.speed = 1;
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Overrides the default root motion.
            // Allows us to modify the positional speed before it's applied.
            if (Time.deltaTime > 0)
            {
				if (PlayerController.Instance != null)
				{
					if (PlayerController.Instance.isGrounded)
					{
						Vector3 v = (animator.deltaPosition * PlayerController.Instance.moveSpeedMultiplier) / Time.deltaTime;

						// Preserve the existing y part of the current velocity.
						v.y = PlayerController.Instance.playerRb.velocity.y;
						PlayerController.Instance.playerRb.velocity = v;
					}
				}
            }
        }

        // OnStateIK is called right after Animator.OnAnimatorIK()
        override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Implement code that sets up animation IK (inverse kinematics)
        }
    }
}