using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeScript : MonoBehaviour
{
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TextMeshProUGUI _sliderText;
    private const string MASTER_VOLUME = "masterVolume";

    void Start()
    {
        _volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(); });

        Load();

    }

    private void ChangeVolume()
    {
        AudioListener.volume = _volumeSlider.value;
        _sliderText.text = _volumeSlider.value.ToString("0.00");
        Save();
    }

    private void Load()
    {
        _volumeSlider.value = PlayerPrefs.GetFloat(MASTER_VOLUME);
    }

    private void Save()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME, _volumeSlider.value);
    }
}
