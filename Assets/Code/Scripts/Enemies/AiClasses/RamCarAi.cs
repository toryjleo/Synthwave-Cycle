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
        stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
        Attack();
    }

    public override void Attack()
    {
        Debug.Log("Ramcar attacking!!!");
        //RamCar just drives directly into the player (if targeted)
        // TODO: avoid steering too far off the path
        // TODO: verify separate works when not attacking
        // movementTargetPosition.transform.position = playerHealth.gameObject.transform.position;
        SetTarget(playerHealth.gameObject);
    }

    public override void UpdateMovementLocation() // TODO: Use this function to follow player (?)
    {
        if (target != null)
        {
            movementTargetPosition.transform.position = GetChaseLocation();
            vehicleController.target = movementTargetPosition.transform;
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //TODO: Move followAgain to where we need to follow again
            stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            //TODO: Do something when two cars get stuck together
            // movementTargetPosition.transform.position = GetChaseLocation();
            stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
        }
        base.OnCollisionEnter(collision);
    }

    private Vector3 GetChaseLocation()
    {
        return stats.ChaseRange * Vector3.Normalize(this.transform.position - target.transform.position) + target.transform.position;
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
        float separateForce = stats.MaxSeparateForce;
        float maxDistanceToSeparate = stats.SeparateRange;

        //the vector that will be used to calculate flee behavior if a too close interaction happens
        Vector3 sum = new Vector3();
        //this counts how many TOO CLOSE interactions an entity has, if it has more than one
        int count = 0;

        foreach (Ai ai in pool)
        {
            float distance = Vector3.Distance(ai.transform.position, transform.position);

            if (ai.transform.position != transform.position && distance < maxDistanceToSeparate)
            {
                // creates vec between two objects
                Vector3 diff = transform.position - ai.transform.position;
                diff.Normalize();
                // sum is the flee direction added together
                sum += diff;
                count++;
            }
        }

        if (count > 0)
        {
            float targetDistanceSquared = Vector3.SqrMagnitude(movementTargetPosition.transform.position - transform.position);
            sum.Normalize();

            Vector3 steer = sum * (targetDistanceSquared / (separateForce * separateForce));

            movementTargetPosition.transform.position = steer;
        }
    }

    public override void Group(ArrayList pool, float fixedDeltaTime)
    {
        // throw new NotImplementedException();
    }
}