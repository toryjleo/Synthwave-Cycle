using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Delegate to 
public delegate void GameStateHandler(GameState previousState, GameState newState);

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

    private static GameState currentState;

    public static event GameStateHandler notifyListenersGameStateHasChanged;


    #region static functions


    public static bool GameIsPlaying()
    {
        if (Instance) // Covering case where Gamestatecontroller gets called before it is initialized
        {
            return currentState == GameState.Playing;
        }
        else
        {
            return false;
        }
    }

    public static bool PlayerIsSpawning()
    {
        if (Instance)
        {
            return currentState == GameState.Spawning;
        }
        else 
        {
            return false;
        }
    }

    public static GameState GetGameState()
    {
        return currentState;
    }

    public static GameState WorldState
    {
        get { return currentState; }
        set {
            if (value == currentState)
            {
                Debug.Log("Trying to set state to '" + value + "' but world is already in that state.");
            }
            else
            {
                // Holding previous state to notify listeners of switch.
                GameState previousState = currentState;

                switch (value)
                {
                    case GameState.Spawning:
                        if (currentState == GameState.Playing) 
                            { // TODO: Make sure that all UI is updated to respawn
                            }
                        currentState = value;
                        break;
                    case GameState.Playing:
                        currentState = value;
                        break;
                    default:
                        currentState = value;
                        break;
                }

                notifyListenersGameStateHasChanged?.Invoke(previousState, currentState);
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
        currentState = GameState.Playing;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && ((currentState == GameState.Playing) || (currentState == GameState.Spawning)))
        {
            GameReset();
        }
    }
    #endregion

    /// <summary>
    /// Calls ResetGameObject() on every IResettable object in the game world
    /// </summary>
    private void GameReset()
    {
        Debug.Log("Resetting!");
        List<IResettable> resetObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IResettable>().ToList();
        foreach (IResettable r in resetObjects)
        {
            r.ResetGameObject();
        }
        WorldState = GameState.Playing;
    }


}

public enum GameState
{
    Uninitialized,
    Menu,
    Spawning,
    Playing,
    Resetting
}
