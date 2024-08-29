using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Waves;

/// <summary>
/// A wave type that derives from the base Wave class that includes a wave action to
/// trigger the LevelComplete game state.
/// </summary>
[CreateAssetMenu(menuName = "Wave/LevelComplete Wave", fileName = "New LevelComplete Wave")]
public class LevelCompleteWave : Wave
{
    [SerializeField] public AudioClip finalMusicLoop;

    public override WaveType GetWaveType()
    {
        return WaveType.LevelComplete;
    }

    internal override List<WaveEnemyInfo> TriggerWaveAction()
    {
        GameStateController.HandleTrigger(StateTrigger.LevelComplete);
        return null;
    }

    public AudioClip GetTrackVariation()
    {
        return finalMusicLoop;
    }
}
