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
        OverHeated,
        OverHeatComplete,
    }

    /// <summary>
    /// Manages states for a given gun
    /// </summary>
    public class StateController 
    {

        public Idle idle;
        public FireBurstShot fireBurstShot;
        public FireSingleShot fireSingleShot;
        public BetweenShots betweenShots;
        public OutOfAmmo outOfAmmo;
        public OverHeated overHeated;

        private State state;

        public bool printState = false;

        public bool CanShoot {  get => state == idle; }

        public bool IsOverHeated { get => state == overHeated; }

        /// <summary>
        /// True if currently firing a burst round
        /// </summary>
        public bool FiringBurstRounds { get => state == fireBurstShot; }

        public bool HasAmmo { get => state != outOfAmmo; }

        public StateController(int burstShotNum, bool printState = false) 
        {
            idle           = new Idle(this);
            fireBurstShot  = new FireBurstShot(this, burstShotNum);
            fireSingleShot = new FireSingleShot(this);
            betweenShots   = new BetweenShots(this);
            outOfAmmo      = new OutOfAmmo(this);
            overHeated     = new OverHeated(this);

            state = idle;

            this.printState = printState;
        }

        /// <summary>
        /// Sends a trigger to the current state. May trigger a state change.
        /// </summary>
        /// <param name="trigger">Trigger to send to state</param>
        public void HandleTrigger(StateTrigger trigger)
        {
            State newState = state.HandleTrigger(trigger);
            if (newState != null)
            {
                if (printState)
                {
                    state.PrintStateEnter();
                }
                state = newState;
                newState.Enter();
            }
        }

        /// <summary>
        /// Set the state machine to the initial configuration
        /// </summary>
        public void Reset() 
        {
            state = idle;
        }
    }

    /// <summary>
    /// Abstract gun state declaration
    /// </summary>
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

        /// <summary>
        /// Handles a trigger for this state
        /// </summary>
        /// <param name="trigger">Trigger to create a state transition</param>
        /// <returns>The new state if there is a transision</returns>
        public abstract State HandleTrigger(StateTrigger trigger);

        /// <summary>
        /// Prints the current state
        /// </summary>
        public void PrintStateEnter()
        {
            Debug.Log("GunState >  " + Name);
        }

        /// <summary>
        /// Called when entering this state
        /// </summary>
        public virtual void Enter()
        {
            notifyListenersEnter?.Invoke();
        }

        /// <summary>
        /// Called when leaving this state
        /// </summary>
        public void Exit()
        {
            notifyListenersExit?.Invoke();
        }

    }

    /// <summary>
    /// Default state for a gun where it can be shot
    /// </summary>
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

    /// <summary>
    /// State for a gun that fires a burst shot
    /// </summary>
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
                case StateTrigger.OverHeated:
                    return stateController.overHeated;
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

    /// <summary>
    /// State for a gun that files a single shotS
    /// </summary>
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
                case StateTrigger.OverHeated:
                    return stateController.overHeated;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// State for a gun where it is cooling down and waiting to shoot again
    /// </summary>
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

    /// <summary>
    /// State for a gun where it is out of ammo
    /// </summary>
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

    /// <summary>
    /// State for a gun that files a single shotS
    /// </summary>
    public class OverHeated : State
    {
        public OverHeated(StateController stateController) : base(stateController)
        {
        }

        public override string Name { get => "OverHeated"; }
        public override State HandleTrigger(StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.OverHeatComplete:
                    Exit();
                    return stateController.betweenShots;
                default:
                    return null;
            }
        }
    }
}