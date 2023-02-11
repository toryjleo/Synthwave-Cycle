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


    public static bool IsGamePlaying()
    {
        return Instance.currentState == GameState.Playing;
    }

    public static bool IsPlayerSpawning()
    {
        return Instance.currentState == GameState.Spawning;
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
        //currentState = GameState.Menu;
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
    Menu,
    Spawning,
    Playing,
    Resetting
}
