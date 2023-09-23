using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A track is a container of all associated music clips.
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
        //if (CurrentWave >= previousWave || CurrentWave < previousWave - 1)
        {
            spawner.SpawnWave(sequence[CurrentWave].GetWaveAi());
            previousWave = CurrentWave;
        }
    }

    internal void UpdateCurrentWave()
    {
        int index = 0;
        for (; index < sequence.Count; index++)
        {
            if (!sequence[index].IsOverThreshold())
            {
                Debug.Log("Current Wave: " + (index) + "/" + (sequence.Count - 1));
                break;
            }
        }
        CurrentWave = index;
    }
}
