using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A wave holds the information for one "Wave Level" of enemies
/// When the danger level reches the DLThreshold,
/// squads of waveEnemies are spawned in every track loop
/// </summary>
[CreateAssetMenu(menuName = "Wave/Wave", fileName = "New Wave")]
public class Wave : ScriptableObject
{
    //The minimum danger level required for this Wave to take effect
    [SerializeField]
    public int DLThreshold;
    
    //The audio loop for this wave
    [SerializeField]
    public AudioClip TrackVariation;

    //A collection of enemy types, each loop, a squad of each type is spawned
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
