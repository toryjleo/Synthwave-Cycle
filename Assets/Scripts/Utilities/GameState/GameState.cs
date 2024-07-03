public delegate void StateChangeHandler();


public abstract class GameState
{
    public event StateChangeHandler notifyListenersEnter;
    public event StateChangeHandler notifyListenersExit;

    public abstract GameState HandleTrigger(StateTrigger trigger, GameStateController c);

    public void Enter()
    {
        notifyListenersEnter?.Invoke();
    }

    public void Exit()
    {
        notifyListenersExit?.Invoke();
    }
}

public class Loading : GameState
{
    public override GameState HandleTrigger(StateTrigger trigger, GameStateController c)
    {
        if (trigger == StateTrigger.LoadingComplete) 
        {
            // TODO: Return GameStartPaused
        }
        return null;
    }
}

public class GameStartPaused : GameState
{
    public override GameState HandleTrigger(StateTrigger trigger, GameStateController c)
    {
        if (trigger == StateTrigger.StartGame) 
        {
            // TODO: Return Playing
        }
        return null;
    }
}

public class GamePlayPaused : GameState
{
    public override GameState HandleTrigger(StateTrigger trigger, GameStateController c)
    {
        if (trigger == StateTrigger.StartGame) 
        {
            // TODO: Return Playing
        }
        return null;
    }
}

public class Playing : GameState
{
    public override GameState HandleTrigger(StateTrigger trigger, GameStateController c)
    {
        if (trigger == StateTrigger.StartGame) 
        {
            // TODO: return GamePlayPaused
            //
        }
        else if (trigger == StateTrigger.ZeroHP) 
        {
            // TODO: Return playerdead
        }
        else if (trigger == StateTrigger.LevelComplete) 
        {
            // TODO: return level complete
        }
        return null;
    }
}

public class PlayerDead : GameState
{
    public override GameState HandleTrigger(StateTrigger trigger, GameStateController c)
    {
        if (trigger == StateTrigger.Reset) 
        {
            // Return resetting
        }
        return null;
    }
}

public class Resetting : GameState
{
    public override GameState HandleTrigger(StateTrigger trigger, GameStateController c)
    {
        if (trigger == StateTrigger.LoadingComplete) 
        {
            // return gamestartpaused
        }
        return null;
    }
}

public class LevelComplete : GameState
{
    public override GameState HandleTrigger(StateTrigger trigger, GameStateController c)
    {
        if(trigger == StateTrigger.Reset) 
        {
            // return resetting
        }
        else if (trigger == StateTrigger.TransitionFromLevel) 
        {
            // return loading
        }
        return null;
    }
}
