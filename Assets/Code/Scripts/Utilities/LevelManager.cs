using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorObject;
using UnityEngine;

/// <summary>
/// Controls level gameplay objects and initializes/resets gameplay controlled features (Jukebox, WorldGenerator, etc.)
/// </summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField] public Jukebox jukebox;
    [SerializeField] public WorldGenerator worldGenerator;

    [SerializeField] public GameLevel currentLevel;
    [SerializeField] private GameSave gameSave;

    List<IResettable> resetObjects;

    public string SongName
    {
        get => currentLevel.WaveSequence.SongName;
    }

    public Sprite RadioFace
    {
        get => currentLevel.WaveSequence.RadioFace;
    }

    void Awake()
    {
        LevelSelector levelSelector = FindObjectOfType<LevelSelector>();
        if (!levelSelector)
        {
            Debug.LogWarning("No level selector found!");
            return;
        }
        else
        {
            currentLevel = levelSelector.SelectedLevel;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        jukebox.Init(currentLevel.WaveSequence);
        worldGenerator.CreateGround(currentLevel.GroundMat);
        ImpactManager.Instance.Init();
        ImpactManager.Instance.AddGroundMapping(currentLevel.GroundImpactMapping);
        resetObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IResettable>().ToList();

        Arsenal arsenal = FindObjectOfType<Arsenal>();
        arsenal.Init(gameSave);

        GameStateController.resetting.notifyListenersEnter += GameReset;
        GameStateController.levelComplete.notifyListenersEnter += GameComplete;

        Initialize();
    }

    private void Initialize()
    {
        GameStateController.HandleTrigger(StateTrigger.LoadingComplete);
    }


    /// <summary>
    /// Calls ResetGameObject() on every IResettable object in the game world
    /// </summary>
    private void GameReset()
    {
        Debug.Log("Resetting!");
        resetObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IResettable>().ToList();
        DangerLevel.Instance.dangerLevel = 0;//make sure we don't start at the same level
        ImpactManager.Instance.Reset();
        foreach (IResettable r in resetObjects)
        {
            r.ResetGameObject();
        }
        currentLevel.WaveSequence.ResetGameObject();
        Initialize();
    }

    /// <summary>
    /// Updates the level progress for the game save
    /// </summary>
    private void GameComplete()
    {
        // If the current level you're on is the last level: go back to the first level (for 'Continue' purposes)
        if (currentLevel.LevelNumber + 1 > gameSave.levelSequence.Length - 1)
        {
            gameSave.CurrentLevel = 0;
        }
        // Else, your next level to continue with will be the next level
        else gameSave.CurrentLevel += currentLevel.LevelNumber + 1;

        // If you beat the max level you have unlocked, increase the max level to the next one
        // UNLESS: it ends up being greater than the number of levels that exist, then just keep it at max progress
        if (gameSave.MaxLevelProgess == currentLevel.LevelNumber && gameSave.MaxLevelProgess < gameSave.levelSequence.Length - 1)
        {
            gameSave.MaxLevelProgess += 1;
        }
    }
}
