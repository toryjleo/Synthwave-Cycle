using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SBomberAi : VehicleAi
{
    //How far the S-bomber will stay before deciding to dash into the player
    static float TRAIL_DISTANCE = 2f;

    [SerializeField]
    float ExplosionDamage = 50f;

    [SerializeField]
    GameObject AttackTelegraph; // this object appears and disappears when the target is preparing to attack

    public override void Initialize()
    {

    }

    public override void HandleInRangeEnter()
    {
        // if (timeByTarget >= stats.TimeToAttack - 3) // Telegraph attack 3 seconds before charge
        // {
        //     AttackTelegraph.SetActive(true);
        // }
        // else
        // {
        //     AttackTelegraph.SetActive(false);
        // }
        AttackTelegraph.SetActive(true);
    }

    public override void HandleInRangeExit()
    {
        AttackTelegraph.SetActive(false);
    }

    public override void HandleAttackingEnter()
    {
        // vehicleController.target = target.transform;
        Attack();
    }

    public override void Attack()
    {
        Debug.Log("S Bomber attacking!!!");
        //Dive bomb the player
        // vehicleController.enabled = false;
        // rb.AddForce(Vector3.Normalize(target.transform.position - this.transform.position) * 1000f * Time.fixedDeltaTime);

        SetTarget(playerHealth.gameObject);
    }

    //S-Bomber hovers near player building confidence before charging and exploding
    public override void UpdateMovementLocation()
    {
        if (target != null)
        {
            movementTargetPosition.transform.position = GetChaseLocation();
            vehicleController.target = movementTargetPosition.transform;
        }
    }

    private Vector3 GetChaseLocation()
    {
        return stats.ChaseRange * Vector3.Normalize(this.transform.position - target.transform.position) + target.transform.position;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // if (collision != null && collision.collider.gameObject == target)
        // {
        //     target.GetComponent<PlayerHealth>().TakeDamage(ExplosionDamage);
        //     this.Die();
        // }

        if (collision.gameObject.tag == "Player")
        {
            target.GetComponent<PlayerHealth>().TakeDamage(ExplosionDamage);
            this.Die();
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
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
