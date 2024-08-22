using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Waves;

[CreateAssetMenu(menuName = "Wave/Audio Wave", fileName = "New Audio Wave")]
public class AudioWave : HostileWave
{
    //The transmission audio clip for this wave
    [SerializeField]
    public AudioClip RadioClip;

    public AudioClip GetRadioClip
    {
        get
        {
            return RadioClip;
        }
    }

    public override WaveType GetWaveType()
    {
        return WaveType.AudioWave;
    }
}
