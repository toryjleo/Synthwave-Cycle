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
    public override Enemy GetEnemyType()
    {
        return Enemy.RamCar;
    }

    public override void UpdateMovementLocation()
    {
        //RamCar just drives directly into the player (if targeted)
        if (target != null)
        {
            movementTargetPosition.transform.position = target.transform.position;
            //movementTargetPosition.transform.position = (Vector3.Normalize(this.transform.position - target.transform.position) + target.transform.position) * 2;
            vehicleController.target = movementTargetPosition.transform;
        }
    }
}