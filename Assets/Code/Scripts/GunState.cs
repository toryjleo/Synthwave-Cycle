using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunState
{
    public enum StateTrigger
    {
        FireSingleShot,
        FireBurstShot,
        EnterTimeBetween,
        TimeToFireComplete,
        OutOfAmmo,
        AddAmmo,
    }

    public class StateController 
    {

        public Idle idle;
        public FireBurstShot fireBurstShot;
        public FireSingleShot fireSingleShot;
        public BetweenShots betweenShots;
        public OutOfAmmo outOfAmmo;

        private State state;

        public bool CanShoot {  get => state == idle; }

        public bool FiringBurstRounds { get => state == fireBurstShot; }

        public bool HasAmmo { get => state != outOfAmmo; }

        public StateController(int burstShotNum) 
        {
            idle           = new Idle(this);
            fireBurstShot  = new FireBurstShot(this, burstShotNum);
            fireSingleShot = new FireSingleShot(this);
            betweenShots   = new BetweenShots(this);
            outOfAmmo      = new OutOfAmmo(this);

            state = idle;
        }

        public void HandleTrigger(StateTrigger trigger)
        {
            State newState = state.HandleTrigger(trigger);
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

        protected StateController stateController;

        public State(StateController stateController) 
        {
            this.stateController = stateController;
        }

        public virtual string Name { get; }

        public abstract State HandleTrigger(StateTrigger trigger);

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
        public Idle(StateController stateController) : base(stateController)
        {
        }

        public override string Name { get => "Idle"; }
        public override State HandleTrigger(StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.FireBurstShot:
                    Exit();
                    stateController.fireBurstShot.Reset();
                    return stateController.fireBurstShot;
                case StateTrigger.FireSingleShot:
                    Exit();
                    return stateController.fireSingleShot;
                default:
                    return null;
            }
        }
    }

    public class FireBurstShot : State
    {
        public FireBurstShot(StateController stateController, int burstShotNum) : base(stateController)
        {
            this.burstMax = burstShotNum;
        }

        private int burstMax = 0;
        private int burstShotCount = 0;

        public override string Name { get => $"FireBurstShot, rounds remaining: {burstShotCount}/{burstMax}"; }
        public override State HandleTrigger(StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.FireBurstShot:
                    Exit();
                    return stateController.fireBurstShot;
                case StateTrigger.EnterTimeBetween:
                    Exit();
                    return stateController.betweenShots;
                case StateTrigger.OutOfAmmo:
                    Exit();
                    return stateController.outOfAmmo;
                default:
                    return null;
            }
        }

        public override void Enter()
        {
            burstShotCount++;
            base.Enter();
            if (burstShotCount == burstMax) 
            {
                stateController.HandleTrigger(StateTrigger.EnterTimeBetween);
            }
        }

        public void Reset() 
        {
            burstShotCount = 0;
        }
    }

    public class FireSingleShot : State
    {
        public FireSingleShot(StateController stateController) : base(stateController)
        {
        }

        public override string Name { get => "FireSingleShot"; }
        public override State HandleTrigger(StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.EnterTimeBetween:
                    Exit();
                    return stateController.betweenShots;
                case StateTrigger.OutOfAmmo:
                    Exit();
                    return stateController.outOfAmmo;
                default:
                    return null;
            }
        }
    }

    public class BetweenShots : State
    {
        public BetweenShots(StateController stateController) : base(stateController)
        {
        }

        public override string Name { get => "BetweenShots"; }
        public override State HandleTrigger(StateTrigger trigger)
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

    public class OutOfAmmo : State
    {
        public OutOfAmmo(StateController stateController) : base(stateController)
        {
        }

        public override string Name { get => "OutOfAmmo"; }
        public override State HandleTrigger(StateTrigger trigger)
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