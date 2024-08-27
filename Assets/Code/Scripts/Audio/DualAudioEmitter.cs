using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualAudioEmitter : MonoBehaviour
{
    // An audiosource array with 2 members to switch between with "toggle"
    public AudioSource[] audioSourceArray;
    int toggle;

    private Coroutine coroutine;
    private const float fullVolume = .7f;

    protected BoundsChecker boundsChecker;
    private float timeTillTrackEnds = 0;

    public float TimeTillTrackEnds
    {
        set { timeTillTrackEnds = value; }
    }

    public bool isPlaying
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
            boundsChecker.onCrossedTransmissionBounds += HandleTransmissionBoundsEvent;
        }
    }

    protected virtual void Update()
    {
        timeTillTrackEnds -= Time.deltaTime;

        // Debug.Log("Time till this track ends: " + timeTillTrackEnds);
    }

    public void Init()
    {
        toggle = 0;
        StopCoroutineSetFullVolume();
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
        StopCoroutineSetFullVolume();

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

    public virtual void Mute(bool enabled)
    {
        audioSourceArray[1 - toggle].mute = enabled;
        audioSourceArray[toggle].mute = enabled;
    }

    private void DimForTime(double timeLength)
    {
        StopCoroutineSetFullVolume();
        if (timeLength > 0)
        {
            coroutine = StartCoroutine(DimForTimeCoroutine(timeLength));
        }
    }

    private void SetVolume(float volume)
    {
        audioSourceArray[toggle].volume = volume;
        audioSourceArray[1 - toggle].volume = volume;
    }

    private IEnumerator DimForTimeCoroutine(double timeLength)
    {
        float percentageOfFull = .1f;
        float dimVolume = fullVolume * percentageOfFull;

        SetVolume(dimVolume);

        yield return new WaitForSeconds((float)(timeLength));

        SetVolume(fullVolume);
        coroutine = null;
    }

    private void StopCoroutineSetFullVolume()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;

        }
        SetVolume(fullVolume);
    }

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
