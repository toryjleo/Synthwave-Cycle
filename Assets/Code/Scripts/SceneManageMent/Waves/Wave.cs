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
        internal abstract List<WaveEnemyInfo> TriggerWaveAction();
    }
}