using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class<c>VehicleAI</c> 
/// VehicleAI holds all the code that makes an enemy Vehicle different form other enemy types
public class VehicleAI : Ai
{
    public ArcadeAiVehicleController vehicleController;

    //How much damage ramming deals
    [SerializeField]
    public float DamageMultiplyer = 1.0f;

    //How much additional force does a ram apply
    [SerializeField] 
    public float RamModifier = 0.2f;

    [SerializeField]
    public GameObject itemDrop;

    public GameObject movementTarget;

    //How much directional/rotational force effects the player on a ram
    private const float MAX_RANDOM_TORQUE = 4500f;
    private const float MAX_RAM_MAGNITUDE = 200f;

    //This is the time the Vehicle has spent within CONFIDENCE_BUILD_DISTANCE to it's target
    //When it exceeds TIME_BY_TARGET_TO_ATTACK the car is ready to attack
    private float timeByTarget = 0;
    private float TIME_BY_TARGET_TO_ATTACK;
    private const float CONFIDENCE_BUILD_DISTANCE = 25f;
   
    public override void NewLife()
    {
        TIME_BY_TARGET_TO_ATTACK = Random.Range(3, 11);
        base.NewLife();
    }

    public override void Attack() 
    {
        // TODO: Fix
        /*if (myGun != null && myGun.CanShootAgain() && alive)
        {
            this.myGun.PrimaryFire(target.transform.position);
        }*/
    }

    public override void Update()
    {
        base.Update();
        if(movementTarget != null)
        {
            if (Vector3.Distance(transform.position, movementTarget.transform.position) <= CONFIDENCE_BUILD_DISTANCE)
            {
                timeByTarget += Time.deltaTime;
            }
            else
            {
                timeByTarget -= Time.deltaTime;
            }
            if (timeByTarget < 0)
            {
                timeByTarget = 0;
            }
            else if (timeByTarget > TIME_BY_TARGET_TO_ATTACK)
            {
                timeByTarget = TIME_BY_TARGET_TO_ATTACK;
            }
        }
    }



    //If Car has spent enough time by the player, ATTACK!
    public bool IsConfident()
    {
        return timeByTarget >= TIME_BY_TARGET_TO_ATTACK;
    }

    public override void Init()
    {
        alive = true;
        hp = GetComponentInChildren<Health>();
        vehicleController = GetComponent<ArcadeAiVehicleController>();
        vehicleController.enabled = false;
        DeadEvent += CarDeath;
        this.Despawn += op_ProcessCompleted;
        hp.Init(StartingHP);
    }

    public override void SetTarget(GameObject targ)
    {
        vehicleController.enabled = true;
        base.SetTarget(targ);
    }

    public void SetMovementTarget(GameObject targ)
    {
        movementTarget = targ;
        vehicleController.target = targ.transform;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            
            Debug.DrawLine(transform.position, transform.position + vehicleController.carVelocity);
            //Damage player bike based on difference in velocity * multiplier
            playerHealth.TakeDamage(DamageMultiplyer * 
                (vehicleController.carVelocity - playerMovement.Velocity).magnitude);

            //bikeRB.AddTorque(Vector3.up * Random.Range(-MAX_RANDOM_TORQUE, MAX_RANDOM_TORQUE), ForceMode.Impulse);
        }
    }

    public void CarDeath()
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
        Instantiate(itemDrop, this.transform.position, Quaternion.identity);
    }

    public override Enemy GetEnemyType()
    {
        return Enemy.Car;
    }
}
