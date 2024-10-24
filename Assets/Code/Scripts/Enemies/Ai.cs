using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;

public delegate void NotifyDeath();  // delegate
public delegate void NotifyRespawn();

public abstract class Ai : Poolable
{
    public abstract Enemy GetEnemyType();

    #region Variables for Setup.

    protected AIState.StateController stateController = null;
    public PlayerHealth playerHealth;
    public GameObject target;
    public Rigidbody rb;
    public Gun myGun;
    public bool canAim = false;
    public Health hp;

    public float StartingHP;

    internal float maxSpeed;
    public float maxForce;
    public float score;
    public float dlScore;
    protected float attackRange = 15;
    public float minimumRange;
    internal float TIME_BY_TARGET_TO_ATTACK;
    internal float timeByTarget = 0;

    public bool inWorld = false;

    //Each enemy's speed is relative to a player's gear
    [SerializeField] public int movementGroup;
    //Top speed is determined as a percentage of the gear's max speed
    [SerializeField] public float gearModifier;

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
            CountDownTimeByTarget(Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            stateController.HandleTrigger(AIState.StateTrigger.InRange);
        }

        if (stateController.isInRange || stateController.isAttacking)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > attackRange)
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
        InitStateController();

        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        if (pm != null)
        {
            switch (movementGroup)
            {
                case 1:
                    maxSpeed = pm.TopSpeedMovementGroup1 * gearModifier;
                    break;
                case 2:
                    maxSpeed = pm.TopSpeedMovementGroup2 * gearModifier;
                    break;
                case 3:
                    maxSpeed = pm.TopSpeedMovementGroup3 * gearModifier;
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

        TIME_BY_TARGET_TO_ATTACK = 2.0f;

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
        // TODO: Subtract deltaTime if out of range, rather than set it to 0
        // Handle attack timing
        timeByTarget += deltaTime;
        if (timeByTarget > TIME_BY_TARGET_TO_ATTACK)
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

        if (myGun != null)
        {
            myGun.StopAllCoroutines();
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
    public void HandleInRangeEnter()
    {
        timeByTarget = 0;
    }

    public void HandleAttackingEnter()
    {
        if (canAim)
        {
            Aim(target.transform.position);
        }
        Attack();

        stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
    }

    public virtual void HandleInPoolExit()
    {
        hp.Init(StartingHP);
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
        float desiredChase = 1;

        // this logic creates the vector between where the entity is and where it wants to be
        Vector3 desiredVec = target - transform.position;
        // this creates a magnitude of the desired vector. This is the distance between the points
        float dMag = desiredVec.magnitude;
        // dMag is the distance between the two objects, by subtracting this, I make it so the object doesn't desire to move as far.
        dMag -= minimumRange;

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
    public void Wander(Vector3 wanderDirection) //cause the character to wander
    {
        Vector3 forward = rb.transform.forward; //The normalized vector of which direction the RB is facing
        forward += wanderDirection; //adds a small offset to the forward vector.

        transform.LookAt(forward + transform.position); //TODO make this look way nicer

        Vector3 steer = forward - rb.velocity; //Subtract Velocity so we are not constantly adding to the velocity of the Entity
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
        float separateForce = 1.1f;
        float maxDistanceToSeparate = 100;

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
                if (steer.magnitude > maxForce)
                {
                    steer.Normalize();
                    steer *= maxForce;
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
        float groupForce = 1.2f;
        float maxDistanceToGroup = 100;

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
                if (steer.magnitude > maxForce)
                {
                    steer.Normalize();
                    steer *= maxForce;
                }

                ApplyForce(steer);
            }
        }
    }
    #endregion

    #region Getters & Setters
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
