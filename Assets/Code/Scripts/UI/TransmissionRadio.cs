using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransmissionRadio : MonoBehaviour
{
    private TransmissionArea transmissionArea;
    private PlayerMovement playerMovement;
    private LevelManager levelManager;

    [SerializeField] private GameObject radioFrame;
    [SerializeField] private Image radioFace;
    [SerializeField] private GameObject wifiSignal;


    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        transmissionArea = FindObjectOfType<TransmissionArea>();
        playerMovement = FindObjectOfType<PlayerMovement>();

        if (!levelManager)
        {
            Debug.LogWarning("No level manager found!");
        }
        else if (!levelManager.RadioFace)
        {
            Debug.LogWarning("Level does not have a face for radio!");
        }
        else
        {
            radioFace.sprite = levelManager.RadioFace;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transmissionArea != null && playerMovement != null)
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
