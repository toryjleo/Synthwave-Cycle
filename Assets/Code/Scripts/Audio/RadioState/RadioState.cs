using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RadioState
{

    public enum StateTrigger
    {
        ToggleRadio,
        InBounds,
        OutOfBounds,
    }

    
    public abstract class State
    {
        public event StateChangeHandler notifyListenersEnter;
        public event StateChangeHandler notifyListenersExit;

        public virtual string Name { get;}

        public abstract State HandleTrigger(RadioStateController stateController, StateTrigger trigger);

        public void PrintStateEnter()
        {
            Debug.Log("RadioState >  " + Name);
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

    public class RadioOff : State
    {
        public override string Name { get => "RadioOff";}

        public override State HandleTrigger(RadioStateController stateController, StateTrigger trigger)
        {
            switch (trigger) 
            {
                case StateTrigger.ToggleRadio:
                    Exit();
                    return stateController.radioOn;
                default:
                    return null;
            }
        }
    }

    public class RadioOn : State
    {
        // TODO: get boundschecker to listen to this state's notifyListenersEnter. When that triggers, call boundschecker's event
        public override string Name { get => "RadioOn"; }

        public override State HandleTrigger(RadioStateController stateController, StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.ToggleRadio:
                    Exit();
                    return stateController.radioOff;
                case StateTrigger.OutOfBounds:
                    Exit();
                    return stateController.outOfBounds;
                case StateTrigger.InBounds:
                    Exit();
                    return stateController.inBounds;
                default:
                    return null;
            }
        }
    }

    public class InBounds : State 
    {
        public override string Name { get => "InBounds"; }

        public override State HandleTrigger(RadioStateController stateController, StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.ToggleRadio:
                    Exit();
                    return stateController.radioOff;
                case StateTrigger.OutOfBounds:
                    Exit();
                    return stateController.outOfBounds;
                default:
                    return null;
            }
        }

    }

    public class OutOfBounds : State
    {
        public override string Name { get => "OutOfBounds"; }

        public override State HandleTrigger(RadioStateController stateController, StateTrigger trigger)
        {
            switch (trigger)
            {
                case StateTrigger.ToggleRadio:
                    Exit();
                    return stateController.radioOff;
                case StateTrigger.InBounds:
                    Exit();
                    return stateController.inBounds;
                default:
                    return null;
            }
        }

    }
}