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
        Initialize();
    }

    private void Initialize()
    {
        jukebox.Init(currentLevel.WaveSequence);
        worldGenerator.CreateGround(currentLevel.GroundMat);
        GameStateController.HandleTrigger(StateTrigger.LoadingComplete);
    }
}
