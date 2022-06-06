using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO: look at https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/


/// <summary>
/// A class that plays a random track on start.
/// </summary>
public class Jukebox : MonoBehaviour
{
    private Track track;
    //A list of all the tracks to play
    public List<Track> trackList;
    //Used to queue up a new track
    private Track nextTrack = null;
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
        AudioClip clipToPlay = null;
        if (nextTrack != null)
        {
            track = nextTrack;
            nextTrack = null;
            clipToPlay = track.intro;
        }
        else
        {
            // TODO: Make dynamic
            int dl = DLevel.Instance.GetDangerLevel();
            int variationIndex = 0;
            for (; variationIndex < track.variations.Count; variationIndex++)
            {
                if (track.variations[variationIndex].dangerLevelStart >= dl)
                {
                    break;
                }
            }
            Debug.unityLogger.Log("Danger level: " + dl + ". Queueing variation: " + variationIndex);
            clipToPlay = track.variations[variationIndex].variation;
        }

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

    /// <summary>
    /// Sets the next track to play after the current loop
    /// </summary>
    public void SetNextTrack(Track newTrack)
    {
        nextTrack = newTrack;
    }
}
