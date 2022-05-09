using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO: look at https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/


/// <summary>
/// A class that plays a random track on start.
/// </summary>
public class Jukebox : MonoBehaviour
{
    private bool scheduleas1;
    public Track track;
    public AudioSource[] audioSourceArray;
    int toggle;

    double nextStartTime;


    void Start()
    {
        toggle = 0;
        TrackVariation tv = track.variations[0];

        audioSourceArray[toggle].clip = tv.variation;

        double startTime  = AudioSettings.dspTime + 0.2;
        double introDuration = GetClipDuration(tv.variation);
        nextStartTime = startTime + introDuration;

        audioSourceArray[toggle].PlayScheduled(startTime);
        toggle = 1 - toggle;

    }

    private void Update()
    {
        
        if (AudioSettings.dspTime > nextStartTime - 1) 
        {
            // TODO: Make dynamic
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
        
    }


    private double GetClipDuration(AudioClip clip)
    {
        double duration = (double)clip.samples / clip.frequency;
        return duration;
    }


    /*private void CheckTrack() 
    {
        audioSourceArray[toggle].Play();
        Invoke("CheckTrack", audioSourceArray[toggle].clip.length);
    }



    /// <summary>
    /// Plays a specified track.
    /// </summary>
    /// <param name="newClip">The new track to switch to.</param>
    private void ChangeTrack(AudioClip newClip) 
    {
        audioSourceArray[toggle].Stop();
        audioSourceArray[toggle].clip = newClip;
        audioSourceArray[toggle].Play();
    }*/
}
