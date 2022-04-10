using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that plays a random track on start.
/// </summary>
public class Jukebox : MonoBehaviour
{
    public AudioClip[] playList;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        int length = playList.Length;
        if (length == 0)
        {
            Debug.LogError("Jukebox has 0 songs to play!");
        }
        else
        {
            int indexToPlay = Random.Range(0, length);
            ChangeTrack(playList[indexToPlay]);
        }
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
