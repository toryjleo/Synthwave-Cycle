using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransmissionRadio : MonoBehaviour
{
    private BoundsChecker boundsChecker;
    private LevelManager levelManager;
    private RadioStateController radioStateController;

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
        radioStateController = FindObjectOfType<RadioStateController>();
        if (radioStateController == null)
        {
            Debug.LogWarning("TransmissionRadio does not have reference to a RadioStateController in scene");
        }
        else
        {
            radioStateController.outOfBounds.notifyListenersEnter += HandleRadioIsNotPlaying;
            radioStateController.radioPlaying.notifyListenersEnter += HandleRadioIsPlaying;
            radioStateController.radioNotPlaying.notifyListenersEnter += HandleRadioIsNotPlaying;
            radioStateController.radioOff.notifyListenersEnter += HandleRadioOff;
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
    }

    private void Update()
    {
        if (radioFrame.activeSelf)
        {
            radioFace.color = new Color(1, boundsChecker.TransmissionClarity, boundsChecker.TransmissionClarity, 1);
        }
    }

    private void HandleRadioIsPlaying()
    {
        wifiSignal.gameObject.SetActive(false);
        radioFrame.gameObject.SetActive(true);
    }

    private void HandleRadioIsNotPlaying()
    {
        wifiSignal.gameObject.SetActive(true);
        radioFrame.gameObject.SetActive(false);
    }

    private void HandleRadioOff()
    {
        wifiSignal.gameObject.SetActive(false);
        radioFrame.gameObject.SetActive(false);
    }

    public void Toggle()
    {
        // ToggleRadioEmitter();
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
