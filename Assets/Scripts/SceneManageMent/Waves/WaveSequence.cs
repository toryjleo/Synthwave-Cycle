using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    return sequence[currentWave].GetTrackVariation();
  }
  /// <summary>
  /// Updates the wave and spawns in wave according to danger level
  /// </summary>
  public void SpawnNewWave()
  {
    UpdateCurrentWave();
    spawner.SpawnWave(sequence[currentWave].GetWaveInfo());
    previousWave = currentWave;
  }

  /// <summary>
  /// Sets the currentWave number based on Danger Level
  /// </summary>
  internal void UpdateCurrentWave()
  {
    for (int index = sequence.Count - 1; index >= 0; index--)
    {
      // Iterate backwards and spawn in the waves for the highest danger level
      if (sequence[index].IsOverThreshold())
      {
        currentWave = index;
        //Log the wave + 1 because the index starts at 0, but the tracks start at 1
        Debug.Log("Current Wave: " + (currentWave + 1) + "/" + (sequence.Count) + "\nDanger Level: " + DangerLevel.Instance.GetDangerLevel());
        break;
      }
      for (int index = sequence.Count - 1; index >= 0; index--)
      {
        // Iterate backwards and spawn in the waves for the highest danger level
        if (sequence[index].IsOverThreshold())
        {
          currentWave = index;
          //Log the wave + 1 because the index starts at 0, but the tracks start at 1
          Debug.Log("Current Wave: " + (currentWave + 1) + "/" + (sequence.Count) + "\nDanger Level: " + DangerLevel.Instance.GetDangerLevel());
          break;
        }
      }
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
    AudioClip clipToPlay = sequence[currentWave].GetTrackVariation();
    double duration = (double)clipToPlay.samples / clipToPlay.frequency;
    return currentTime + (duration / sequence[currentWave].wavesInTrack);
  }

  public void ResetGameObject()
  {
    currentWave = 0;
    previousWave = -1;
  }
}
