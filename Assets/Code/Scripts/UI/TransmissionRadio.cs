using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransmissionRadio : MonoBehaviour
{
    private BoundsChecker boundsChecker;
    private LevelManager levelManager;

    [SerializeField] private GameObject radioFrame;
    [SerializeField] private Image radioFace;
    [SerializeField] private GameObject wifiSignal;


    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        boundsChecker = FindObjectOfType<BoundsChecker>();
        if (boundsChecker == null) 
        {
            Debug.LogWarning("TransmissionRadio does not have reference to a BoundsChecker in scene");
        }
        else 
        {
            boundsChecker.transmissionBoundsEvent += HandleTransmissionBoundsEvent;
        }

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
        // Disable radio at start
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (radioFrame.activeSelf) 
        {
            radioFace.color = new Color(1, boundsChecker.TransmissionClarity, boundsChecker.TransmissionClarity, 1);
            // TODO: apply transmissionclarity
        }
    }

    private void HandleTransmissionBoundsEvent(bool isWithinBounds)
    {
        wifiSignal.SetActive(!isWithinBounds);
        radioFrame.SetActive(isWithinBounds);
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
