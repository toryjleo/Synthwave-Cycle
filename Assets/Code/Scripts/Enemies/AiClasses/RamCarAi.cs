using System;
using System.Collections;
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

    public override void HandleAttackingEnter()
    {
        Attack();

        //TODO: Move followAgain to where we need to follow again
        stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
    }

    public override void Attack()
    {
        // TODO: Set up state for attacking
        Debug.Log("Ramcar attacking!!!");
        //RamCar just drives directly into the player (if targeted)
        if (target != null)
        {
            //movementTargetPosition.transform.position = target.transform.position;
            movementTargetPosition.transform.position = (Vector3.Normalize(this.transform.position - target.transform.position) + target.transform.position) * 2;
            //vehicleController.target = movementTargetPosition.transform;
            SetTarget(movementTargetPosition);
            //rb.AddForce(Vector3.Normalize(target.transform.position - this.transform.position) * 1000f * Time.fixedDeltaTime);
        }
    }

    public override void UpdateMovementLocation() // TODO: Use this function to follow player (?)
    {
        if (target != null)
        {
            //have we hovered by the player long enough to attack?
            movementTargetPosition.transform.position = stats.ChaseRange * Vector3.Normalize(this.transform.position - target.transform.position) + target.transform.position;
            vehicleController.target = movementTargetPosition.transform;
        }
    }

    public override void Chase(Vector3 target, float fixedDeltaTime)
    {
        UpdateMovementLocation();
    }

    public override void Wander(Vector3 wanderDirection, float fixedDeltaTime)
    {
        // throw new NotImplementedException();
    }

    public override void Separate(ArrayList pool, float fixedDeltaTime)
    {
        // throw new NotImplementedException();
    }

    public override void Group(ArrayList pool, float fixedDeltaTime)
    {
        // throw new NotImplementedException();
    }
}