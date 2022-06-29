using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the movement behavior our bike uses
/// </summary>
public class BikeMovementComponent : MonoBehaviour
{
    // Parent of the bike mesh. This is used to get the forward vector of the bike. 
    // The forward vector of the bike will change as we alter the rotation of this variable.
    public GameObject bikeMeshParent;
    // The gameObject that holds the bike mesh. This will only be used for animations.
    public GameObject bikeMeshChild;

    public Vector3 appliedForce; // The force being applied to the bike
    public Rigidbody rb;

    // Dictates movement speed
    private Health health;


    public float MoveSpeed = 100; //The speed of the bike 
    public float Traction = 3; //How slippy the bike is when turning 

    public float SteerAngle = 10; //the angle at which the bike turns

    private float dragCoefficient = .98f; // A linear scale of how much drag will be applied to the bike

    private float maxLean = 40.0f;

    private const float ACCELERATION_SCALE = 5.0f;

    private const float STARTING_HEALTH = 200.0f;

    private const float MAX_ACCELERATION = 1000.0f;

    /// <summary>
    /// The current acceleration of the bike. Is dependant on health
    /// </summary>
    private float Acceleration 
    {
        get 
        {
            return Mathf.Clamp( HitPoints / ACCELERATION_SCALE, STARTING_HEALTH / ACCELERATION_SCALE, MAX_ACCELERATION);
        }
    }

    // Number of player hit points
    public float HitPoints 
    {
        get 
        {
            return health.HitPoints;
        }
    }

    private void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
        ApplyForces();
    }


    /// <summary>Initialize this class's variables. A replacement for a constructor.</summary>
    public void Init()
    {
        // The bike will begin at rest
        appliedForce = new Vector3(0, 0, 0);

        rb = GetComponent<Rigidbody>();
        rb.velocity=appliedForce;
        health = GetComponentInChildren<Health>();

        
        health.Init(STARTING_HEALTH);
    }
    /// <summary>Reset the health of the bike component to initial value.</summary>
    public void ResetHealth(){
        health = GetComponentInChildren<Health>();
        health.Init(STARTING_HEALTH);
    }
    /// <summary>Reset the motion of the bike component to initial value.</summary>
    public void ResetMotion(){
        // The bike will be reset at rest
        appliedForce = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody>();
        rb.velocity=new Vector3(0, 0, 0);
        
    }
    public Vector3 ForwardVector()
    {
        return new Vector3(-bikeMeshParent.transform.right.x, bikeMeshParent.transform.right.y, -bikeMeshParent.transform.right.z);
    }

    #region Forces
    /// <summary> Main method for controlling bike 
    /// Applies forces to Rigid body in relation to player input 
    /// </summary>
    public void ApplyForces()
    {


        //Movement Forward and Back and applies velocity 
        appliedForce += ForwardVector().normalized * Acceleration * Input.GetAxis("Vertical") * Time.fixedDeltaTime;


        //Steering Takes Horizontal Input and rotates both 
        float steerInupt = Input.GetAxis("Horizontal");
        bikeMeshChild.transform.localRotation = Quaternion.Euler(maxLean * steerInupt, 0, 0);
        bikeMeshParent.transform.Rotate(Vector3.up * steerInupt * (appliedForce.magnitude + 100) * Time.fixedDeltaTime);

        //Drag and MaxSpeed Limit to prevent infinit velocity  
        appliedForce *= dragCoefficient;
        //appliedForce = Vector3.ClampMagnitude(appliedForce, MaxSpeed);

        // Debug lines
        Debug.DrawRay(rb.transform.position, ForwardVector().normalized * 30, Color.red);
        Debug.DrawRay(rb.transform.position, appliedForce.normalized * 30, Color.blue);

        appliedForce = Vector3.Lerp(appliedForce.normalized, ForwardVector().normalized, Traction * Time.fixedDeltaTime) * appliedForce.magnitude;


        rb.AddForce(appliedForce);
    }

    /// <summary>Responds to the gun'd NotifyShot event.</summary>
    /// <param name="forceOfBulletOnBike">The force of the bullet to apply to the bike.</param>
    public void bl_ProcessCompleted(Vector3 forceOfBulletOnBike)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(forceOfBulletOnBike.x, forceOfBulletOnBike.y, forceOfBulletOnBike.z));
        //velocity += acceleration * Time.fixedDeltaTime;

        // Reset acceleration for next update
        //acceleration = new Vector2(0, 0);
    }

    #endregion
}
