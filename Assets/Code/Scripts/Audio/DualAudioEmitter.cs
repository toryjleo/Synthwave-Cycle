using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualAudioEmitter : MonoBehaviour
{
    #region AudioControl
    // An audiosource array with 2 members to switch between with "toggle"
    public AudioSource[] audioSourceArray;
    int toggle;
    #endregion

    public void Init()
    {
        toggle = 0;
    }

    /// <summary>
    /// Adds the next song to play to the queue of audiosources
    /// </summary>
    public void QueueNextSong(AudioClip clipToPlay, double nextAudioLoopTime)
    {
        // Loads the next Clip to play and schedules when it will start
        audioSourceArray[toggle].clip = clipToPlay;
        audioSourceArray[toggle].PlayScheduled(nextAudioLoopTime);
        // Switches the toggle to use the other Audio Source next
        toggle = 1 - toggle;

    }

    public void ResetGameObject()
    {
        for (int i = 0; i < audioSourceArray.Length; i++)
        {
            audioSourceArray[i].Stop();
            audioSourceArray[i].clip = null;
        }

        toggle = 0;
    }

    public void Pause()
    {
        audioSourceArray[1 - toggle].Play();
    }

    public void Play() 
    {
        audioSourceArray[1 - toggle].Pause();
    }

    public void Mute(bool enabled) 
    {
        audioSourceArray[1 - toggle].mute = enabled;
        audioSourceArray[toggle].mute = enabled;
    }
}
