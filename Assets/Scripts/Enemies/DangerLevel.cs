using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;


public class DangerLevel : MonoBehaviour, IResettable
{
    public static DangerLevel Instance;

    public Timer dangerTimer;
    public int dangerLevel;

    private int currentDlThreashold;

    const int DECAY_TICK_MS = 3000;
    const int START_LEVEL = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dangerLevel = START_LEVEL;
        currentDlThreashold = START_LEVEL;
        StartTimer();
    }

    private void StartTimer() 
    {
        // This timer increases the danger level and is used for determining the amount and difficulty of enemies being
        // spawned
        dangerTimer = new Timer(DECAY_TICK_MS);
        dangerTimer.AutoReset = true;
        dangerTimer.Enabled = true;
        dangerTimer.Elapsed += XTimer_Elapsed;
    }

    /// <summary>
    /// When xTimer Elapses every {DECAY_TICK_MS} milliseconds, decrease the danger level by 1. 
    /// </summary>
    /// <param name="sender"></param> 
    /// <param name="e"></param>
    private void XTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        dangerLevel--;
        if (dangerLevel < currentDlThreashold)
        {
            dangerLevel = currentDlThreashold;
        }
    }

    /// <summary>
    /// Returns the current danger level 
    /// </summary>
    public int GetDangerLevel()
    {
        return dangerLevel;
    }

    internal void IncreaseDangerLevel(int dlScore)
    {
        dangerLevel += dlScore;
    }

    public void ResetGameObject()
    {
        dangerLevel = START_LEVEL;
        currentDlThreashold = START_LEVEL;
        StartTimer();
    }

    public void SetDlThreashold(int newMinimum)
    {
        currentDlThreashold = newMinimum;
    }

}
