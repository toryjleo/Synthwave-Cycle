using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Waves;

[CreateAssetMenu(menuName = "Wave/LevelComplete Wave", fileName = "New LevelComplete Wave")]
public class LevelCompleteWave : Wave
{
    public override WaveType GetWaveType()
    {
        return WaveType.LevelComplete;
    }

    internal override List<WaveEnemyInfo> GetWaveAction()
    {
        GameStateController.HandleTrigger(StateTrigger.LevelComplete);
        return null;
    }
}
