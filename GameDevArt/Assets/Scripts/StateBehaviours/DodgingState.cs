﻿using UnityEngine;

namespace CityBashers
{
    public class DodgingState : StateMachineBehaviour
    {
		private Vector3 relativeDodgeDir;

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineEnter(animator, stateMachinePathHash);
        }

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			relativeDodgeDir = PlayerController.Instance.transform.InverseTransformDirection(
				PlayerController.Instance.transform.forward *
				(PlayerController.Instance.MoveAxis.sqrMagnitude > 0 ? 1 : -1) *
				PlayerController.Instance.dodgeSpeed * Time.unscaledDeltaTime);

			// Update UI elements.
			PlayerController.Instance.magic -= PlayerController.Instance.dodgeMagicCost;
			PlayerController.Instance.PlayerUI.SetTrigger("Show");

			// Tweak movement amounts.
			PlayerController.Instance.moveMultiplier *= PlayerController.Instance.dodgeSpeedupFactor;
			PlayerController.Instance.animSpeedMultiplier *= PlayerController.Instance.dodgeSpeedupFactor;
			PlayerController.Instance.movingTurnSpeed *= PlayerController.Instance.dodgeSpeedupFactor;
			PlayerController.Instance.stationaryTurnSpeed *= PlayerController.Instance.dodgeSpeedupFactor;

			animator.updateMode = AnimatorUpdateMode.UnscaledTime;

			// Set dodge time.
			TimescaleController.Instance.targetTimeScale = PlayerController.Instance.dodgeTimeScale;

			// Call events.
			PlayerController.Instance.OnDodgeBegan.Invoke();
			PlayerController.Instance.nextDodge = Time.time + PlayerController.Instance.dodgeRate;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {            
            DodgeAction(animator);
        }

        void DodgeAction(Animator playerAnim)
        {
            // Game is not paused.
            if (GameController.Instance.isPaused == false)
            {
				// Decrease time left of dodging.
				PlayerController.Instance.transform.Translate(relativeDodgeDir, Space.Self);
				PlayerController.Instance.move = Vector3.zero;

				playerAnim.SetBool("Dodging", true);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
			// Game is not paused.
			if (GameController.Instance.isPaused == false)
			{
				TimescaleController.Instance.targetTimeScale = 1; // Reset time scale.

				// Reset dodging animation parameters.
				animator.ResetTrigger("Dodge");
				animator.SetBool("Dodging", false);

				PlayerController.Instance.moveMultiplier /= PlayerController.Instance.dodgeSpeedupFactor;
				PlayerController.Instance.animSpeedMultiplier /= PlayerController.Instance.dodgeSpeedupFactor;
				PlayerController.Instance.movingTurnSpeed /= PlayerController.Instance.dodgeSpeedupFactor;
				PlayerController.Instance.stationaryTurnSpeed /= PlayerController.Instance.dodgeSpeedupFactor;

				animator.updateMode = AnimatorUpdateMode.Normal;

				PlayerController.Instance.OnDodgeEnded.Invoke();
				PlayerController.Instance.isDodging = false;
			}
		}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Implement code that processes and affects root motion
        }

        // OnStateIK is called right after Animator.OnAnimatorIK()
        override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Implement code that sets up animation IK (inverse kinematics)
        }
    }
}
