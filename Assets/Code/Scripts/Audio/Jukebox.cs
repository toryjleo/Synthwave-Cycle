using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO: look at https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/


/// <summary>
/// A class that plays a random track on start.
/// </summary>
public class Jukebox : MonoBehaviour, IResettable
{
    //The current WaveSequence in play
    private EditorObject.WaveSequence sequence;

    [SerializeField] private DualAudioEmitter dualAudioEmitter;

    double nextAudioLoopTime;
    double nextWaveSpawnTime;

    private bool canPlay;
    double nextAudioLoopDifference;

    //Resets the jukebox and starts a new Wave Sequence
    public void Init(EditorObject.WaveSequence seq)
    {
        sequence = seq;
        sequence.Init(GameObject.FindObjectOfType<SquadSpawner>());

        if (dualAudioEmitter == null)
        {
            Debug.LogError("Jukebox has no DualAudioEmitter assigned");
        }
        else 
        {
            dualAudioEmitter.Init();
        }


        canPlay = false;
        nextAudioLoopDifference = 0;

        GameStateController.playing.notifyListenersEnter += HandlePlayingEnter;
        GameStateController.playing.notifyListenersExit += HandlePlayingExit;
    }

    private void Update()
    {
        if (canPlay)
        {
            // Schedule next track 1 second before this track ends
            if (AudioSettings.dspTime > nextAudioLoopTime - 1)
            {
                QueueNextSong();
            }
            // Logic for spawning in next wave
            if (AudioSettings.dspTime > nextWaveSpawnTime)
            {
                sequence.SpawnNewWave();
                nextWaveSpawnTime = sequence.GetNextWaveTime(nextWaveSpawnTime);
            }
        }

    }

    /// <summary>
    /// Adds the next song to play to the queue of audiosources
    /// </summary>
    private void QueueNextSong()
    {
        AudioClip clipToPlay = sequence.GetCurrentTrackVariation();
        dualAudioEmitter.QueueNextSong(clipToPlay, nextAudioLoopTime);
        // Checks how long the Clip will last and updates the Next Start Time with a new value
        double duration = (double)clipToPlay.samples / clipToPlay.frequency;
        nextAudioLoopTime = nextAudioLoopTime + duration;
    }

    private double GetClipDuration(AudioClip clip)
    {
        double duration = (double)clip.samples / clip.frequency;
        return duration;
    }

    public void ResetGameObject()
    {
        dualAudioEmitter.ResetGameObject();
        canPlay = false;
        nextAudioLoopDifference = 0;
    }

    private void HandlePlayingEnter()
    {
        dualAudioEmitter.HandlePlayingEnter();

        double startTime = AudioSettings.dspTime + 0.2;

        if (nextAudioLoopDifference != 0)
        {
            startTime = AudioSettings.dspTime + nextAudioLoopDifference;
        }

        nextAudioLoopTime = startTime;
        nextWaveSpawnTime = startTime;

        canPlay = true;
    }

    private void HandlePlayingExit()
    {
        dualAudioEmitter.HandlePlayingExit();
        nextAudioLoopDifference = nextAudioLoopTime - AudioSettings.dspTime;

        canPlay = false;
    }
}
