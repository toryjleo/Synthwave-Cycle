using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO: look at https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/


/// <summary>
/// A class that plays a random track on start.
/// </summary>
public class Jukebox : MonoBehaviour, IResettable
{
    [SerializeField]
    public List<WaveSequence> soundTracks;
    private WaveSequence sequence;
    // An audiosource array with 2 members to switch between with "toggle"
    public AudioSource[] audioSourceArray;
    int toggle;

    double nextStartTime;


    void Start()
    {
        sequence = soundTracks[Random.Range(0, soundTracks.Count)];
        sequence.spawner = GameObject.FindObjectOfType<SquadSpawner>();
        toggle = 0;
        sequence.UpdateCurrentWave();
        AudioClip clip = sequence.GetCurrentTrackVariation();
        // Schedule the first track
        audioSourceArray[toggle].clip = clip;

        double startTime  = AudioSettings.dspTime + 0.2;
        double introDuration = GetClipDuration(clip);
        nextStartTime = startTime + introDuration;

        audioSourceArray[toggle].PlayScheduled(startTime);
        toggle = 1 - toggle;

    }

    private void Update()
    {
        // Schedule next track 1 second before this track ends
        if (AudioSettings.dspTime > nextStartTime - 1) 
        {
            sequence.CheckForWaveSpawn();
            QueueNextSong();
        }
        
    }

    /// <summary>
    /// Adds the next song to play to the queue of audiosources
    /// </summary>
    private void QueueNextSong()
    {
        //AudioClip clipToPlay = null;
        //// TODO: Make dynamic
        //int dl = DLevel.Instance.GetDangerLevel();
        //int variationIndex = 0;
        //for (; variationIndex < sequence.variations.Count; variationIndex++)
        //{
        //    if (sequence.variations[variationIndex].dangerLevelStart >= dl)
        //    {
        //        break;
        //    }
        //}
        //Debug.unityLogger.Log("Danger level: " + dl + ". Queueing variation: " + variationIndex);
        //clipToPlay = sequence.variations[variationIndex].variation;
        AudioClip clipToPlay = sequence.GetCurrentTrackVariation();
        // Loads the next Clip to play and schedules when it will start
        audioSourceArray[toggle].clip = clipToPlay;
        audioSourceArray[toggle].PlayScheduled(nextStartTime);
        // Checks how long the Clip will last and updates the Next Start Time with a new value
        double duration = (double)clipToPlay.samples / clipToPlay.frequency;
        nextStartTime = nextStartTime + duration;
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
        }
        Start();
    }
}
