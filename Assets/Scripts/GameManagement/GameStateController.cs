using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public delegate void NotifyGSC();

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

    private BikeScript bike;


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
                switch (value)
                {
                    default:
                        currentState = value;
                        break;
                }
            }
    }

    #endregion

    private GameStateController()
    {
        //The game should start in the menu, but for now, we start it in a playing state
        currentState = GameState.Playing;
    }

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

        FindBike();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && ((currentState == GameState.Playing) || (currentState == GameState.Spawning)))
        {
            GameReset();
        }

        else if (bike.Energy <= 0)
        {
            // Lose Condition
            Debug.Log("Player ran out of health");
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
    }

    private void FindBike()
    {
        BikeScript[] bikeScripts = Object.FindObjectsOfType<BikeScript>();
        if (bikeScripts.Length <= 0)
        {
            Debug.LogError("WorldGenerator did not find any BikeScripts in scene");
        }
        else
        {
            bike = bikeScripts[0];
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
