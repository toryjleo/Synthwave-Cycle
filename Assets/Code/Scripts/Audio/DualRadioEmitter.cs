using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualRadioEmitter : DualAudioEmitter
{
    private BoundsChecker boundsChecker;

    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleTransmissionBoundsEvent(bool isWithinBounds) 
    {
        Debug.Log("Is within bounds: " + isWithinBounds);
    }
}
