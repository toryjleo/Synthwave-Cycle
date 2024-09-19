using System.Collections;
using System.Collections.Generic;
using EditorObject;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Handles the GameplayUI screen when loading is finished
/// </summary>
public class StartGame : MonoBehaviour
{
    [SerializeField] private SettingsData settingsData;
    [SerializeField] public AudioMixer audioMixer;

    // Start is called before the first frame update
    void Start()
    {
        SetMixerNumbers();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameStateController.HandleTrigger(StateTrigger.StartGame);
        }
    }

    /// <summary>
    /// Sets the audio mixer channels to their corresponding settingsData fields
    /// </summary>
    private void SetMixerNumbers()
    {
        audioMixer.SetFloat("MainVolume", settingsData.MainVolume);
        audioMixer.SetFloat("MusicVolume", settingsData.MusicVolume);
        audioMixer.SetFloat("EffectsVolume", settingsData.EffectsVolume);
    }
}
