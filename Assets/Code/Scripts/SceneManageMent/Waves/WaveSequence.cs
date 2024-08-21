using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Waves;

namespace EditorObject
{

    /// <summary>
    /// Holds a set of Waves and handles moving from one to the next, as well as providing
    /// the Jukebox with the current TrackVariation audio clip
    /// </summary>
    [CreateAssetMenu(menuName = "Wave/WaveSequence", fileName = "New Wave Sequence")]
    public class WaveSequence : ScriptableObject, IResettable
    {
        private int previousWave = -1;
        private int currentWave = 0;
        [SerializeField]
        public List<Wave> sequence;
        internal SquadSpawner spawner;
        [SerializeField]
        public string songName;

        public AudioClip GetCurrentTrackVariation()
        {
            if (sequence[currentWave].GetWaveType() != WaveType.AudioWave)
            {
                Debug.Log("No radio clip on wave " + currentWave);
            }

            if (sequence[currentWave].GetWaveType() != WaveType.LevelComplete)
            {
                HostileWave wave = (HostileWave)sequence[currentWave];
                return wave.GetTrackVariation();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Updates the wave and spawns in wave according to danger level
        /// </summary>
        public void SpawnNewWave()
        {
            UpdateCurrentWave();
            spawner.SpawnWave(sequence[currentWave].TriggerWaveAction());
            previousWave = currentWave;
        }

        /// <summary>
        /// Sets the currentWave number based on Danger Level
        /// </summary>
        internal void UpdateCurrentWave()
        {
            // Iterate backwards and spawn in the waves for the highest danger level
            if (sequence[currentWave].IsOverThreshold())
            {
                currentWave += 1;
                int nextWave = currentWave + 1;
                int nextThreshold = (currentWave == sequence.Count - 1) ? int.MaxValue : sequence[nextWave].DLThreshold;
                DangerLevel.Instance.SetDlThreshold(sequence[currentWave].DLThreshold, nextThreshold);
                //Log the wave + 1 because the index starts at 0, but the tracks start at 1
                Debug.Log("Current Wave: " + nextWave + "/" + sequence.Count + "\nDanger Level: " + DangerLevel.Instance.GetDangerLevel());
                Debug.Log("Danger Level Threshold: " + nextThreshold);
            }
        }

        internal void Init(SquadSpawner squadSpawner)
        {
            spawner = squadSpawner;
            currentWave = 0;
            previousWave = -1;
        }
        /// <summary>
        /// Gets the universal time that the next wave will spawn at,
        /// according to the waves per loop and loop duration
        /// </summary>
        internal double GetNextWaveTime(double currentTime)
        {
            if (sequence[currentWave].GetWaveType() != WaveType.LevelComplete)
            {
                HostileWave wave = (HostileWave)sequence[currentWave];
                AudioClip clipToPlay = wave.GetTrackVariation();
                double duration = (double)clipToPlay.samples / clipToPlay.frequency;
                return currentTime + duration;
            }
            else
            {
                return 0;
            }
        }

        public void ResetGameObject()
        {
            currentWave = 0;
            previousWave = -1;
        }
    }
}