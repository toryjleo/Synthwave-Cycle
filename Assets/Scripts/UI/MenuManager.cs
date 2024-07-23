using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the menus in our main gameplay scene
/// </summary>
public class MenuManager : MonoBehaviour
{
  [SerializeField] public GameObject loadingScreen;
  [SerializeField] public GameObject loadingFinishedScreen;
  [SerializeField] public GameObject pauseScreen;
  [SerializeField] public GameObject gameplayUI;
  [SerializeField] public GameObject playerDeadScreen;
  [SerializeField] public GameObject levelCompleteScreen;

  [SerializeField] public TextMeshProUGUI songDisplayText;
  private LevelManager levelManager;

  void Start()
  {
    levelManager = FindObjectOfType<LevelManager>();
    songDisplayText.text = "Current Song: " + levelManager.SongName + " by Shadow Mage";

    //Loading
    GameStateController.loading.notifyListenersEnter += HandleLoadingEnter;
    GameStateController.loading.notifyListenersExit += HandleLoadingExit;

    //Loading complete/GameStartPaused
    GameStateController.gamesStartPaused.notifyListenersEnter += HandleLoadingFinishedEnter;
    GameStateController.gamesStartPaused.notifyListenersExit += HandleLoadingFinishedExit;

    //Playing
    GameStateController.playing.notifyListenersEnter += HandlePlayingEnter;
    GameStateController.playing.notifyListenersExit += StopTimeScale;

    //Paused
    GameStateController.gamePlayPaused.notifyListenersEnter += HandlePausingFinishedEnter;
    GameStateController.gamePlayPaused.notifyListenersExit += HandlePausingFinishedExit;

    //Player dead
    GameStateController.playerDead.notifyListenersEnter += HandlePlayerDeadEnter;
    GameStateController.playerDead.notifyListenersExit += HandlePlayerDeadExit;

    //Level Complete
    GameStateController.levelComplete.notifyListenersEnter += HandleLevelCompleteEnter;
    GameStateController.levelComplete.notifyListenersExit += HandleLevelCompleteExit;

    //Reset
    GameStateController.resetting.notifyListenersEnter += HandleLoadingEnter;
    GameStateController.resetting.notifyListenersExit += HandleLoadingExit;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      GameStateController.HandleTrigger(StateTrigger.StartGame);
    }

    if (Input.GetKeyDown(KeyCode.R))
    {
      GameStateController.HandleTrigger(StateTrigger.Reset);
    }
  }

  private void StartTimeScale()
  {
    Time.timeScale = 1;
  }

  private void StopTimeScale()
  {
    Time.timeScale = 0;
  }

  private void HandleLoadingEnter()
  {
    loadingScreen.SetActive(true);
  }

  private void HandleLoadingExit()
  {
    loadingScreen.SetActive(false);
  }

  private void HandleLoadingFinishedEnter()
  {
    loadingFinishedScreen.SetActive(true);
    StopTimeScale();
  }

  private void HandleLoadingFinishedExit()
  {
    loadingFinishedScreen.SetActive(false);
    StartTimeScale();
  }

  private void HandlePausingFinishedEnter()
  {
    pauseScreen.SetActive(true);
  }

  private void HandlePausingFinishedExit()
  {
    pauseScreen.SetActive(false);
  }

  private void HandlePlayingEnter()
  {
    gameplayUI.SetActive(true);
    StartTimeScale();
  }

  private void HandlePlayerDeadEnter()
  {
    playerDeadScreen.SetActive(true);
    gameplayUI.SetActive(false);
    StartTimeScale();
  }

  private void HandlePlayerDeadExit()
  {
    playerDeadScreen.SetActive(false);
    StopTimeScale();
  }

  private void HandleLevelCompleteEnter()
  {
    gameplayUI.SetActive(false);
    levelCompleteScreen.SetActive(true);
  }

  private void HandleLevelCompleteExit()
  {
    levelCompleteScreen.SetActive(false);
  }
}
