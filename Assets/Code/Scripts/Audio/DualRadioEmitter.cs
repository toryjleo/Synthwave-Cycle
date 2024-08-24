using System;
using UnityEngine;

public class DualRadioEmitter : DualAudioEmitter
{

    [SerializeField]
    private AudioSource noisePlayer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

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
    void Update()
    {
        noisePlayer.volume = 1 - boundsChecker.TransmissionClarity;
    }

    protected override void HandleTransmissionBoundsEvent(bool isWithinBounds)
    {
        Mute(!isWithinBounds);
        noisePlayer.mute = !isWithinBounds;
    }
}
