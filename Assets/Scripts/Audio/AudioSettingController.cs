using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettingController : MonoBehaviour
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

    public float GetMasterVolume()
    {
        return AudioListener.volume;
    }

    public void SetMasterVolume(float newVolume)
    {
        // set audio listener
        AudioListener.volume = newVolume;
        // update player prefs
        PlayerPrefs.SetFloat(MASTER_VOLUME, newVolume);
    }

}
