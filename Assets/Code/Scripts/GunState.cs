using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunState
{
    public enum StateTrigger
    {
        Fire,
        TimeToFireComplete,
    }

    public class StateController 
    {

        public Idle idle;
        public InUse inUse;

        private State state;

        public StateController() 
        {
            idle = new Idle();
            inUse = new InUse();

            state = idle;
        }

        public void HandleTrigger(StateTrigger trigger)
        {
            State newState = state.HandleTrigger(this, trigger);
            if (newState != null)
            {
                state = newState;
                newState.Enter();
            }
        }
    }


    public abstract class State
    {
        public event StateChangeHandler notifyListenersEnter;
        public event StateChangeHandler notifyListenersExit;

        public virtual string Name { get; }

        public abstract State HandleTrigger(StateController stateController, StateTrigger trigger);

        public void PrintStateEnter()
        {
            Debug.Log("GunState >  " + Name);
        }

        public virtual void Enter()
        {
            PrintStateEnter();
            notifyListenersEnter?.Invoke();
        }

        public void Exit()
        {
            notifyListenersExit?.Invoke();
        }

    }

    public class Idle : State
    {
        public override string Name { get => "Idle"; }
        public override State HandleTrigger(StateController stateController, StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.Fire:
                    Exit();
                    return stateController.inUse;
                default:
                    return null;
            }
        }
    }

    public class InUse : State
    {
        public override string Name { get => "InUse"; }
        public override State HandleTrigger(StateController stateController, StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.TimeToFireComplete:
                    Exit();
                    return stateController.idle;
                default:
                    return null;
            }
        }
    }
}