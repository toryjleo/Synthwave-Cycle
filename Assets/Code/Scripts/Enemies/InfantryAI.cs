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
        if (myGuns != null && myGuns[0].CanShootAgain())
        {
            this.myGuns[0].PrimaryFire(target.transform.position);
            animationStateController.AimWhileWalking(true);
        }
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

        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        animationStateController = GetComponent<CyborgAnimationStateController>();
        hp.Init(aiStats.Health);

        myGuns[0].Init();

        // Error checking
        if (animationStateController == null)
        {
            Debug.LogError("This object needs a CyborgAnimationStateController component");
        }
        if (rb == null)
        {
            Debug.LogError("This object needs a rigidBody component");
        }
        if (hp == null)
        {
            Debug.LogError("This object needs a health component");
        }

        base.Init(stats);
    }

    /// <summary>
    /// This Metod is used to set the animation speed without causing errors or Ai that do not have an animation state controller.
    /// </summary>
    /// <param name="animationSpeed"></param>
    public virtual void SetAnimationSpeed(float animationSpeed)
    {
        if (animationStateController != null)
        {
            animationStateController.SetSpeed(animationSpeed);
        }
    }

    public override void Die()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;

        animationStateController.TriggerDeathA();

        rb.detectCollisions = false;
        animationStateController.SetAlive(false);

        base.Die();
    }

    public override void Reset()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        animationStateController.SetAlive(true);
        base.Reset();
    }

    public override void Initialize()
    {

    }

    #region MOVEMENT
    /// <summary>
    /// Applies the desired force to the rigidbody
    /// </summary>
    /// <param name="force"></param>
    public void ApplyForce(Vector3 force, float fixedDeltaTime)
    {
        rb.AddForce(force * fixedDeltaTime);
    }

    public override void Chase(Vector3 target, float fixedDeltaTime)
    {
        float desiredChase = stats.MaxChaseForce;

        // this logic creates the vector between where the entity is and where it wants to be
        Vector3 desiredVec = target - transform.position;
        // this creates a magnitude of the desired vector. This is the distance between the points
        float dMag = desiredVec.magnitude;
        // dMag is the distance between the two objects, by subtracting this, I make it so the object doesn't desire to move as far.
        dMag -= stats.ChaseRange;

        // one the distance is measured this vector can now be used to actually generate movement, 
        // but that movement has to be constant or at least adaptable, which is what the next part does
        desiredVec.Normalize();
        transform.LookAt(target);

        //Currently Walking towards the target
        if (dMag < maxSpeed)
        {
            desiredVec *= dMag;
        }
        else
        {
            desiredVec *= maxSpeed;
        }

        // Subtract Velocity so we are not constantly adding to the velocity of the Entity
        Vector3 steer = (desiredVec - rb.velocity) * desiredChase;
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
                if (steer.magnitude > stats.MaxMovementForce)
                {
                    steer.Normalize();
                    steer *= stats.MaxMovementForce;
                }

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
                if (steer.magnitude > stats.MaxMovementForce)
                {
                    steer.Normalize();
                    steer *= stats.MaxMovementForce;
                }

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
