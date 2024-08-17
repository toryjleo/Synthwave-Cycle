using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SBomberAi : VehicleAi
{
    //How far the S-bomber will stay before deciding to dash into the player
    static float TRAIL_DISTANCE = 15f;
    public override void Attack()
    {

    }

    //S-Bomber hovers near player building confidence before charging and exploding
    public override void UpdateMovementLocation()
    {
        //have we hovered by the player long enough to attack?
        if (timeByTarget >= TIME_BY_TARGET_TO_ATTACK)
        {
            movementTarget.position = target.transform.position;
        }
        else
        {
            movementTarget.position = TRAIL_DISTANCE * Vector3.Normalize(this.transform.position - target.transform.position) + target.transform.position;
        }
        vehicleController.target = movementTarget;
    }
}
