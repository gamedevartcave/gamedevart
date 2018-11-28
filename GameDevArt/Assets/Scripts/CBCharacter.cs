using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBashers
{
    public enum CharacterStates
    {
        Unknown,
        Idle,
        Movement,
        Paused,
        Interacting,
        Death
    }

    public enum MovementStates
    {
        Unknown,
        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        KnockedDown,
        GetingUp,
        Dashing,
        Dogding
    }
    public enum CombatStates
    {
        Unknown,
        Idle,
        ChangingWeapon,
        Reloading,
        MeleeAttack,
        FireWeapon,
        Blocking,
        Parry
    }
    public class CBCharacter : MonoBehaviour
    {
        private StateMachine MovementState;
        private StateMachine CombatState;
        private StateMachine CharacterStates;

        // Use this for initialization
        void Start()
        {
            MovementState = new StateMachine();
            CombatState = new StateMachine();
            CharacterStates = new StateMachine();
        }

        // Update is called once per frame
        void Update()
        {
        MovementState.Update();
        CombatState.Update();
        CharacterStates.Update();
        }
        void FixedUpdate()
        {
            MovementState.FixedUpdate();
            CombatState.FixedUpdate();
            CharacterStates.FixedUpdate();
        }
    }
}
