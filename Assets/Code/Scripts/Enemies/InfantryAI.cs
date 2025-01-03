using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EditorObject;
using Generic;
using UnityEngine;

/// <summary>
/// InfantryAI handles animations and gun-shooting for all humanoid enemy units
/// </summary>
public class InfantryAI : Ai
{
    public CyborgAnimationStateController animationStateController;

    public override void Attack()
    {
        myGuns[0].ExternalFire = true;
        animationStateController.AimWhileWalking(true);
    }

    public override void ManualUpdate(ArrayList enemies, Vector3 wanderDirection, float fixedDeltaTime)
    {
        //TODO: Set the target from the future enemy manager
        if (playerHealth != null && playerHealth.HitPoints > 0)
        {
            SetTarget(playerHealth.gameObject);
        }
        SetAnimationSpeed(rb.velocity.magnitude);
        base.ManualUpdate(enemies, wanderDirection, fixedDeltaTime);
    }

    public override void Init(IPoolableInstantiateData stats)
    {
        AiStats aiStats = stats as AiStats;
        if (!aiStats)
        {
            Debug.LogWarning("InfantryAi stats are not readable as TestAi!");
        }

        health = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        animationStateController = GetComponent<CyborgAnimationStateController>();
        health.Init(aiStats.Health);

        myGuns[0].Init(aiStats.GunStats);

        // Error checking
        if (animationStateController == null)
        {
            Debug.LogError("This object needs a CyborgAnimationStateController component");
        }
        if (rb == null)
        {
            Debug.LogError("This object needs a rigidBody component");
        }
        if (health == null)
        {
            Debug.LogError("This object needs a health component");
        }

        base.Init(stats);
    }

    /// <summary>
    /// This Method is used to set the animation speed without causing errors or Ai that do not have an animation state controller.
    /// </summary>
    /// <param name="animationSpeed"></param>
    public virtual void SetAnimationSpeed(float animationSpeed)
    {
        if (animationStateController != null)
        {
            animationStateController.SetSpeed(animationSpeed);
        }
    }

    public override void HandleInRangeEnter()
    {
        animationStateController.AimWhileWalking(true);
        base.HandleInRangeEnter();
    }

    public override void HandleInRangeExit()
    {
        animationStateController.AimWhileWalking(false);
        base.HandleInRangeExit();
    }

    public override void HandleDeathEnter()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;

        animationStateController.TriggerDeathA();

        rb.detectCollisions = false;
        animationStateController.SetAlive(false);

        base.HandleDeathEnter();
    }

    public override void Reset()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        animationStateController.SetAlive(true);
        base.Reset();
    }

    #region MOVEMENT
    /// <summary>
    /// Applies the desired force to the rigidbody
    /// </summary>
    /// <param name="force"></param>
    public void ApplyForce(Vector3 force, float fixedDeltaTime)
    {
        rb.AddForce(force * fixedDeltaTime);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }


    public override void Chase(Vector3 target, float fixedDeltaTime)
    {
        Vector3 toTarget = target - transform.position;
        float distanceFromUsToTarget = toTarget.magnitude;
        float distancefromUsToChasePoint = distanceFromUsToTarget - stats.ChaseRange;


        bool withinRange = distancefromUsToChasePoint < 0;
        Vector3 steer = Vector3.zero;
        if (withinRange)
        {
            // We are trying to slow down, not chase
            Vector3 velocity = rb.velocity;
            steer = -velocity * stats.ArrivalWeight;
        }
        else
        {

            Vector3 currentLocationToChasePoint = toTarget.normalized * distancefromUsToChasePoint;
            steer = currentLocationToChasePoint.normalized * stats.MaxChaseForce;
        }

        ApplyForce(steer, fixedDeltaTime);
    }

    /// <summary>
    /// This function will edit the steer of an AI so it moves away from nearby other AI
    /// </summary>
    /// <param name="pool">Pool is the grouping of all of the AI controlled entities in the boid that need to be separated from one another</param>
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

            if (count > 0)
            {
                sum /= count;
                sum.Normalize();
                sum *= maxSpeed;

                Vector3 steer = (sum - rb.velocity) * separateForce;

                ApplyForce(steer, fixedDeltaTime);
            }

        }
    }

    /// <summary>
    /// Used to group enemies together if they get too separated
    /// </summary>
    /// <param name="pool"></param>
    public override void Group(ArrayList pool, float fixedDeltaTime)
    {
        float groupForce = stats.MaxGroupingForce;
        float maxDistanceToGroup = stats.GroupingRange;

        //the vector that will be used to calculate flee behavior if a too far interaction happens
        Vector3 sum = new Vector3();
        //this counts how many TOO FAR interactions an entity has, if it has more than one
        int count = 0;

        foreach (Ai ai in pool)
        {
            float distance = Vector3.Distance(ai.transform.position, transform.position);

            if (ai.transform.position != transform.position && distance > maxDistanceToGroup)
            {
                // creates vec between two objects
                Vector3 diff = ai.transform.position - transform.position;
                diff.Normalize();
                // sum is the group direction added together
                sum += diff;
                count++;
            }

            if (count > 0)
            {
                sum /= count;
                sum.Normalize();
                sum *= maxSpeed;

                Vector3 steer = (sum - rb.velocity) * groupForce;

                ApplyForce(steer, fixedDeltaTime);
            }
        }
    }

    /// <summary>
    /// This method is used for when an AI has no target and will move around in a Boid fashion
    /// </summary>
    public override void Wander(Vector3 wanderDirection, float fixedDeltaTime)
    {
        // The normalized vector of which direction the RB is facing
        Vector3 forward = rb.transform.forward;
        // Adds a small offset to the forward vector.
        forward += wanderDirection;

        transform.LookAt(forward + transform.position);

        // Subtract Velocity so we are not constantly adding to the velocity of the Entity
        Vector3 steer = forward - rb.velocity;
        ApplyForce(steer, fixedDeltaTime);
    }
    #endregion
}
