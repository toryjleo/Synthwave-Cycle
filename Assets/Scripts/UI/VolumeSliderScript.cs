using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderScript : MonoBehaviour
{
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TextMeshProUGUI _sliderText;
    [SerializeField] private AudioSettingController _audioSettingController;

    void Start()
    {
        _volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(); });

        Load();
        
    }

    private void ChangeVolume()
    {
        _sliderText.text = _volumeSlider.value.ToString("0.00");
        Save();
    }

    private void Load()
    {
        _volumeSlider.value = _audioSettingController.GetMasterVolume();
    }

    private void Save()
    {
        _audioSettingController.SetMasterVolume(_volumeSlider.value);
    }
}
