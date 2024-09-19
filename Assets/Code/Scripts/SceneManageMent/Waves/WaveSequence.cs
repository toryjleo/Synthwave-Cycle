using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        [SerializeField] private List<Wave> sequence;
        internal SquadSpawner spawner;
        [SerializeField] private string songName;
        [SerializeField] private Sprite radioFace;
        [SerializeField] private bool debugWaveInfo = false;

        public Sprite RadioFace { get => radioFace; }

        public string SongName { get => songName; }

        private bool hasAlreadyPlayedRadioClip = false;


        public bool CurrentTrackRadioWaveHasAlreadyPlayed
        {
            get { return hasAlreadyPlayedRadioClip; }
        }
        public bool CurrentTrackIsAudioWave
        {
            get => WaveIsAudioWave(currentWave);
        }

        public bool CurrentWaveIsFinal
        {
            get => sequence[currentWave].GetWaveType() == WaveType.LevelComplete;
        }

        public AudioClip GetCurrentRadioClip
        {
            get
            {
                if (sequence[currentWave].GetWaveType() != WaveType.AudioWave)
                {
                    Debug.LogError("Tried to get a radioclip where there is none");
                    return null;
                }
                else
                {

                    hasAlreadyPlayedRadioClip = true;
                    return ((AudioWave)sequence[currentWave]).GetRadioClip;
                }
            }
        }

        /// <summary>
        /// Gets the audio to be played as music during a wave. An audio wave does not have
        /// music while a level complete wave and hostile wave return their music tracks.
        /// </summary>
        /// <returns></returns>
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
                LevelCompleteWave wave = (LevelCompleteWave)sequence[currentWave];
                return wave.GetTrackVariation();
            }
        }

        /// <summary>
        /// Updates the wave and spawns in wave according to danger level,
        /// Used to spawn in the next sequential wave
        /// </summary>
        public void SpawnNewWave()
        {
            spawner.SpawnWave(sequence[currentWave].TriggerWaveAction());
        }

        /// <summary>
        /// Sets the currentWave number based on Danger Level
        /// </summary>
        internal void UpdateCurrentWave()
        {
            if (debugWaveInfo) Debug.Log("CURRENT WAVE before update:" + currentWave);
            // Iterate backwards and spawn in the waves for the highest danger level
            if (sequence[currentWave].IsOverThreshold())
            {
                hasAlreadyPlayedRadioClip = false;
                previousWave = currentWave;
                currentWave += 1;
                int nextWave = currentWave + 1;
                int nextThreshold = sequence[currentWave].DLThreshold;
                DangerLevel.Instance.SetDlThreshold(sequence[previousWave].DLThreshold, nextThreshold);
                //Log the wave + 1 because the index starts at 0, but the tracks start at 1
                Debug.Log("Current Wave: " + nextWave + "/" + sequence.Count + "\nDanger Level: " + DangerLevel.Instance.GetDangerLevel());
                Debug.Log("Danger Level Threshold: " + nextThreshold);
            }
            if (debugWaveInfo) Debug.Log("CURRENT WAVE after update:" + currentWave);
        }

        internal void Init(SquadSpawner squadSpawner)
        {
            hasAlreadyPlayedRadioClip = false;
            spawner = squadSpawner;
            currentWave = 0;
            previousWave = -1;

            //Set the initial DlThreshold for the first wave (used in gameplay UI)
            int nextThreshold = sequence[0].DLThreshold;
            DangerLevel.Instance.SetDlThreshold(0, nextThreshold);
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

        /// <summary>
        /// Verifies if a wave is a radio log playing wave
        /// </summary>
        /// <param name="index">Wave index in the wave sequence</param>
        /// <returns>Boolean whether indexed wave is of type AudioWave</returns>
        private bool WaveIsAudioWave(int index)
        {
            return sequence[index].GetWaveType() == WaveType.AudioWave;
        }

        public void ResetGameObject()
        {
            currentWave = 0;
            previousWave = -1;
            hasAlreadyPlayedRadioClip = false;

            //Set the initial DlThreshold for the first wave (used in gameplay UI)
            int nextThreshold = sequence[0].DLThreshold;
            DangerLevel.Instance.SetDlThreshold(0, nextThreshold);
        }
    }
}