using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunState
{
    public enum StateTrigger
    {
        Fire,
        TimeToFireComplete,
        OutOfAmmo,
        AddAmmo,
    }

    public class StateController 
    {

        public Idle idle;
        public InUse inUse;
        public OutOfAmmo outOfAmmo;

        private State state;

        public bool CanShoot {  get => state == idle; }

        public bool HasAmmo { get => state != outOfAmmo; }

        public StateController() 
        {
            idle = new Idle();
            inUse = new InUse();
            outOfAmmo = new OutOfAmmo();


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

        public void Reset() 
        {
            state = idle;
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
                case StateTrigger.OutOfAmmo:
                    Exit();
                    return stateController.outOfAmmo;
                case StateTrigger.Fire:
                    Exit();
                    return stateController.inUse;
                default:
                    return null;
            }
        }
    }

    public class OutOfAmmo : State
    {
        public override string Name { get => "OutOfAmmo"; }
        public override State HandleTrigger(StateController stateController, StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.AddAmmo:
                    Exit();
                    return stateController.idle;
                default:
                    return null;
            }
        }
    }
}