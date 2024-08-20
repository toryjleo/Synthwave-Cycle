using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmissionRadio : MonoBehaviour
{
    private TransmissionArea transmissionArea;
    private PlayerMovement playerMovement;

    [SerializeField] private GameObject radioFrame;
    [SerializeField] private GameObject wifiSignal;


    // Start is called before the first frame update
    void Start()
    {
        transmissionArea = FindObjectOfType<TransmissionArea>();
        playerMovement = FindObjectOfType<PlayerMovement>();


    }

    // Update is called once per frame
    void Update()
    {
        if(transmissionArea != null && playerMovement != null)
        {
            if (TransmissionIsComingIn())
            {
                // Show radio box
                wifiSignal.SetActive(false);
                radioFrame.SetActive(true);
            }
            else
            {
                wifiSignal.SetActive(true);
                radioFrame.SetActive(false);
            }
        }
    }

    private bool TransmissionIsComingIn() 
    {
        return transmissionArea.TransmissionClarity(playerMovement.transform.position) > 0;
    }

    public void Toggle() 
    {
        gameObject.SetActive(!gameObject.activeSelf);

        wifiSignal.SetActive(false);
        radioFrame.SetActive(false);
    }
}
