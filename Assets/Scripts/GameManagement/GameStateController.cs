using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    private static GameStateController instance;

    private GameState currentState;


    #region static functions

    public static GameStateController Instance()
    {
        if(instance == null)
        {
            instance = new GameStateController();
        }
        return instance;
    }

    public static bool IsGamePlaying()
    {
        return Instance().currentState == GameState.Playing;
    }

    public static bool IsPlayerSpawning()
    {
        return Instance().currentState == GameState.Spawning;
    }

    public static GameState GetGameState()
    {
        return Instance().currentState; 
    }

    #endregion

    private GameStateController()
    {
        currentState = GameState.Menu;
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
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
