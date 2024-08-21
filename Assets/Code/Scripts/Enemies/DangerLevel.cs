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

    private int minimumThreshold;
    private int maximumThreshold;

    const int DECAY_TICK_MS = 3000;
    const int START_LEVEL = 1;

    public float PercentProgress
    {
        get => Mathf.Clamp01((float)(dangerLevel - minimumThreshold) / (float)(maximumThreshold - minimumThreshold));
    }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dangerLevel = START_LEVEL;
        minimumThreshold = START_LEVEL;
        maximumThreshold = int.MaxValue;
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
        if (dangerLevel < minimumThreshold)
        {
            dangerLevel = minimumThreshold;
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
        Mathf.Clamp(dangerLevel, minimumThreshold, maximumThreshold);
    }

    public void ResetGameObject()
    {
        dangerLevel = START_LEVEL;
        minimumThreshold = START_LEVEL;
        maximumThreshold = int.MaxValue;
        StartTimer();
    }

    public void SetDlThreshold(int newMinimum, int newMaximum = int.MaxValue)
    {
        minimumThreshold = newMinimum;
        maximumThreshold = newMaximum;
    }

}
