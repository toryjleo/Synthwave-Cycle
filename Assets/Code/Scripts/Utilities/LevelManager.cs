using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    public Jukebox jukebox;
    [SerializeField]
    public WorldGenerator worldGenerator;

    [SerializeField]
    public GameLevel currentLevel;

    List<IResettable> resetObjects;

    public string SongName
    {
        get => currentLevel.WaveSequence.songName;
    }

    public Sprite RadioFace
    {
        get => currentLevel.WaveSequence.radioFace;
    }

    // Start is called before the first frame update
    void Start()
    {
        jukebox.Init(currentLevel.WaveSequence);
        worldGenerator.CreateGround(currentLevel.GroundMat);
        resetObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IResettable>().ToList();

        GameStateController.resetting.notifyListenersEnter += GameReset;


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
        foreach (IResettable r in resetObjects)
        {
            r.ResetGameObject();
        }
        currentLevel.WaveSequence.ResetGameObject();
        Initialize();
    }
}
