using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    /// <summary>
    /// The GameStateController is a singleton that holds the current state (playing/menu/etc.) of the game
    /// so other game objects can know how to act at any time
    /// </summary>
    private static GameStateController Instance;

    private GameState currentState;


    #region static functions


    public static bool GameIsPlaying()
    {
        if (Instance) // Covering case where Gamestatecontroller gets called before it is initialized
        {
            return Instance.currentState == GameState.Playing;
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
            return Instance.currentState == GameState.Spawning;
        }
        else 
        {
            return false;
        }
    }

    public static GameState GetGameState()
    {
        return Instance.currentState; 
    }

    #endregion

    private GameStateController()
    {
        //The game should start in the menu, but for now, we start it in a playing state
        currentState = GameState.Playing;
    }

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
