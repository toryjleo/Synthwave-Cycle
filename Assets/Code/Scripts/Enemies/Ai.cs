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
    public Health health;
    protected AiStats stats;

    internal float maxSpeed;
    internal float timeByTarget = 0;

    public bool inWorld = false;
    protected bool addDlScore = true;

    public event NotifyDeath DeadVisualsEvent;
    public event NotifyRespawn RespawnEvent;
    #endregion

    // Update is called once per frame
    public virtual void ManualUpdate(ArrayList enemies, Vector3 wanderDirection, float fixedDeltaTime)
    {
        if (stateController.isWandering)
        {
            Wander(wanderDirection, fixedDeltaTime);
        }
        else if (stateController.isFollowing)
        {
            Move(target.transform.position, enemies, fixedDeltaTime);

            if (Vector3.Distance(transform.position, target.transform.position) <= stats.AttackRange)
            {
                stateController.HandleTrigger(AIState.StateTrigger.InRange);
            }
        }
        else if (stateController.isInRange)
        {
            Move(target.transform.position, enemies, fixedDeltaTime);
            CountDownTimeByTarget(fixedDeltaTime);

            IsOutOfRange();
        }
        else if (stateController.isAttacking)
        {
            IsOutOfRange();
        }

        // Out of range subtract time from attack
        if (!stateController.isInRange && timeByTarget > 0)
        {
            CountDownTimeByTarget(-fixedDeltaTime);
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
        stateController.attacking.notifyListenersEnter += HandleAttackingEnter;
        stateController.attacking.notifyListenersExit += HandleAttackingExit;
        stateController.inPool.notifyListenersExit += HandleInPoolExit;
        stateController.wandering.notifyListenersEnter += HandleWanderingEnter;
        stateController.dead.notifyListenersEnter += HandleDeathEnter;
        stateController.inRange.notifyListenersEnter += HandleInRangeEnter;
        stateController.inRange.notifyListenersExit += HandleInRangeExit;
        GameStateController.playerDead.notifyListenersEnter += HandlePlayerDeadEnter;
        Despawn += HandleDespawned;
        health.deadEvent += HpAtZero;
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
    public virtual void HandleDeathEnter()
    {
        //Notify all listeners that this AI has died
        DeadVisualsEvent?.Invoke();

        if (myGuns != null && myGuns.Length > 0)
        {
            foreach (Gun gun in myGuns)
            {
                gun.StopAllCoroutines();
            }
        }

        if (DangerLevel.Instance && addDlScore)
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

    /// <summary>
    /// Checks to see if enemy falls out of range
    /// </summary>
    public void IsOutOfRange()
    {
        if (Vector3.Distance(transform.position, target.transform.position) > stats.AttackRange)
        {
            stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
        }
    }

    #region EventHandlers

    public virtual void HandleInRangeEnter()
    {

    }

    public virtual void HandleInRangeExit()
    {

    }

    public virtual void HandleAttackingEnter()
    {
        if (stats.CanAim)
        {
            Aim(target.transform.position);
        }
        Attack();

        stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
    }

    public virtual void HandleAttackingExit()
    {

    }

    public virtual void HandleInPoolExit()
    {
        health.Init(stats.Health);
        RespawnEvent?.Invoke();
        rb.detectCollisions = true;

        inWorld = true;
    }

    public void HandleWanderingEnter()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null && playerHealth.HitPoints > 0)
        {
            SetTarget(playerHealth.gameObject);
        }
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

    public void HpAtZero()
    {
        Debug.Log("AI is DEAD!!");
        stateController.HandleTrigger(AIState.StateTrigger.AiKilled);
    }
    #endregion

    #region MOVEMENT
    /// <summary>
    /// This method works for ranged Enemies that do not get into direct melee range with the target
    /// </summary>
    /// <param name="target"> Vector to target </param>
    public virtual void Move(Vector3 target, ArrayList enemyList, float fixedDeltaTime)
    {
        Chase(target, fixedDeltaTime);
        Separate(enemyList, fixedDeltaTime);
        Group(enemyList, fixedDeltaTime);
    }

    public abstract void Chase(Vector3 target, float fixedDeltaTime);

    public abstract void Wander(Vector3 wanderDirection, float fixedDeltaTime);

    public abstract void Separate(ArrayList pool, float fixedDeltaTime);

    public abstract void Group(ArrayList pool, float fixedDeltaTime);
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
            addDlScore = true;
        }
    }
}
