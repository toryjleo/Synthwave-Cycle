using System;
using UnityEngine;

/// <summary>
/// Handles logic specific to playing the radio noises
/// requires: AudioSource with a looping white noise track
/// </summary>
public class DualRadioEmitter : DualAudioEmitter
{

    [SerializeField] private AudioSource noisePlayer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Mute(true);

        if (!noisePlayer)
        {
            Debug.LogWarning("No noise player in dual radio emitter!");
        }
        else
        {
            noisePlayer.mute = true;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        noisePlayer.volume = 1 - boundsChecker.TransmissionClarity;
    }

    public override void Mute(bool enabled)
    {
        base.Mute(enabled);
        noisePlayer.mute = enabled;
    }

    protected override void HandleRadioPlaying()
    {
        Mute(false);
    }

    protected override void HandleRadioNotPlaying()
    {
        Mute(true);
    }
}
