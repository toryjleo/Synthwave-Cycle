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

    private void Start()
    {
        if (PlayButton)           { PlayButton.onClick.AddListener(() =>           { MoveToScene("TrueScene"); }); }
        if (ReturnMainMenuButton) { ReturnMainMenuButton.onClick.AddListener(() => { MoveToScene("MainMenu"); }); }
        if (QuitButton)           { QuitButton.onClick.AddListener(() =>           { QuitGame(); }); }
    }

    public void MoveToScene(string sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
