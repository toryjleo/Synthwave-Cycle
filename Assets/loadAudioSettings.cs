using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadAudioSettings : MonoBehaviour
{
    private const string MASTER_VOLUME = "masterVolume";
    // Start is called before the first frame update
    void Start()
    {

        if (!PlayerPrefs.HasKey(MASTER_VOLUME))
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME, 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    private void Load()
    {
        AudioListener.volume = PlayerPrefs.GetFloat(MASTER_VOLUME);
    }

    /*
    private void Save()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME, _volumeSlider.value);
    }
    */
}
