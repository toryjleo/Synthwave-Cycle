using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO: look at https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/


/// <summary>
/// A class that plays a random track on start.
/// </summary>
public class Jukebox : MonoBehaviour
{
    public Track track;
    // An audiosource array with 2 members to switch between with "toggle"
    public AudioSource[] audioSourceArray;
    int toggle;

    double nextStartTime;


    void Start()
    {
        toggle = 0;
        TrackVariation tv = track.variations[0];
        // Schedule the first track
        audioSourceArray[toggle].clip = tv.variation;

        double startTime  = AudioSettings.dspTime + 0.2;
        double introDuration = GetClipDuration(tv.variation);
        nextStartTime = startTime + introDuration;

        audioSourceArray[toggle].PlayScheduled(startTime);
        toggle = 1 - toggle;

    }

    private void Update()
    {
        // Schedule next track 1 second before this track ends
        if (AudioSettings.dspTime > nextStartTime - 1) 
        {
            QueueNextSong();
        }
        
    }

    /// <summary>
    /// Adds the next song to play to the queue of audiosources
    /// </summary>
    private void QueueNextSong() 
    {
        // TODO: Make dynamic
        // TODO: Schedule the next track based on the current danger level
        AudioClip clipToPlay = track.variations[0].variation;

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
}
