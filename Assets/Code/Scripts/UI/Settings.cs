using System.Collections;
using System.Collections.Generic;
using EditorObject;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Settings manages the user-determined settings, attached to the settings screens
/// in the unity scenes
/// </summary>
public class Settings : MonoBehaviour
{
    [SerializeField] private SettingsData settingsData;
    [SerializeField] private Slider mainVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    private Resolution[] resolutions;
    private bool isFullscreen;

    void Start()
    {
        SetSlidersToSettingsData();
        SetUpResolutionSettings();
    }

    /// <summary>
    /// Sets slider values to corresponding settingsData fields
    /// </summary>
    private void SetSlidersToSettingsData()
    {
        mainVolumeSlider.value = settingsData.MainVolume;
        musicVolumeSlider.value = settingsData.MusicVolume;
        effectsVolumeSlider.value = settingsData.EffectsVolume;
    }

    /// <summary>
    /// Calculates the possible resolutions for the systems,
    /// populates the dropdown with those resolutions options,
    /// determines the resolution the player launched with.
    /// ALSO: sets the fullScreenToggle checkbox depending on that status at launch
    /// </summary>
    private void SetUpResolutionSettings()
    {
        //Set up options for the resolution-selecting dropdown
        resolutions = Screen.resolutions;
        if (!resolutionDropdown)
        {
            Debug.LogError("No reference to dropdown in options screen!");
            return;
        }
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        //Fullscreen check
        isFullscreen = Screen.fullScreen;
        fullScreenToggle.isOn = isFullscreen;
    }

    /// <summary>
    /// Sets the main volume channel, along with the settings object MainVolume
    /// </summary>
    /// <param name="volume">float value of the slider</param>
    public void SetMainVolume(float volume)
    {
        audioMixer.SetFloat("MainVolume", volume);
        settingsData.MainVolume = volume;
    }

    /// <summary>
    /// Sets the music volume channel, along with the settings object MusicVolume
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        settingsData.MusicVolume = volume;
    }

    /// <summary>
    /// Sets the effects volume channel, along with the settings object EffectsVolume
    /// </summary>
    /// <param name="volume"></param>
    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("EffectsVolume", volume);
        settingsData.EffectsVolume = volume;
    }

    /// <summary>
    /// Toggles fullscreen option via UI toggle
    /// </summary>
    public void SetFullscreen()
    {
        isFullscreen = !isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    /// <summary>
    /// Sets resolution based on UI dropdown index selection
    /// </summary>
    /// <param name="resolutionIndex"></param>
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    /// <summary>
    /// Resets values to their defaults, defined in SettingsData
    /// </summary>
    public void SetDefaults()
    {
        settingsData.ResetToDefaults();

        SetSlidersToSettingsData();
    }
}
