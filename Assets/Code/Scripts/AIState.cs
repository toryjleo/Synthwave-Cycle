using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIState 
{
    public enum StateTrigger 
    {
        Spawning,
        AiKilled,
        ArrivedAtLocation,
        HasTarget,
        InRange,
        OutOfRange,
        CountownToAttackComplete,
        FollowAgain,
        Despawned,
        PlayerDead,
    }

    /// <summary>
    /// Manages states for a given AI
    /// </summary>
    public class StateController 
    {
        #region States
        public InPool inPool = null;
        public Wandering wandering = null;
        public Following following = null;
        public InRange inRange = null;
        public Attacking attacking = null;
        public Dead dead = null;
        #endregion


        private State state;

        public bool printState = false;

        public StateController(bool printState = false)
        {
            inPool = new InPool(this);
            wandering = new Wandering(this);
            following = new Following(this);
            inRange = new InRange(this);
            attacking = new Attacking(this);
            dead = new Dead(this);

            Reset();

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
            state = inPool;
            if (printState)
            {
                state.PrintStateEnter();
            }
        }
    }

    /// <summary>
    /// Abstract AI state declaration
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
            Debug.Log("AIState >  " + Name);
        }

        /// <summary>
        /// Called when entering this state
        /// </summary>
        public virtual void Enter()
        {
            PrintStateEnter();
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

    #region State Implementation
    /// <summary>
    /// Initial state. When the enemy gets pooled or returns to pool.
    /// </summary>
    public class InPool : State
    {
        public override string Name { get => "InPool"; }

        public InPool(StateController stateController) : base(stateController)
        {
        }

        public override State HandleTrigger(StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.Spawning:
                    Exit();
                    return stateController.wandering;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// State for wandering behavior
    /// </summary>
    public class Wandering : State
    {
        public override string Name { get => "Wandering"; }

        public Wandering(StateController stateController) : base(stateController)
        {
        }

        public override State HandleTrigger(StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.AiKilled:
                    Exit();
                    return stateController.dead;
                case StateTrigger.ArrivedAtLocation:
                    Exit();
                    return stateController.wandering;
                case StateTrigger.HasTarget:
                    Exit();
                    return stateController.following;

                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// State for following behavior
    /// </summary>
    public class Following : State
    {
        public override string Name { get => "Following"; }

        public Following(StateController stateController) : base(stateController)
        {
        }

        public override State HandleTrigger(StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.AiKilled:
                    Exit();
                    return stateController.dead;
                case StateTrigger.PlayerDead:
                    Exit();
                    return stateController.wandering;
                case StateTrigger.InRange:
                    Exit();
                    return stateController.inRange;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// State for when ai is in range of target
    /// </summary>
    public class InRange : State
    {
        public override string Name { get => "InRange"; }

        public InRange(StateController stateController) : base(stateController)
        {
        }

        public override State HandleTrigger(StateTrigger trigger)
        {
            switch (trigger)
            {

                case StateTrigger.AiKilled:
                    Exit();
                    return stateController.dead;
                case StateTrigger.PlayerDead:
                    Exit();
                    return stateController.wandering;
                case StateTrigger.OutOfRange:
                    Exit();
                    return stateController.following;
                case StateTrigger.CountownToAttackComplete:
                    Exit();
                    return stateController.wandering;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// State for when ai is attacking
    /// </summary>
    public class Attacking : State
    {
        public override string Name { get => "Attacking"; }

        public Attacking(StateController stateController) : base(stateController)
        {
        }

        public override State HandleTrigger(StateTrigger trigger)
        {
            switch (trigger)
            {

                case StateTrigger.AiKilled:
                    Exit();
                    return stateController.dead;
                case StateTrigger.PlayerDead:
                    Exit();
                    return stateController.wandering;
                case StateTrigger.OutOfRange:
                    Exit();
                    return stateController.following;
                case StateTrigger.FollowAgain:
                    Exit();
                    return stateController.following;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// State for when an ai is dead
    /// </summary>
    public class Dead : State
    {
        public override string Name { get => "Dead"; }

        public Dead(StateController stateController) : base(stateController)
        {
        }

        public override State HandleTrigger(StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.Despawned:
                    Exit();
                    return stateController.inPool;
                default:
                    return null;
            }
        }
    }
    #endregion
}
