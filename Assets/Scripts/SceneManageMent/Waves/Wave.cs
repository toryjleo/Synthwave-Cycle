using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave/Wave", fileName = "New Wave")]

public class Wave : ScriptableObject
{
    [SerializeField]
    public int DLThreshold;
    [SerializeField]
    public AudioClip TrackVariation;
    [SerializeField]
    public List<Enemy> waveEnemies;
    public bool IsOverThreshold()
    {
        return DLevel.Instance.GetDangerLevel() > DLThreshold;
    }

    internal AudioClip GetTrackVariation()
    {
        return TrackVariation;
    }

    internal List<Enemy> GetWaveAi()
    {
        return waveEnemies;
    }
}
