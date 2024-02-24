using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Menu code
/// </summary>
public class ChangeScene : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Button ReturnMainMenuButton;

    private bool returningToMainMenu;

    private void Start()
    {
        if (PlayButton)           { PlayButton.onClick.AddListener(() =>           { MoveToScene("TrueScene"); }); }
        if (ReturnMainMenuButton) { ReturnMainMenuButton.onClick.AddListener(() => { ReturnToMainMenu(); }); }
        if (QuitButton)           { QuitButton.onClick.AddListener(() =>           { QuitGame(); }); }

        returningToMainMenu = false;
    }

    public void MoveToScene(string sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    public void ReturnToMainMenu() 
    {
        MoveToScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    private IEnumerator LoadMainMenuAfterDelay(double delay)
    {
        yield return new WaitForSeconds((float)delay);
        Debug.Log("Loading Main Menu");
        ReturnToMainMenu();
    }

    public void TryReturnMainMenu(double delay)
    {
        if (!returningToMainMenu) 
        {
            returningToMainMenu = true;
            StartCoroutine(LoadMainMenuAfterDelay(delay));
        }
    }
}
