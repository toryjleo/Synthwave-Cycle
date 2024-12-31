using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RamCarAi : VehicleAi
{

    public override void ManualUpdate(ArrayList enemies, Vector3 wanderDirection, float fixedDeltaTime)
    {
        base.ManualUpdate(enemies, wanderDirection, fixedDeltaTime);
        if (stateController.isAttacking)
        {
            Attack();
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
        }
        base.OnCollisionEnter(collision);
    }
}