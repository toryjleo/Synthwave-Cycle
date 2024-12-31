using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
// TODO: look at https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/

public delegate void NotifyRadioStatus(bool radioIsPlaying);

/// <summary>
/// A class that plays a random track on start.
/// </summary>
public class Jukebox : MonoBehaviour, IResettable
{
    //The current WaveSequence in play
    private EditorObject.WaveSequence sequence;

    [SerializeField] private DualAudioEmitter musicPlayer;
    [SerializeField] private DualAudioEmitter radioClipPlayer;
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private float radioVolume = 0.5f;
    public float MusicVolume { get => musicVolume; set => musicVolume = value; }
    public float RadioVolume { get => radioVolume; set => radioVolume = value; }

    double nextAudioLoopTime;
    double nextWaveSpawnTime;

    private bool canPlay;
    double nextAudioLoopDifference;

    public NotifyRadioStatus onRadioStatusUpdate;

    private bool radioWasPlayingLastFrame = false;

    //Resets the jukebox and starts a new Wave Sequence
    public void Init(EditorObject.WaveSequence seq)
    {
        sequence = seq;
        sequence.Init(GameObject.FindObjectOfType<EnemyManager>());

        InitializeDualAudioEmitter(musicPlayer);
        InitializeDualAudioEmitter(radioClipPlayer);

        musicPlayer.SetVolume(musicVolume);
        radioClipPlayer.SetVolume(radioVolume);

        canPlay = false;
        nextAudioLoopDifference = 0;

        GameStateController.playing.notifyListenersEnter += HandlePlayingEnter;
        GameStateController.playing.notifyListenersExit += HandlePlayingExit;
        GameStateController.levelComplete.notifyListenersEnter += HandleLevelComplete;
        GameStateController.playerDead.notifyListenersEnter += HandlePlayingExit;

        radioWasPlayingLastFrame = false;
    }

    private void Update()
    {
        if (canPlay)
        {
            // Schedule next track 1 second before this track ends
            if (AudioSettings.dspTime > nextAudioLoopTime - 1)
            {
                QueueNextSong();
            }
            // Logic for spawning in next wave
            if (AudioSettings.dspTime > nextWaveSpawnTime)
            {
                sequence.SpawnNewWave();
                nextWaveSpawnTime = sequence.GetNextWaveTime(nextWaveSpawnTime);
            }
        }


        if (radioClipPlayer.IsPlaying != radioWasPlayingLastFrame)
        {
            radioWasPlayingLastFrame = radioClipPlayer.IsPlaying;
            TriggerRadioStatusUpdate();
        }

    }

    /// <summary>
    /// Calls Init on a DualAudioEmitter with error checking
    /// </summary>
    /// <param name="emitter">Emitter to initialize</param>
    private void InitializeDualAudioEmitter(DualAudioEmitter emitter)
    {
        if (emitter == null)
        {
            Debug.LogError("Jukebox needs a DualAudioEmitter assigned");
        }
        else
        {
            emitter.Init();
        }
    }

    /// <summary>
    /// Adds the next song to play to the queue of audiosources, updates the current wave
    /// which checks to see if wave is progressing
    /// </summary>
    private void QueueNextSong()
    {
        if (sequence.CurrentWaveIsFinal)
        {
            //Play the LevelComplete wave's final audio loop
            AudioClip clipToPlay = sequence.GetCurrentTrackVariation();
            musicPlayer.QueueNextSong(clipToPlay, nextAudioLoopTime);

            sequence.SpawnNewWave();
        }
        else
        {
            //Check for next wave or same wave again
            sequence.UpdateCurrentWave();

            //Get music track for the wave
            AudioClip clipToPlay = sequence.GetCurrentTrackVariation();
            musicPlayer.QueueNextSong(clipToPlay, nextAudioLoopTime);

            if (sequence.CurrentTrackIsAudioWave && !sequence.CurrentTrackRadioWaveHasAlreadyPlayed)
            {
                AudioClip radioClip = sequence.GetCurrentRadioClip;
                if (radioClip != null)
                {
                    radioClipPlayer.QueueNextSong(radioClip, nextAudioLoopTime);
                }
                else
                {
                    Debug.LogError("Radio Wave does not have a radio clip attached");
                }
            }

            // Checks how long the Clip will last and updates the Next Start Time with a new value
            double duration = (double)clipToPlay.samples / clipToPlay.frequency;
            nextAudioLoopTime = nextAudioLoopTime + duration;
        }
    }

    /// <summary>
    /// Returns the duration of the clip
    /// </summary>
    /// <param name="clip">Audio clip to measure</param>
    /// <returns>A double representing time</returns>
    private double GetClipDuration(AudioClip clip)
    {
        double duration = (double)clip.samples / clip.frequency;
        return duration;
    }

    //Resets music and radio players, disallows music playing (due to resetting state)
    public void ResetGameObject()
    {
        musicPlayer.ResetGameObject();
        radioClipPlayer.ResetGameObject();
        canPlay = false;
        nextAudioLoopDifference = 0;
    }

    /// <summary>
    /// Handles logic for entering the playing state
    /// </summary>
    private void HandlePlayingEnter()
    {
        musicPlayer.Play();
        radioClipPlayer.Play();

        double startTime = AudioSettings.dspTime + 0.2;

        if (nextAudioLoopDifference != 0)
        {
            startTime = AudioSettings.dspTime + nextAudioLoopDifference;
        }

        nextAudioLoopTime = startTime;
        nextWaveSpawnTime = startTime;

        canPlay = true;
    }

    /// <summary>
    /// Handles logic for exiting the playing state
    /// </summary>
    private void HandlePlayingExit()
    {
        musicPlayer.Pause();
        radioClipPlayer.Pause();
        nextAudioLoopDifference = nextAudioLoopTime - AudioSettings.dspTime;

        canPlay = false;
    }

    /// <summary>
    /// Handles logic for completing the game
    /// </summary>
    private void HandleLevelComplete()
    {
        musicPlayer.Play();
        musicPlayer.Loop();
        radioClipPlayer.Pause();
        nextAudioLoopDifference = nextAudioLoopTime - AudioSettings.dspTime;

        canPlay = false;
    }

    public void HandleInBoundsEnter()
    {
        TriggerRadioStatusUpdate();
    }

    private void TriggerRadioStatusUpdate()
    {
        onRadioStatusUpdate?.Invoke(radioClipPlayer.IsPlaying);
    }
}
