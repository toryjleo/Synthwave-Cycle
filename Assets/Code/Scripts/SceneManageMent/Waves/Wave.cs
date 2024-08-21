using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Waves
{
    public enum WaveType
    {
        HostileWave, LevelComplete, AudioWave
    };


    /// <summary>
    /// Serializable Tuple holding information for how each individual squad will be spawned
    /// Has an enemy type and a position relative to the player for where the squad will spawn in
    /// </summary>
    [Serializable]
    public class WaveEnemyInfo
    {
        public Enemy enemyType;
        public SpawnLocation spawnLocation;
    }


    /// <summary>
    /// A wave holds the information for one "Wave Level" of enemies
    /// When the danger level reaches the DLThreshold,
    /// squads of waveEnemies are spawned in every track loop
    /// </summary>
    [CreateAssetMenu(menuName = "Wave/Wave", fileName = "New Wave")]
    public abstract class Wave : ScriptableObject
    {
        //The minimum danger level required for this Wave to take effect
        [SerializeField]
        public int DLThreshold;

        public abstract WaveType GetWaveType();

        //Determines if current DL is above this wave's threshold
        public bool IsOverThreshold()
        {

            return DangerLevel.Instance.GetDangerLevel() >= DLThreshold;
        }

        // Activates wave properties and returns a list of wave enemies
        internal abstract List<WaveEnemyInfo> GetWaveAction();
    }

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

        internal override List<WaveEnemyInfo> GetWaveAction()
        {
            return waveEnemies;
        }

        public AudioClip GetTrackVariation()
        {
            return TrackVariation;
        }
    }

    [CreateAssetMenu(menuName = "Wave/Audio Wave", fileName = "New Audio Wave")]
    public class AudioWave : HostileWave
    {
        //The transmission audio clip for this wave
        [SerializeField]
        public AudioClip RadioClip;

        public override WaveType GetWaveType()
        {
            return WaveType.AudioWave;
        }
    }

    [CreateAssetMenu(menuName = "Wave/LevelComplete Wave", fileName = "New LevelComplete Wave")]
    public class LevelComplete : Wave
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
}