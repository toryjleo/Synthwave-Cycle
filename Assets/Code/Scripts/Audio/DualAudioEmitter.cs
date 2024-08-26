using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles chaining 2 AudioSources together in time
/// Requires: 2 audiosources put into an array
/// </summary>
public class DualAudioEmitter : MonoBehaviour
{
    #region Members

    #region Audio Toggling
    // An audiosource array with 2 members to switch between with "toggle"
    [SerializeField] private AudioSource[] audioSourceArray;
    int toggle;
    #endregion

    #region Volume Management
    private Coroutine coroutine;
    private const float fullVolume = .7f;
    #endregion


    protected BoundsChecker boundsChecker;
    private float timeTillTrackEnds = 0;

    #endregion

    public float TimeTillTrackEnds
    {
        set { timeTillTrackEnds = value; }
    }

    public bool IsPlaying
    {
        get { return audioSourceArray[toggle].isPlaying || audioSourceArray[1 - toggle].isPlaying; }
    }


    protected virtual void Start()
    {
        boundsChecker = FindObjectOfType<BoundsChecker>();
        if (boundsChecker == null)
        {
            Debug.LogWarning("Could not find BoundsChecker");
        }
        else
        {
            boundsChecker.transmissionBoundsEvent += HandleTransmissionBoundsEvent;
        }
    }

    protected virtual void Update()
    {
        timeTillTrackEnds -= Time.deltaTime;
    }

    public void Init()
    {
        toggle = 0;
        StopCoroutineSetFullVolume();
    }

    /// <summary>
    /// Adds the next song to play to the queue of audiosources
    /// </summary>
    /// <param name="clipToPlay">AudioClip to play next</param>
    /// <param name="nextAudioLoopTime">Time to wait before playing the clip</param>
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
        StopCoroutineSetFullVolume();

        toggle = 0;
    }

    #region Track Control

    /// <summary>
    /// Plays the audiotrack
    /// </summary>
    public void Play()
    {
        audioSourceArray[1 - toggle].Play();
    }

    /// <summary>
    /// Pauses the audiotrack
    /// </summary>
    public void Pause()
    {
        audioSourceArray[1 - toggle].Pause();
    }

    /// <summary>
    /// Sets the volume of both audiosources to specified volume.
    /// </summary>
    /// <param name="volume">Volume level to set audiosources to</param>
    public void SetVolume(float volume)
    {
        audioSourceArray[toggle].volume = volume;
        audioSourceArray[1 - toggle].volume = volume;
    }

    #endregion

    /// <summary>
    /// Handles muting both audiotracks
    /// </summary>
    /// <param name="enabled">Will mute tracks if true, else unmute</param>
    public virtual void Mute(bool enabled)
    {
        audioSourceArray[1 - toggle].mute = enabled;
        audioSourceArray[toggle].mute = enabled;
    }

    /// <summary>
    /// Dims the volumes of the tracks for a specified time
    /// </summary>
    /// <param name="timeLength">Length of time to dim for</param>
    private void DimForTime(double timeLength)
    {
        StopCoroutineSetFullVolume();
        if (timeLength > 0)
        {
            coroutine = StartCoroutine(DimForTimeCoroutine(timeLength));
        }
    }

    /// <summary>
    /// Dims to 10% volume for an amount of time
    /// </summary>
    /// <param name="timeLength">Amount of time to dim for</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator DimForTimeCoroutine(double timeLength)
    {
        float percentageOfFull = .1f;
        float dimVolume = fullVolume * percentageOfFull;

        SetVolume(dimVolume);

        yield return new WaitForSeconds((float)(timeLength));

        SetVolume(fullVolume);
        coroutine = null;
    }

    /// <summary>
    /// Stops the current coroutine and sets volume back to where it was
    /// </summary>
    private void StopCoroutineSetFullVolume()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;

        }
        SetVolume(fullVolume);
    }

    /// <summary>
    /// Handles volume dimming when going in and out of the TransmissionArea
    /// </summary>
    /// <param name="isWithinBounds">True if inside of bounds</param>
    protected virtual void HandleTransmissionBoundsEvent(bool isWithinBounds)
    {
        if (!isWithinBounds)
        {
            StopCoroutineSetFullVolume();
        }
        else
        {
            DimForTime(timeTillTrackEnds);
        }
    }
}
