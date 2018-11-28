using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CityBashers
{
    public abstract class StateBehaviour
    {
        public PlayerController PlayerController { get; private set; }

        public StateBehaviour(PlayerController controller)
        {
            PlayerController = controller;
        }

        protected Transform transform
        {
            get { return PlayerController.transform; }
        }

        protected static T Instantiate<T>(T original, Transform parent) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate<T>(original, parent, false);
        }

        public virtual void OnStateEnter(StateMachine stateMachine) { }
        public virtual void OnStateUpdate(StateMachine stateMachine) { }
        public virtual void OnStateFixedUpdate(StateMachine stateMachine) { }
        public virtual void OnStateExit(StateMachine stateMachine) { }
        public virtual void OnBackgroundUpdate() { }

        public virtual void OnTriggerEnter(Collider other) { }
        public virtual void OnTriggerStay(Collider other) { }
        public virtual void OnCollisionEnter(Collision other) { }
        public virtual void OnCollisionStay(Collision other) { }
        public virtual void OnCollisionExit(Collision other) { }
    }

    public class StateMachine
    {
        public StateBehaviour PreviousState { get; private set; }
        public StateBehaviour CurrentState { get; private set; }
        private UnityEvent StatesBackgroundUpdate;

        public void Update()
        {
            CurrentState.OnStateUpdate(this);
            StatesBackgroundUpdate.Invoke();
        }

        public void FixedUpdate()
        {
            CurrentState.OnStateFixedUpdate(this);
        }

        public void ChangeState(StateBehaviour newState)
        {
            PreviousState = CurrentState;
            CurrentState.OnStateExit(this);
            CurrentState = newState;
            CurrentState.OnStateEnter(this);
        }
    }
}
