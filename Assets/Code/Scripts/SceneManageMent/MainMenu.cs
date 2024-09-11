using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using EditorObject;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SettingsData settingsData;
    [SerializeField] public AudioMixer audioMixer;

    void Start()
    {
        audioMixer.SetFloat("MainVolume", settingsData.MainVolume);
        audioMixer.SetFloat("MusicVolume", settingsData.MusicVolume);
        audioMixer.SetFloat("EffectsVolume", settingsData.EffectsVolume);
    }

    /// <summary>Will load the game.</summary>
    public void StartOnClick()
    {
        StartCoroutine(LoadYourAsyncScene("TrueScene"));
    }

    /// <summary>Will quit the application.</summary>
    public void QuitOnClick()
    {
        Application.Quit();
    }

    /// <summary>Loads the scene associated with the string asycronously.</summary>
    /// <param name="scene">The name of the scene to load.</param>
    IEnumerator LoadYourAsyncScene(string scene)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
