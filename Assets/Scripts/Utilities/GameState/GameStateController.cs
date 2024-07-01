using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Delegate method template to handle switching from a previous state to a new state.
public delegate void GameStateHandler(GameStateEnum previousState, GameStateEnum newState);

public enum StateTrigger
{
    LoadingComplete,
    StartGame,
    LevelComplete,
    ZeroHP,
    Reset,
    TransitionFromLevel,
}

public enum GameStateEnum
{
    Uninitialized,
    Menu,
    Spawning,
    Playing,
    Resetting
}


public class GameStateController : MonoBehaviour
{
    /// <summary>
    /// The GameStateController is a singleton that holds the current state (playing/menu/etc.) of the game
    /// so other game objects can know how to act at any time
    /// 
    /// Requires Prefabs in same scene:
    /// 
    /// </summary>
    private static GameStateController Instance;

    private static GameStateEnum state;

    public static event GameStateHandler notifyListenersGameStateHasChanged;

    public static GameStateEnum State()
    {
        return state;
    }


    #region static functions

    public static void HandleTrigger(StateTrigger trigger) 
    {
    
    }

    // TODO: Remove when you have a game paused and playing state
    public static bool GameIsPlaying()
    {
        if (Instance) // Covering case where Gamestatecontroller gets called before it is initialized
        {
            return state == GameStateEnum.Playing;
        }
        else
        {
            return false;
        }
    }

    // TODO: Update code to call objects instead of having listeners
    /// <summary>
    /// A Proprty which handles the changing of game states and notifies listeners.
    /// </summary>
    public static GameStateEnum WorldState
    {
        get { return state; }
        set {
            if (value == state)
            {
                Debug.Log("Trying to set state to '" + value + "' but world is already in that state.");
            }
            else
            {
                // Holding previous state to notify listeners of switch.
                GameStateEnum previousState = state;

                switch (value)
                {
                    case GameStateEnum.Spawning:
                        state = value;
                        break;
                    case GameStateEnum.Playing:
                        state = value;
                        break;
                    default:
                        state = value;
                        break;
                }

                notifyListenersGameStateHasChanged?.Invoke(previousState, state);
            }
            }
    }

    #endregion

    #region MonoBehavior
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        //The game should start in the menu, but for now, we start it in a playing state
        state = GameStateEnum.Playing;
    }

    private void Update()
    {
        // TODO: Call trigger instead
        if (Input.GetKeyDown(KeyCode.R) && ((state == GameStateEnum.Playing) || (state == GameStateEnum.Spawning)))
        {
            GameReset();
        }
    }
    #endregion

    // 
    /// <summary>
    /// Calls ResetGameObject() on every IResettable object in the game world
    /// </summary>
    private void GameReset()
    {
        // TODO: Move logic to level loader
        Debug.Log("Resetting!");
        List<IResettable> resetObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IResettable>().ToList();
        DLevel.Instance.dangerLevel = 0;//make sure we don't start at the same level
        foreach (IResettable r in resetObjects)
        {
            r.ResetGameObject();
        }
        WorldState = GameStateEnum.Playing;
    }


}
