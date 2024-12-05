using System.Collections;
using System.Collections.Generic;
using EditorObject;
using Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Class<c>VehicleAI</c> 
/// VehicleAI holds all the code that makes an enemy Vehicle different form other enemy types
public abstract class VehicleAi : Ai
{
    public ArcadeAiVehicleController vehicleController;

    //How much damage ramming deals
    [SerializeField] public float DamageMultiplier = 1.0f;

    [SerializeField] public GameObject itemDrop;

    [SerializeField] public GameObject movementTargetPosition;

    //This object appears and disappears when the target is preparing to attack
    [SerializeField] GameObject AttackTelegraph;

    public override void ManualUpdate(ArrayList enemies, Vector3 wanderDirection, float fixedDeltaTime)
    {
        base.ManualUpdate(enemies, wanderDirection, fixedDeltaTime);
    }

    public override void Init(IPoolableInstantiateData stats)
    {
        AiStats aiStats = stats as AiStats;
        if (!aiStats)
        {
            Debug.LogWarning("VehicleAi stats are not readable as TestAi!");
        }

        health = GetComponentInChildren<Health>();
        vehicleController = GetComponent<ArcadeAiVehicleController>();
        vehicleController.enabled = false;
        health.Init(aiStats.Health);

        base.Init(stats);

        //Must be called after base.Init()
        vehicleController.MaxSpeed = maxSpeed;
    }

    public override void HandleInRangeEnter()
    {
        AttackTelegraph.SetActive(true);
    }

    public override void HandleInRangeExit()
    {
        AttackTelegraph.SetActive(false);
    }

    public override void HandleInPoolExit()
    {
        vehicleController.enabled = true;
        base.HandleInPoolExit();
    }

    public override void HandleAttackingEnter()
    {
        //stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
        Attack();
    }

    public override void Attack()
    {
        Debug.Log("Vehicle attacking!!!");
        //RamCar just drives directly into the player (if targeted)
        // SetTarget(playerHealth.gameObject);
        CalculateAttackMovement();
    }

    public override void SetTarget(GameObject targ)
    {
        vehicleController.target = targ.transform;
        base.SetTarget(targ);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();


            Debug.DrawLine(transform.position, transform.position + vehicleController.carVelocity);
            //Damage player bike based on difference in velocity * multiplier
            playerHealth.TakeDamage(DamageMultiplier *
                (vehicleController.carVelocity - playerMovement.Velocity).magnitude);

            //bikeRB.AddTorque(Vector3.up * Random.Range(-MAX_RANDOM_TORQUE, MAX_RANDOM_TORQUE), ForceMode.Impulse);
        }
    }

    public override void HandleDeathEnter()
    {
        float minMaxTorque = 1800f;
        rb.angularDrag = 1;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(new Vector3(0, 20f, 0), ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(-minMaxTorque, minMaxTorque),
                                Random.Range(-minMaxTorque, minMaxTorque),
                                Random.Range(-minMaxTorque, minMaxTorque)),
                                ForceMode.Impulse);
        vehicleController.enabled = false;

        base.HandleDeathEnter();
    }

    #region MOVEMENT
    //All vehicles have a target, but some vehicles interact with their targets in different ways
    protected void UpdateMovementLocation()
    {
        if (target != null)
        {
            movementTargetPosition.transform.position = GetChaseLocation();
            vehicleController.target = movementTargetPosition.transform;
        }
    }

    protected void CalculateAttackMovement()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        movementTargetPosition.transform.position = (20 * direction) + target.transform.position;
    }

    protected Vector3 GetChaseLocation()
    {
        return stats.ChaseRange * Vector3.Normalize(transform.position - target.transform.position) + target.transform.position;
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
    #endregion
}
