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
        spawner.SpawnWave(sequence[CurrentWave].GetWaveAi());
        previousWave = CurrentWave;
    }

    internal void UpdateCurrentWave()
    {
        for (int index = 0; index < sequence.Count; index++)
        {
            if (!sequence[index].IsOverThreshold())
            {
                Debug.Log("Current Wave: " + (index) + "/" + (sequence.Count - 1) + "\nDanger Level: " + DLevel.Instance.GetDangerLevel());
                CurrentWave = index;
                break;
            }
        }
    }
}
