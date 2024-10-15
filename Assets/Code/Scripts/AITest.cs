using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITest : MonoBehaviour
{
    private AIState.StateController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = new AIState.StateController(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            controller.HandleTrigger(AIState.StateTrigger.Spawning);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controller.HandleTrigger(AIState.StateTrigger.AiKilled);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            controller.HandleTrigger(AIState.StateTrigger.ArrivedAtLocation);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            controller.HandleTrigger(AIState.StateTrigger.HasTarget);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            controller.HandleTrigger(AIState.StateTrigger.InRange);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            controller.HandleTrigger(AIState.StateTrigger.OutOfRange);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            controller.HandleTrigger(AIState.StateTrigger.CountownToAttackComplete);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            controller.HandleTrigger(AIState.StateTrigger.FollowAgain);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            controller.HandleTrigger(AIState.StateTrigger.Despawned);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            controller.HandleTrigger(AIState.StateTrigger.TargetRemoved);
        }
    }
}
