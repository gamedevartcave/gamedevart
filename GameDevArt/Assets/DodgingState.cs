using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBashers
{
    public class DodgingState : StateMachineBehaviour
    {
        private PlayerController pc;
        private float dodgeTimeRemain;
        private float nextDodge;
        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineEnter(animator, stateMachinePathHash);
            pc = PlayerController.instance;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {            
            // TODO fix hack
            if (pc == null)
            {
                pc = PlayerController.instance;
            }

            //DodgeAction(animator);
        }

        void DodgeAction(Animator playerAnim)
        {
            if ((pc.magic > pc.dodgeMagicCost || pc.unlimitedMagic))// &&
               //(pc.playerActions.DodgeLeft.WasPressed || pc.playerActions.DodgeRight.WasPressed))
            {
                // Get dodge angle.
                // Assign to player animation.
                if (Time.time > nextDodge)
                {
                    pc.isDodging = true;

                    //playerAnim.SetFloat("DodgeDir", pc.playerActions.Dodge.Value);
                    playerAnim.SetBool("Dodging", true);

                    playerAnim.SetTrigger("Dodge");
                    pc.magic -= pc.dodgeMagicCost;
                    pc.PlayerUI.SetTrigger("Show");

                    pc.moveMultiplier *= pc.dodgeSpeedupFactor;
                    pc.animSpeedMultiplier *= pc.dodgeSpeedupFactor;
                    pc.movingTurnSpeed *= pc.dodgeSpeedupFactor;
                    pc.stationaryTurnSpeed *= pc.dodgeSpeedupFactor;

                    playerAnim.updateMode = AnimatorUpdateMode.UnscaledTime;

                    dodgeTimeRemain = pc.DodgeTimeDuration;
                    TimescaleController.instance.targetTimeScale = pc.dodgeTimeScale;

                    pc.OnDodgeBegan.Invoke();
                    nextDodge = Time.time + pc.dodgeRate;

                    //Debug.Log ("Dodged " + playerActions.Dodge.Value);
                }

                else // Not able to dodge yet.

                {
                }
            }

            // Dodge time ran out.
            if (dodgeTimeRemain <= 0)
            {
                // If is dodging.
                if (pc.isDodging == true)
                {
                    // Game is not paused.
                    if (GameController.instance.isPaused == false)
                    {
                        TimescaleController.instance.targetTimeScale = 1; // Reset time scale.

                        // Reset dodging animation parameters.
                        //playerAnim.SetFloat("DodgeDir", 0);
                        playerAnim.ResetTrigger("Dodge");
                        playerAnim.SetBool("Dodging", false);

                        pc.moveMultiplier /= pc.dodgeSpeedupFactor;
                        pc.animSpeedMultiplier /= pc.dodgeSpeedupFactor;
                        pc.movingTurnSpeed /= pc.dodgeSpeedupFactor;
                        pc.stationaryTurnSpeed /= pc.dodgeSpeedupFactor;
                        
                        playerAnim.updateMode = AnimatorUpdateMode.Normal;
                        
                        pc.OnDodgeEnded.Invoke();
                        pc.isDodging = false;
                    }
                }
            }

            else // There is dodge time.

            {

				Debug.Log ("isDodge time");
                // Game is not paused.
                if (GameController.instance.isPaused == false)
                {
                    // Decrease time left of dodging.
                    dodgeTimeRemain -= Time.unscaledDeltaTime;

                    Vector3 relativeDodgeDir = pc.transform.InverseTransformDirection(
                        pc.transform.forward *
                        (pc.MoveAxis.sqrMagnitude > 0 ? 1 : -1) *
                        pc.dodgeSpeed * Time.unscaledDeltaTime);

                    pc.transform.Translate(relativeDodgeDir, Space.Self);

					playerAnim.SetBool("Dodging", true);
					Debug.Log ("Dodging");
                }
            }
        }


        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
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
