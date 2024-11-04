using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RamCarAi : VehicleAi
{
    public override void Initialize()
    {

    }

    public override void Attack() // TODO: Use this function to attack (UpdateMovementLocation)
    {
        //RamCar just drives directly into the player (if targeted)
        if (target != null)
        {
            movementTargetPosition.transform.position = target.transform.position;
            //movementTargetPosition.transform.position = (Vector3.Normalize(this.transform.position - target.transform.position) + target.transform.position) * 2;
            vehicleController.target = movementTargetPosition.transform;
        }
    }

    public override void UpdateMovementLocation() // TODO: Use this function to follow player (?)
    {
        if (target != null)
        {
            //have we hovered by the player long enough to attack?
            movementTargetPosition.transform.position = stats.FollowRange * Vector3.Normalize(this.transform.position - target.transform.position) + target.transform.position;
            vehicleController.target = movementTargetPosition.transform;
        }
    }
}