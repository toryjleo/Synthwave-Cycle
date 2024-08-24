using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransmissionRadio : MonoBehaviour
{
    private BoundsChecker boundsChecker;
    private LevelManager levelManager;
    private bool inBounds;

    [SerializeField] private GameObject radioFrame;
    [SerializeField] private Image radioFace;
    [SerializeField] private GameObject wifiSignal;
    [SerializeField] private AudioSource radio1;
    [SerializeField] private AudioSource radio2;


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
        }

        if (radioFrame.activeSelf && !radio1.isPlaying && !radio2.isPlaying)
        {
            radioFrame.SetActive(false);
            wifiSignal.SetActive(true);
        }
        else if (!radioFrame.activeSelf && inBounds && (radio1.isPlaying || radio2.isPlaying))
        {
            radioFrame.SetActive(true);
            wifiSignal.SetActive(false);
        }
    }

    private void HandleTransmissionBoundsEvent(bool isWithinBounds)
    {
        inBounds = isWithinBounds;
        wifiSignal.SetActive(!isWithinBounds);
        radioFrame.SetActive(isWithinBounds);
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
