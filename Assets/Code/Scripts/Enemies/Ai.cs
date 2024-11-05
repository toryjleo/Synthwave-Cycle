using System.Collections;
using System.Collections.Generic;
using EditorObject;
using Generic;
using UnityEngine;

public delegate void NotifyDeath();
public delegate void NotifyRespawn();

public abstract class Ai : Poolable
{
    #region Variables for Setup.
    protected AIState.StateController stateController = null;
    public PlayerHealth playerHealth;
    public GameObject target;
    public Rigidbody rb;
    public Gun[] myGuns;
    public Health hp;
    protected AiStats stats;

    internal float maxSpeed;
    internal float timeByTarget = 0;

    public bool inWorld = false;

    public event NotifyDeath DeadEvent;
    public event NotifyRespawn RespawnEvent;
    #endregion

    // Update is called once per frame
    public virtual void ManualUpdate(ArrayList enemies, Vector3 wanderDirection)
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null && playerHealth.HitPoints > 0)
        {
            SetTarget(playerHealth.gameObject);
        }

        if (hp.HitPoints <= 0)
        {
            stateController.HandleTrigger(AIState.StateTrigger.AiKilled);
        }

        if (stateController.isWandering)
        {
            Wander(wanderDirection);
        }
        else if (stateController.isFollowing)
        {
            Move(target.transform.position, enemies);
        }
        else if (stateController.isInRange)
        {
            Move(target.transform.position, enemies);
            CountDownTimeByTarget(Time.fixedDeltaTime);
        }

        // Out of range subtract time from attack
        if (!stateController.isInRange && timeByTarget > 0)
        {
            CountDownTimeByTarget(-Time.fixedDeltaTime);
        }

        if (Vector3.Distance(transform.position, target.transform.position) <= stats.AttackRange)
        {
            stateController.HandleTrigger(AIState.StateTrigger.InRange);
        }

        if (stateController.isInRange || stateController.isAttacking)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > stats.AttackRange)
            {
                stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
            }
        }
    }

    void Start()
    {

    }

    public override void Init(IPoolableInstantiateData stats)
    {
        this.stats = (AiStats)stats;
        if (!this.stats)
        {
            Debug.LogError("AI Stats are not initialized correctly!");
        }
        InitStateController();

        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        if (pm != null)
        {
            switch (this.stats.MovementGroup)
            {
                case 1:
                    maxSpeed = pm.TopSpeedMovementGroup1 * this.stats.GearModifier;
                    break;
                case 2:
                    maxSpeed = pm.TopSpeedMovementGroup2 * this.stats.GearModifier;
                    break;
                case 3:
                    maxSpeed = pm.TopSpeedMovementGroup3 * this.stats.GearModifier;
                    break;
            }
        }
    }

    /// <summary>
    /// Initializes the stateController for the Ai unit, and sets up event handlers
    /// </summary>
    public virtual void InitStateController()
    {
        stateController = new AIState.StateController(true);
        stateController.inRange.notifyListenersEnter += HandleInRangeEnter;
        stateController.attacking.notifyListenersEnter += HandleAttackingEnter;
        stateController.inPool.notifyListenersExit += HandleInPoolExit;
        stateController.dead.notifyListenersEnter += Die;
        GameStateController.playerDead.notifyListenersEnter += HandlePlayerDeadEnter;
        Despawn += HandleDespawned;
    }

    /// <summary>
    /// Spawns in an Ai unit, facing the player and at the desired position.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="playerLocation"></param>
    public void Spawn(Vector3 position, Vector3 playerLocation)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(playerLocation - position);
        gameObject.SetActive(true);

        stateController.HandleTrigger(AIState.StateTrigger.Spawning);
        inWorld = true;
    }

    /// <summary>
    /// If the Ai is in range of its target, start counting down. Triggers a
    /// CountdownToAttackComplete stateTrigger when finished.
    /// </summary>
    /// <param name="deltaTime"></param>
    public void CountDownTimeByTarget(float deltaTime)
    {
        // Handle attack timing
        timeByTarget += deltaTime;
        if (timeByTarget > stats.TimeToAttack)
        {
            stateController.HandleTrigger(AIState.StateTrigger.CountdownToAttackComplete);
        }
    }

    /// <summary>
    /// This method plays a death animation and the deactivates the enemy
    /// </summary>
    public virtual void Die()
    {
        //Notify all listeners that this AI has died
        DeadEvent?.Invoke();

        if (myGuns != null && myGuns.Length > 0)
        {
            foreach (Gun gun in myGuns)
            {
                gun.StopAllCoroutines();
            }
        }

        if (DangerLevel.Instance)
        {
            DangerLevel.Instance.IncreaseDangerLevel(stats.DlScore);
        }
    }

    /// <summary>
    /// Causes the AI to lookAt the desired Vector3 target
    /// </summary>
    /// <param name="aimAt"></param>
    public virtual void Aim(Vector3 aimAt)
    {
        transform.LookAt(aimAt);
    }

    /// <summary>
    /// This method is called when the entity wants to attack. Checks if it has a gun
    /// </summary>
    public abstract void Attack();

    #region EventHandlers
    public virtual void HandleInRangeEnter()
    {
        // timeByTarget = 0;
    }

    public void HandleAttackingEnter()
    {
        if (stats.CanAim)
        {
            Aim(target.transform.position);
        }
        Attack();

        stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
    }

    public virtual void HandleInPoolExit()
    {
        hp.Init(stats.Health);
        RespawnEvent?.Invoke();
        rb.detectCollisions = true;

        inWorld = true;
    }

    public void HandlePlayerDeadEnter()
    {
        stateController.HandleTrigger(AIState.StateTrigger.TargetRemoved);
    }

    public void HandleDespawned(SelfDespawn entity)
    {
        gameObject.SetActive(false);
        stateController.HandleTrigger(AIState.StateTrigger.Despawned);
        inWorld = false;
    }
    #endregion

    // TODO: Make a new class (NOT Monobehavior) that holds all the movement functions
    #region MOVEMENT
    /// <summary>
    /// This method works for ranged Enemies that do not get into direct melee range with the target
    /// </summary>
    /// <param name="target"> Vector to target </param>
    public virtual void Move(Vector3 target, ArrayList enemyList) //This can be used for Enemies that stay at range and don't run into melee.
    {
        Chase(target);
        Separate(enemyList);
        Group(enemyList);
    }

    public void Chase(Vector3 target)
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
        ApplyForce(steer);
    }

    /// <summary>
    /// This method is used for when an AI has no target and will move around in a Boid fashion
    /// </summary>
    public void Wander(Vector3 wanderDirection)
    {
        // The normalized vector of which direction the RB is facing
        Vector3 forward = rb.transform.forward;
        // Adds a small offset to the forward vector.
        forward += wanderDirection;

        transform.LookAt(forward + transform.position); //TODO make this look way nicer

        // Subtract Velocity so we are not constantly adding to the velocity of the Entity
        Vector3 steer = forward - rb.velocity;
        ApplyForce(steer);

    }

    /// <summary>
    /// Applies the desired force to the rigidbody
    /// </summary>
    /// <param name="force"></param>
    public void ApplyForce(Vector3 force)
    {
        rb.AddForce(force);
    }

    /// <summary>
    /// This function will edit the steer of an AI so it moves away from nearby other AI
    /// </summary>
    /// <param name="pool">Pool is the grouping of all of the AI controlled entities in the boid that need to be separated from one another</param>
    public void Separate(ArrayList pool)
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

                ApplyForce(steer);
            }

        }
    }

    /// <summary>
    /// Used to group enemies together if they get too separated
    /// </summary>
    /// <param name="pool"></param>
    public void Group(ArrayList pool)
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

                ApplyForce(steer);
            }
        }
    }
    #endregion

    #region Getters & Setters
    public Enemy GetEnemyType()
    {
        return stats.EnemyType;
    }

    /// <summary>
    /// This method sets the target of the entity
    /// </summary>
    /// <param name="targ"></param>
    public virtual void SetTarget(GameObject targ)//sets the target of the entity and equips the gun
    {
        target = targ;
        if (!targ)
        {
            stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
        }
        else
        {
            stateController.HandleTrigger(AIState.StateTrigger.HasTarget);
        }
    }
    #endregion

    public override void Reset()
    {
        Debug.Log("RESETTING THIS AI GUY");

        if (inWorld)
        {
            Debug.Log("DESPAWNING AI");
            // OnDespawn();
            gameObject.SetActive(false);
            stateController.HandleTrigger(AIState.StateTrigger.Despawned);
            inWorld = false;
        }
    }
}
