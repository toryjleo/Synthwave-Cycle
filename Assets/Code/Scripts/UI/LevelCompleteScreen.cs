using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Functions handled by the level complete screen, part of the GameplayUI
/// </summary>
public class LevelCompleteScreen : MonoBehaviour
{
    /// <summary>
    /// Sets the time to resume and loads the MainMenu scene
    /// </summary>
    public void MainMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(LoadYourAsyncScene("MainMenu"));
    }

    /// <summary>
    /// Tells the GameStateController to reset the game
    /// </summary>
    public void PlayAgain()
    {
        GameStateController.HandleTrigger(StateTrigger.Reset);
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
