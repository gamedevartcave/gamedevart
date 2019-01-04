using System;
using System.Collections;
using System.Collections.Generic;
using CityBashers;
using JetBrains.Annotations;
using UnityEngine;

namespace CityBashers
{



    public class GroundState : StateMachineBehaviour
    {
        private PlayerController playerController;
        private float turnAmount;
        private float forwardAmount;
        public Vector3 groundNormal;



        public float  runCycleLegOffset = 0.2f; // Specific to the character in sample assets, will need to be modified to work with others


        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineEnter(animator, stateMachinePathHash);
            playerController = PlayerController.instance;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // TODO fix hack
            if (playerController == null)
            {
                playerController = PlayerController.instance;
            }

            Move(playerController.move, animator);
        }

        public void Move(Vector3 move, Animator playerAnim)
        {
            // Vonvert the world relative moveInput vector into a local-relative
            // Turn amount and forward amount required to head in the desired direction.
            if (move.sqrMagnitude > 1f) move.Normalize();
            move = playerController.transform.InverseTransformDirection(move);
            move = Vector3.ProjectOnPlane(move, groundNormal);

            // Update turning.
            turnAmount = Mathf.Atan2(move.x, move.z);
            ApplyExtraTurnRotation();
            forwardAmount = move.z;

            // Control and velocity handling is different when grounded and airborne.
            if (playerController.isGrounded == true)
            {
                playerController.jumpState = 0;
            }

            else // Is airborne.

            {
                // HandleAirborneMovement();
            }

            // Send input and other state parameters to the animator
            UpdateAnimator(move, playerAnim);
        }

        void UpdateAnimator(Vector3 move, Animator playerAnim)
        {
            // Calculate which leg is behind, so as to leave that leg trailing in the jump animation.
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle = Mathf.Repeat(
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);

            float jumpLeg = (runCycle < 0.5f ? 1 : -1) * forwardAmount;

            // Update the animator parameters.
            playerAnim.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            playerAnim.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);

            // Update grounded state.
            playerAnim.SetBool("OnGround", playerController.isGrounded);

            if (playerController.isGrounded == true)
            {
                playerAnim.SetFloat("JumpLeg", jumpLeg);
                playerAnim.SetFloat("Jump", 0);

                CamPosBasedOnAngle.instance.offset = new Vector2(
                    CamPosBasedOnAngle.instance.offset.x,
                    Mathf.Lerp(CamPosBasedOnAngle.instance.offset.y, 0, 2 * Time.deltaTime)
                );
            }

            else // Is in mid air.

            {
                playerAnim.SetFloat("Jump", playerController.playerRb.velocity.y);

                CamPosBasedOnAngle.instance.offset = new Vector2(
                    CamPosBasedOnAngle.instance.offset.x,
                    Mathf.Lerp(CamPosBasedOnAngle.instance.offset.y,
                        Mathf.Min(playerController.playerRb.velocity.y * CamPosBasedOnAngle.instance.offsetMult, 0),
                        2 * Time.deltaTime)
                );
            }

            // The anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // Which affects the movement speed because of the root motion.
            if (playerAnim.GetBool("Dodging") == false)
            {
                if (playerController.isGrounded == true && move.sqrMagnitude > 0)
                {
                    playerAnim.speed = playerController.animSpeedMultiplier;
                }

                else // Don't use anim speed while airborne.

                {
                    playerAnim.speed = 1;
                }
            }
        }

        void ApplyExtraTurnRotation()
        {
            // Help the character turn faster (this is in addition to root rotation in the animation).
            float turnSpeed = Mathf.Lerp(playerController.stationaryTurnSpeed, playerController.movingTurnSpeed,
                forwardAmount);
            playerController.transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
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