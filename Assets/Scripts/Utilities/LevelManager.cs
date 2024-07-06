using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour, IResettable
{
    [SerializeField]
    public Jukebox jukebox;
    [SerializeField]
    public WorldGenerator worldGenerator;

    [SerializeField]
    public GameLevel currentLevel;

    public void ResetGameObject()
    {
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameStateController.playing.notifyListenersEnter += StartTimeScale;
        GameStateController.playing.notifyListenersExit += StopTimeScale;

        jukebox.Init(currentLevel.WaveSequence);
        worldGenerator.CreateGround(currentLevel.GroundMat);

                                                             
        Initialize();
    }

    private void StartTimeScale() 
    {
        Time.timeScale = 1;
    }

    private void StopTimeScale()
    {
        Time.timeScale = 0;
    }

    private void Initialize()
    {
        GameStateController.HandleTrigger(StateTrigger.LoadingComplete);
    }
}
