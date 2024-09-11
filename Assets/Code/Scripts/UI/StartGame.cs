using System.Collections;
using System.Collections.Generic;
using EditorObject;
using UnityEngine;
using UnityEngine.Audio;

public class StartGame : MonoBehaviour
{
    [SerializeField] private SettingsData settingsData;
    [SerializeField] public AudioMixer audioMixer;

    // Start is called before the first frame update
    void Start()
    {
        audioMixer.SetFloat("MainVolume", settingsData.MainVolume);
        audioMixer.SetFloat("MusicVolume", settingsData.MusicVolume);
        audioMixer.SetFloat("EffectsVolume", settingsData.EffectsVolume);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameStateController.HandleTrigger(StateTrigger.StartGame);
        }
    }
}
