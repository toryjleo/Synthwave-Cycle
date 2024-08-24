using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RamCarAi : VehicleAi
{

    public override void Attack()
    {

    }

    public override void UpdateMovementLocation()
    {
        //RamCar just drives directly into the player (if targeted)
        if (target != null)
        {
            movementTarget = target.transform;
            vehicleController.target = movementTarget;
        }
    }
}