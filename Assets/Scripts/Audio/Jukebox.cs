using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO: look at https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/


/// <summary>
/// A class that plays a random track on start.
/// </summary>
public class Jukebox : MonoBehaviour, IResettable
{
    //The list of each possible WaveSequence, to be randomly selected on reset
    [SerializeField]
    public List<WaveSequence> soundTracks;
    //The current WaveSequence in play
    public WaveSequence sequence;
    // An audiosource array with 2 members to switch between with "toggle"
    public AudioSource[] audioSourceArray;
    // TODO: Do not make dependant on changeScene script
    private ChangeScene changeScene;
    int toggle;

    double nextStartTime;


    void Start()
    {
        if (sequence == null)
        {
            sequence = soundTracks[Random.Range(0, soundTracks.Count)];
        }
        sequence.Init(GameObject.FindObjectOfType<SquadSpawner>());
        changeScene = GameObject.FindObjectOfType<ChangeScene>();
        toggle = 0;
        AudioClip clip = sequence.GetCurrentTrackVariation();
        // Schedule the first track
        audioSourceArray[toggle].clip = clip;

        double startTime  = AudioSettings.dspTime + 0.2;
        nextStartTime = startTime;

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
        AudioClip clipToPlay = sequence.GetCurrentTrackVariation();
        // Loads the next Clip to play and schedules when it will start
        audioSourceArray[toggle].clip = clipToPlay;
        audioSourceArray[toggle].PlayScheduled(nextStartTime);
        // Checks how long the Clip will last and updates the Next Start Time with a new value
        double duration = (double)clipToPlay.samples / clipToPlay.frequency;
        nextStartTime = nextStartTime + duration;
        // Switches the toggle to use the other Audio Source next
        toggle = 1 - toggle;

        // Check to see if this is going to be the last wave in the sequence
        if (sequence.IsLastWaveSequence())
        {
            LoadMainMenu();
        }
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
            audioSourceArray[i].clip = null;
        }
        sequence = soundTracks[Random.Range(0, soundTracks.Count)];
        Start();
    }

    // TODO: try to move this logic out
    private void LoadMainMenu()
    {
        AudioSource source0 = audioSourceArray[1 - toggle];
        float source0RemainingTime = GetClipRemainingTime(source0);
        // TODO: cover case where player dies after this is called
        Debug.Log("Call ChangeScene.ReturnToMainMenu() and set time for 1 second");
        changeScene.TryReturnMainMenu(source0RemainingTime);
    }

    // TODO: got error before. Need to move to a static class: Extension method must be defined in a non-generic static class
    // Got code from: https://gamedev.stackexchange.com/questions/169339/efficient-way-to-detect-when-audio-clip-ends
    #region utils
    public float GetClipRemainingTime(AudioSource source)
    {
        // Calculate the remainingTime of the given AudioSource,
        // if we keep playing with the same pitch.
        float remainingTime = (source.clip.length - source.time) / source.pitch;
        return IsReversePitch(source) ?
            (source.clip.length + remainingTime) :
            remainingTime;
    }
    
    public bool IsReversePitch(AudioSource source)
    {
        return source.pitch < 0f;
    }
    #endregion
}
