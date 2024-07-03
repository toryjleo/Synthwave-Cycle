using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.PackageManager;

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



    #region static functions

    public static void HandleTrigger(StateTrigger trigger) 
    {
    
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

        //The game should start in the menu, but for now, we start it in a playing state
        state = loading;
        state.Enter();
    }

    private void Update()
    {
        // TODO: Call trigger instead
        if (Input.GetKeyDown(KeyCode.R))
        {
            HandleTrigger(StateTrigger.Reset);
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
        GameStateController.HandleTrigger(StateTrigger.LoadingComplete);

    }


}
