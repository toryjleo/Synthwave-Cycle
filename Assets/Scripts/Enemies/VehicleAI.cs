using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class<c>VehicleAI</c> 
/// VehicleAI holds all the code that makes an enemy Vehicle different form other enemy types
public class VehicleAI : Ai
{
    ArcadeAiVehicleController vehicleController;

    [SerializeField]
    public float DamageMultiplyer = 1.0f;

    [SerializeField] 
    public float RamModifier = 1.0f;

    [SerializeField]
    public GameObject itemDrop;

    public GameObject movementTarget;

    private const float MAX_RANDOM_TORQUE = 4500f;

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
        if (myGun != null && myGun.CanShootAgain() && alive)
        {
            this.myGun.PrimaryFire(target.transform.position);
        }
    }

    public override void Update()
    {
        base.Update();
        Vector3 aimLoc = target.transform.position;
        aimLoc += target.transform.forward * 10;
        myGun?.transform.LookAt(aimLoc);
        if(IsConfident() && Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            Attack();
        }
        if(Vector3.Distance(transform.position, movementTarget.transform.position) <= CONFIDENCE_BUILD_DISTANCE)
        {
            Debug.Log("Close to target: " + timeByTarget + "/" + TIME_BY_TARGET_TO_ATTACK);
            timeByTarget += Time.deltaTime;
        }
        else
        {
            timeByTarget -= Time.deltaTime;
        }
        if(timeByTarget < 0)
        {
            timeByTarget = 0;
        }
        else if(timeByTarget > TIME_BY_TARGET_TO_ATTACK)
        {
            timeByTarget = TIME_BY_TARGET_TO_ATTACK;
        }
    }

    //If Car has spent enough time by the player, ATTACK!
    public bool IsConfident()
    {
        return timeByTarget >= TIME_BY_TARGET_TO_ATTACK;
    }

    public override void Init()
    {
        vehicleController = GetComponent<ArcadeAiVehicleController>();
        DeadEvent += CarDeath;
    }

    public override void SetTarget(GameObject targ)
    {
        //vehicleController.enabled = true;
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
            BikeScript bike = collision.gameObject.GetComponent<BikeScript>();
            Rigidbody bikeRB = bike.movementComponent.rb;
            Debug.DrawLine(transform.position, transform.position + vehicleController.carVelocity);
            bike.movementComponent.TakeDamage(DamageMultiplyer * Mathf.Abs(bike.movementComponent.appliedForce.magnitude - vehicleController.carVelocity.magnitude));
            bikeRB.AddForce((vehicleController.carVelocity - bike.movementComponent.appliedForce) * RamModifier, ForceMode.Impulse);
            bikeRB.AddTorque(Vector3.up * Random.Range(-MAX_RANDOM_TORQUE, MAX_RANDOM_TORQUE), ForceMode.Impulse);
        }
    }

    public void CarDeath()
    {
        float minMaxTorque = 1800f;
        rb.angularDrag = 1;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(new Vector3(0, 20f, 0), ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(-minMaxTorque, minMaxTorque), Random.Range(-minMaxTorque, minMaxTorque), Random.Range(-minMaxTorque, minMaxTorque)), ForceMode.Impulse);
        vehicleController.enabled = false;
        Instantiate(itemDrop, this.transform.position, Quaternion.identity);
    }
}
