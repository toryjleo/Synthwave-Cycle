using System.Collections;
using System.Collections.Generic;
using EditorObject;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private SettingsData settingsData;
    [SerializeField] private Slider mainVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
    [SerializeField] public AudioMixer audioMixer;

    void Start()
    {
        mainVolumeSlider.value = settingsData.MainVolume;
        musicVolumeSlider.value = settingsData.MusicVolume;
        effectsVolumeSlider.value = settingsData.EffectsVolume;
    }

    public void SetMainVolume(float volume)
    {
        audioMixer.SetFloat("MainVolume", volume);
        settingsData.MainVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        settingsData.MusicVolume = volume;
    }

    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("EffectsVolume", volume);
        settingsData.EffectsVolume = volume;
    }
}
