using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Waves;

[CreateAssetMenu(menuName = "Wave/Hostile Wave", fileName = "New Hostile Wave")]
public class HostileWave : Wave
{
    //A collection of enemy types, each loop, a squad of each type is spawned
    [SerializeField]
    public List<WaveEnemyInfo> waveEnemies;

    //The audio loop for this wave
    [SerializeField]
    public AudioClip TrackVariation;

    public override WaveType GetWaveType()
    {
        return WaveType.HostileWave;
    }

    internal override List<WaveEnemyInfo> TriggerWaveAction()
    {
        return waveEnemies;
    }

    public AudioClip GetTrackVariation()
    {
        return TrackVariation;
    }
}
