using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
  [SerializeField] public GameObject loadingScreen;
  [SerializeField] public GameObject loadingFinishedScreen;

  [SerializeField] public GameObject pauseScreen;

  [SerializeField] public GameObject gameplayUI;

  [SerializeField] public GameObject playerDeadScreen;

  void Start()
  {
    if (GameStateController.loading == null)
    {
      Debug.LogError("NO LOADING STATE");
    }
    GameStateController.loading.notifyListenersEnter += HandleLoadingEnter;
    GameStateController.loading.notifyListenersExit += HandleLoadingExit;

    GameStateController.gamesStartPaused.notifyListenersEnter += HandleLoadingFinishedEnter;
    GameStateController.gamesStartPaused.notifyListenersExit += HandleLoadingFinishedExit;

    GameStateController.playing.notifyListenersEnter += HandlePlayingEnter;
    GameStateController.playing.notifyListenersExit += StopTimeScale;

    GameStateController.gamePlayPaused.notifyListenersEnter += HandlePausingFinishedEnter;
    GameStateController.gamePlayPaused.notifyListenersExit += HandlePausingFinishedExit;

    GameStateController.playerDead.notifyListenersEnter += HandlePlayerDeadEnter;
    GameStateController.playerDead.notifyListenersExit += HandlePlayerDeadExit;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      GameStateController.HandleTrigger(StateTrigger.StartGame);
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
  }

  private void HandleLoadingFinishedExit()
  {
    loadingFinishedScreen.SetActive(false);
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
    StartTimeScale();
  }

  private void HandlePlayerDeadExit()
  {
    playerDeadScreen.SetActive(false);
    StopTimeScale();
  }
}
