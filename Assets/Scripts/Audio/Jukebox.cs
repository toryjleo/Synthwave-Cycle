using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO: look at https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/


/// <summary>
/// A class that plays a random track on start.
/// </summary>
public class Jukebox : MonoBehaviour
{
    private bool started;
    public Track track;
    public AudioSource audioSource;


    void Awake()
    {
        started = false;
        audioSource = GetComponent<AudioSource>();
        TrackVariation tv = track.variations[0];
        audioSource.clip = tv.variation;


        // Calculate a Clip’s exact duration
    }

    private void Update()
    {
        if (!started) 
        {
            started = true;
            // The first time the track is called, there is a delay after the first play
            Invoke("CheckTrack", .05f);
        }
    }

    private void CheckTrack() 
    {
        audioSource.Play();
        Invoke("CheckTrack", audioSource.clip.length);
    }



    /// <summary>
    /// Plays a specified track.
    /// </summary>
    /// <param name="newClip">The new track to switch to.</param>
    private void ChangeTrack(AudioClip newClip) 
    {
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();
    }
}
