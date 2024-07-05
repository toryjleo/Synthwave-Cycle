using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
  [SerializeField] public GameObject loadingScreen;
  [SerializeField] public GameObject loadingFinishedScreen;

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
  }

  // Update is called once per frame
  void Update()
  {

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
}
