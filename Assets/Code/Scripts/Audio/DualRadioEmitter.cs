using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualRadioEmitter : DualAudioEmitter
{

    private TransmissionArea transmissionArea;

    // Start is called before the first frame update
    void Start()
    {
        transmissionArea = FindObjectOfType<TransmissionArea>();
        if (transmissionArea == null) 
        {
            Debug.LogWarning("Could not find TransmissionArea in scene");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
