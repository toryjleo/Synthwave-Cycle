using System.Runtime.InteropServices.WindowsRuntime;

public delegate void StateChangeHandler();


public abstract class GameState
{
  public event StateChangeHandler notifyListenersEnter;
  public event StateChangeHandler notifyListenersExit;

  public abstract GameState HandleTrigger(StateTrigger trigger);

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
  public override GameState HandleTrigger(StateTrigger trigger)
  {
    if (trigger == StateTrigger.LoadingComplete)
    {
      Exit();
      return GameStateController.gamesStartPaused;
    }
    return null;
  }
}

public class GameStartPaused : GameState
{
  public override GameState HandleTrigger(StateTrigger trigger)
  {
    if (trigger == StateTrigger.StartGame)
    {
      Exit();
      return GameStateController.playing;
    }
    return null;
  }
}

public class GamePlayPaused : GameState
{
  public override GameState HandleTrigger(StateTrigger trigger)
  {
    if (trigger == StateTrigger.StartGame)
    {
      Exit();
      return GameStateController.playing;
    }
    if (trigger == StateTrigger.Reset)
    {
      Exit();
      return GameStateController.resetting;
    }
    return null;
  }
}

public class Playing : GameState
{
  public override GameState HandleTrigger(StateTrigger trigger)
  {
    if (trigger == StateTrigger.StartGame)
    {
      Exit();
      return GameStateController.gamePlayPaused;
    }
    else if (trigger == StateTrigger.ZeroHP)
    {
      Exit();
      return GameStateController.playerDead;
    }
    else if (trigger == StateTrigger.LevelComplete)
    {
      Exit();
      return GameStateController.levelComplete;
    }
    return null;
  }
}

public class PlayerDead : GameState
{
  public override GameState HandleTrigger(StateTrigger trigger)
  {
    if (trigger == StateTrigger.Reset)
    {
      Exit();
      return GameStateController.resetting;
    }
    return null;
  }
}

public class Resetting : GameState
{
  public override GameState HandleTrigger(StateTrigger trigger)
  {
    if (trigger == StateTrigger.LoadingComplete)
    {
      Exit();
      return GameStateController.gamesStartPaused;
    }
    return null;
  }
}

public class LevelComplete : GameState
{
  public override GameState HandleTrigger(StateTrigger trigger)
  {
    if (trigger == StateTrigger.Reset)
    {
      Exit();
      return GameStateController.resetting;
    }
    else if (trigger == StateTrigger.TransitionFromLevel)
    {
      Exit();
      return GameStateController.loading;
    }
    return null;
  }
}
