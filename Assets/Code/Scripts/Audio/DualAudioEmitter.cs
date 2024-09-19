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
    protected RadioStateController radioStateController;
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

    //Finds the bounds checker and radiostate controller, sets up event handlers
    protected virtual void Start()
    {
        boundsChecker = FindObjectOfType<BoundsChecker>();
        if (boundsChecker == null)
        {
            Debug.LogWarning("Could not find BoundsChecker");
        }

        radioStateController = FindObjectOfType<RadioStateController>();
        if (radioStateController == null)
        {
            Debug.LogWarning("Could not find RadioStateController");
        }
        else
        {
            radioStateController.radioPlaying.notifyListenersEnter += HandleRadioPlaying;
            radioStateController.radioPlaying.notifyListenersExit += HandleRadioNotPlaying;
            radioStateController.radioNotPlaying.notifyListenersEnter += HandleRadioNotPlaying;
            radioStateController.outOfBounds.notifyListenersEnter += HandleRadioNotPlaying;
            radioStateController.radioOff.notifyListenersEnter += HandleRadioNotPlaying;
        }
    }

    protected virtual void Update()
    {

    }

    public void Init()
    {
        toggle = 0;
        SetFullVolume();
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
            audioSourceArray[i].loop = false;
        }
        SetFullVolume();

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
    public virtual void SetVolume(float volume)
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
    /// Looping call for the level complete state entry
    /// </summary>
    public void Loop()
    {
        audioSourceArray[1 - toggle].loop = enabled;
    }

    /// <summary>
    /// Sets volume back to where it was
    /// </summary>
    private void SetFullVolume()
    {
        SetVolume(fullVolume);
    }

    /// <summary>
    /// Handles volume dimming when going in and out of the TransmissionArea (determined
    /// by the Radio State Controller)
    /// </summary>
    protected virtual void HandleRadioPlaying()
    {
        float percentageOfFull = .4f;
        float dimVolume = fullVolume - percentageOfFull;

        SetVolume(dimVolume);
    }

    protected virtual void HandleRadioNotPlaying()
    {
        SetFullVolume();
    }
}
