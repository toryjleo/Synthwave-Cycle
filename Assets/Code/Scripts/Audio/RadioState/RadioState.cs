using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RadioState
{

    public enum StateTrigger
    {
        ToggleRadio,
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

        public void Enter()
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
            if (trigger == StateTrigger.ToggleRadio)
            {
                Exit();
                return stateController.radioOn;
            }
            else 
            {
                return null;
            }
        }
    }

    public class RadioOn : State
    {
        public override string Name { get => "RadioOn"; }

        public override State HandleTrigger(RadioStateController stateController, StateTrigger trigger)
        {
            if (trigger == StateTrigger.ToggleRadio)
            {
                Exit();
                return stateController.radioOff;
            }
            else 
            {
                return null;
            }
        }
    }
}