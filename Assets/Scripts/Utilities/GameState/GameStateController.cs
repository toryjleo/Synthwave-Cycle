using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.PackageManager;
using System;

public enum StateTrigger
{
    LoadingComplete,
    StartGame,
    LevelComplete,
    ZeroHP,
    Reset,
    TransitionFromLevel,
}

/// <summary>
/// The GameStateController is a singleton that holds the current state (playing/menu/etc.) of the game
/// so other game objects can know how to act at any time
/// 
/// Requires Prefabs in same scene:
/// 
/// </summary>
public class GameStateController : MonoBehaviour
{

    private static GameStateController instance;

    private static GameState state;

    public static Loading loading;
    public static GameStartPaused gamesStartPaused;
    public static GamePlayPaused gamePlayPaused;
    public static Playing playing;
    public static PlayerDead playerDead;
    public static Resetting resetting;
    public static LevelComplete levelComplete;

    private bool initialEnter = false;



    #region static functions

    public static void HandleTrigger(StateTrigger trigger) 
    {
        //Debug.Log("Received Trigger: " + trigger);
        GameState newState = state.HandleTrigger(trigger);
        if (newState != null) 
        {
            //Debug.Log("Entering State: " + newState);
            state = newState;
            newState.Enter();
        }
    }

    // TODO: Remove when you have a game paused and playing state. Change logic to use events
    public static bool GameIsPlaying()
    {
        if (instance) // Covering case where Gamestatecontroller gets called before it is initialized
        {
            return state == playing;
        }
        else
        {
            return false;
        }
    }



    #endregion

    #region MonoBehavior
    /// <summary>
    /// Object referencing a gamestate should hook up events in OnEnable()
    /// </summary>
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        loading = new Loading();
        gamesStartPaused = new GameStartPaused();
        gamePlayPaused = new GamePlayPaused();
        playing = new Playing();
        playerDead = new PlayerDead();
        resetting = new Resetting();
        levelComplete = new LevelComplete();

        state = loading;
        initialEnter = false;

    }

    private void Update()
    {
        if (!initialEnter) 
        {
            initialEnter = true;
            state.Enter();
        }

        // TODO: Call trigger instead
        if (Input.GetKeyDown(KeyCode.R))
        {
            HandleTrigger(StateTrigger.Reset);
            GameReset();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            HandleTrigger(StateTrigger.LoadingComplete);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandleTrigger(StateTrigger.StartGame);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HandleTrigger(StateTrigger.LevelComplete);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HandleTrigger(StateTrigger.ZeroHP);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            HandleTrigger(StateTrigger.Reset);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            HandleTrigger(StateTrigger.TransitionFromLevel);
        }
#endif
    }
#endregion
    // TODO: Move elsewhere
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
        GameStateController.HandleTrigger(StateTrigger.LoadingComplete);

    }


}