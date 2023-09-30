using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main Menu code
/// </summary>
public class ChangeScene : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button QuitButton;

    private void Start()
    {
        PlayButton.onClick.AddListener( () => { MoveToScene("TrueScene"); } );
        QuitButton.onClick.AddListener( () => { QuitGame(); } );
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
