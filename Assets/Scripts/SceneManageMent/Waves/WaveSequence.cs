using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a set of Waves and handles moving from one to the next, as well as providing
/// the Jukebox with the current TrackVariation audio clip
/// </summary>
[CreateAssetMenu(menuName = "Wave/WaveSequence", fileName = "New Wave Sequence")]
public class WaveSequence : ScriptableObject
{
    private int previousWave = -1;
    private int CurrentWave = 0;
    [SerializeField]
    public List<Wave> sequence;
    internal SquadSpawner spawner;

    public AudioClip GetCurrentTrackVariation()
    {
        return sequence[CurrentWave].GetTrackVariation();
    }

    public void CheckForWaveSpawn()
    {
        UpdateCurrentWave();
        spawner.SpawnWave(sequence[CurrentWave].GetWaveInfo());
        previousWave = CurrentWave;
    }

    internal void UpdateCurrentWave()
    {
        for (int index = 0; index < sequence.Count; index++)
        {
            if (!sequence[index].IsOverThreshold())
            {
                CurrentWave = Mathf.Clamp(index - 1, 0, sequence.Count - 1);
                //Log the wave + 1 because the index starts at 0, but the tracks start at 1
                Debug.Log("Current Wave: " + (CurrentWave + 1) + "/" + (sequence.Count) + "\nDanger Level: " + DLevel.Instance.GetDangerLevel());
                break;
            }
        }
    }

    internal void Init(SquadSpawner squadSpawner)
    {
        spawner = squadSpawner;
        CurrentWave = 0;
        previousWave = -1;
    }

    internal double GetNextWaveTime(double currentTime)
    {
        AudioClip clipToPlay = sequence[CurrentWave].GetTrackVariation();
        double duration = (double)clipToPlay.samples / clipToPlay.frequency;
        return currentTime + (duration / sequence[CurrentWave].wavesInTrack);
    }
}
