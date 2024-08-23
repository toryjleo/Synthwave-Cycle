using UnityEngine;

public class DualRadioEmitter : DualAudioEmitter
{
    private BoundsChecker boundsChecker;

    [SerializeField]
    private AudioSource noisePlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (!noisePlayer)
        {
            Debug.LogWarning("No noise player in dual radio emitter!");
        }
        else
        {
            noisePlayer.mute = true;
        }

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

    // Update is called once per frame
    void Update()
    {
        noisePlayer.volume = 1 - boundsChecker.TransmissionClarity;
    }

    private void HandleTransmissionBoundsEvent(bool isWithinBounds)
    {
        Mute(!isWithinBounds);
        noisePlayer.mute = !isWithinBounds;
    }
}
