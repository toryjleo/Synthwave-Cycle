using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Logic for pausing and unpausing the menu/game
/// </summary>
public class InGameUI : MonoBehaviour
{
    // Start is called before the first frame update
    private static bool GameIsPaused = false;

    [SerializeField] private GameObject pauseMenuUI;

    [SerializeField] private Image restartUI;

    [SerializeField] private Button ResumeButton;


    private void Awake()
    {
        GameStateController.notifyListenersGameStateHasChanged += HandleGameStateUpdate;

        restartUI.enabled = false;
        if (ResumeButton) { ResumeButton.onClick.AddListener( () => { Resume(); } ); }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    void OnDestroy() 
    {
        GameStateController.notifyListenersGameStateHasChanged -= HandleGameStateUpdate;
    }

    /// <summary>
    /// Removes the pause menu
    /// </summary>
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    /// <summary>
    /// Pulls up the pause menu
    /// </summary>
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
    }

    private void HandleGameStateUpdate(GameState previousState, GameState newState)
    {
        Debug.Log("Handled gamestate update: " + previousState + " to " + newState);
    }
}
