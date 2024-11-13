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

    //How much additional force does a ram apply
    [SerializeField] public float RamModifier = 0.2f;

    [SerializeField] public GameObject itemDrop;

    [SerializeField] public GameObject movementTargetPosition;

    //How much directional/rotational force effects the player on a ram
    private const float MAX_RANDOM_TORQUE = 4500f;
    private const float MAX_RAM_MAGNITUDE = 200f;

    //This is the time the Vehicle has spent within CONFIDENCE_BUILD_DISTANCE to it's target
    //When it exceeds TIME_BY_TARGET_TO_ATTACK the car is ready to attack
    // internal float timeByTarget = 0;
    // internal float TIME_BY_TARGET_TO_ATTACK;
    internal const float CONFIDENCE_BUILD_DISTANCE = 45f;

    //All vehicles have a target, but some vehicles interact with their targets in different ways
    public abstract void UpdateMovementLocation();

    public override void ManualUpdate(ArrayList enemies, Vector3 wanderDirection, float fixedDeltaTime)
    {
        base.ManualUpdate(enemies, wanderDirection, fixedDeltaTime);
        //Figure out where to moved based on the child class movement pattern
        // UpdateMovementLocation();
    }

    public override void Init(IPoolableInstantiateData stats)
    {
        AiStats aiStats = stats as AiStats;
        if (!aiStats)
        {
            Debug.LogWarning("VehicleAi stats are not readable as TestAi!");
        }

        hp = GetComponentInChildren<Health>();
        vehicleController = GetComponent<ArcadeAiVehicleController>();
        vehicleController.enabled = false;
        hp.Init(aiStats.Health);

        base.Init(stats);

        //Must ba called after base.Init()
        vehicleController.MaxSpeed = maxSpeed;
    }

    // public override void InitStateController()
    // {
    //     base.InitStateController();
    // }

    // TODO: Follow the player until inRange, THEN set their vehicle target to attack

    public override void HandleInPoolExit()
    {
        vehicleController.enabled = true;
        base.HandleInPoolExit();
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

    public override void Die()
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

        base.Die();
    }

}
