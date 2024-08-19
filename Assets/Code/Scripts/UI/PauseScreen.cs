using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
  public void Resume()
  {
    GameStateController.HandleTrigger(StateTrigger.StartGame);
  }

  public void Restart()
  {
    GameStateController.HandleTrigger(StateTrigger.Reset);
  }
}
