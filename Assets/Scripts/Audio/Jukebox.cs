using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO: look at https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/


/// <summary>
/// A class that plays a random track on start.
/// </summary>
public class Jukebox : MonoBehaviour, IResettable
{
    //The list of each possible WaveSequence, to be randomly selected on reset
    [SerializeField]
    public List<WaveSequence> soundTracks;
    //The current WaveSequence in play
    public WaveSequence sequence;
    // An audiosource array with 2 members to switch between with "toggle"
    public AudioSource[] audioSourceArray;
    int toggle;

    double nextAudioLoopTime;
    double nextWaveSpawnTime;


    void Start()
    {
        if (sequence == null)
        {
            sequence = soundTracks[Random.Range(0, soundTracks.Count)];
        }
        sequence.Init(GameObject.FindObjectOfType<SquadSpawner>());
        toggle = 0;
        AudioClip clip = sequence.GetCurrentTrackVariation();
        // Schedule the first track
        audioSourceArray[toggle].clip = clip;

        double startTime  = AudioSettings.dspTime + 0.2;
        nextAudioLoopTime = startTime;
        nextWaveSpawnTime = startTime;

        audioSourceArray[toggle].PlayScheduled(startTime);
        toggle = 1 - toggle;
    }

    private void Update()
    {
        // Schedule next track 1 second before this track ends
        if (AudioSettings.dspTime > nextAudioLoopTime - 1) 
        {
            QueueNextSong();
        }
        if(AudioSettings.dspTime > nextWaveSpawnTime)
        {
            sequence.CheckForWaveSpawn();
            nextWaveSpawnTime = sequence.GetNextWaveTime(nextWaveSpawnTime);
        }

    }

    /// <summary>
    /// Adds the next song to play to the queue of audiosources
    /// </summary>
    private void QueueNextSong()
    {
        AudioClip clipToPlay = sequence.GetCurrentTrackVariation();
        // Loads the next Clip to play and schedules when it will start
        audioSourceArray[toggle].clip = clipToPlay;
        audioSourceArray[toggle].PlayScheduled(nextAudioLoopTime);
        // Checks how long the Clip will last and updates the Next Start Time with a new value
        double duration = (double)clipToPlay.samples / clipToPlay.frequency;
        nextAudioLoopTime = nextAudioLoopTime + duration;
        // Switches the toggle to use the other Audio Source next
        toggle = 1 - toggle;
    }


    private double GetClipDuration(AudioClip clip)
    {
        double duration = (double)clip.samples / clip.frequency;
        return duration;
    }

    public void ResetGameObject()
    {
        for (int i = 0; i < audioSourceArray.Length; i++)
        {
            audioSourceArray[i].Stop();
            audioSourceArray[i].clip = null;
        }
        sequence = soundTracks[Random.Range(0, soundTracks.Count)];
        Start();
    }
}
