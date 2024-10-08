using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using EditorObject;
using TMPro;

/// <summary>
/// Handles the main menu scene functions
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private SettingsData settingsData;
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] private GameSave gameSave;
    [SerializeField] private TextMeshProUGUI startText;

    private LevelSelector selector;

    void Start()
    {
        SetMixerNumbers();
        SetStartButtonText();

        // Find level selector
        FindLevelSelector();
    }

    /// <summary>Will load the game via the start button.</summary>
    public void StartOnClick()
    {
        FindLevelSelector();
        selector.SetSelectedLevel(gameSave.levelSequence[0]);
        StartGame();
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void StartGame()
    {
        SetMixerNumbers();
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

    /// <summary>
    /// Sets the audio mixer channels to their corresponding settingsData fields
    /// </summary>
    private void SetMixerNumbers()
    {
        audioMixer.SetFloat("MainVolume", settingsData.MainVolume);
        audioMixer.SetFloat("MusicVolume", settingsData.MusicVolume);
        audioMixer.SetFloat("EffectsVolume", settingsData.EffectsVolume);
    }

    private void SetStartButtonText()
    {
        if (gameSave)
        {
            if (gameSave.MaxLevelProgess > 0)
            {
                startText.text = "Continue";
            }
            else
            {
                startText.text = "Start Game";
            }
        }
    }

    private void FindLevelSelector()
    {
        selector = FindObjectOfType<LevelSelector>();
        if (!selector)
        {
            Debug.LogError("No level selector in scene!");
        }
    }
}
