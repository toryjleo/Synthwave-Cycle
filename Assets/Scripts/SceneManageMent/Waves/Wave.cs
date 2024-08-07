using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum WaveType
{
    HostileWave, LevelComplete, PlayAudioLog
};

namespace EditorObject
{

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

        //If the track has more than one music loop, we need to spawn in waves multiple times
        [SerializeField]
        public int wavesInTrack = 1;

        //A collection of enemy types, each loop, a squad of each type is spawned
        [SerializeField]
        public List<Enemy> waveEnemies;

        //Default: HostileWave, this determines if the wave has any special functionality
        [SerializeField]
        public WaveType waveType;

        public bool IsOverThreshold()
        {
            return DangerLevel.Instance.GetDangerLevel() >= DLThreshold;
        }

        internal AudioClip GetTrackVariation()
        {
            return TrackVariation;
        }

        // Activates wave properties and returns a list of wave enemies
        internal virtual List<Enemy> GetWaveInfo()
        {
            switch (waveType)
            {
                case WaveType.HostileWave:
                    break;
                case WaveType.LevelComplete:
                    GameStateController.HandleTrigger(StateTrigger.LevelComplete);
                    break;
            }
            return waveEnemies;
        }
    }
}