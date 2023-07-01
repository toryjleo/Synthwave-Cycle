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
    public float RamModifier = 2.0f;

    private const float MAX_RANDOM_TORQUE = 4500f;

    //A vehicle currently does not attack, it only intercepts
    public override void Attack() { }

    public override void Init()
    {
        vehicleController = GetComponent<ArcadeAiVehicleController>();
    }

    public override void SetTarget(GameObject targ)
    {
        //vehicleController.enabled = true;
        vehicleController.target = targ.transform;
        base.SetTarget(targ);
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
}
