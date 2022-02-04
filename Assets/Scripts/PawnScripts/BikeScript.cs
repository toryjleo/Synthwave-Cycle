using System.Collections;

using System.Collections.Generic;
using UnityEngine;


// TODO: Make inherit from Pawn class
/// <summary>Class <c>BikeScript</c> A Unity Component which holds the logic for the Player movement.</summary>
/// Expects there to be an LMG spawned in-place on the bike's location
public class BikeScript : MonoBehaviour
{
    public GameObject bikeMeshParent; // Parent of the bike mesh. This is used to get the forward vector of the bike. 
    // The forward vector of the bike will change as we alter the rotation of this variable
    public GameObject bikeMeshChild; // The gameObject that holds the bike mesh. This will only be used for animations.
    public Vector3 deltaPosition; // How far the bike has moved in world coordinates this frame
    public Vector3 velocity; // The velocity of the bike
    public Vector3 acceleration; // The acceleration of the bike this frame

    //private float mass = 8f; // The mass of the bike
    private float engineForce = 75f; // The force of the engine
    private float rotationSpeed = 10f; // A linear scale of how fast the bike will turn
    private float MaxSpeed = 100;
    public float MoveSpeed = 50;
    public float Traction = 10;

    public float SteerAngle = 20;

    public Vector3 MoveForce;

    private float dragCoefficient = .98f; // A linear scale of how much drag will be applied to the bike

    private float maxLean = 40.0f; 

    public Gun currentGun;

    private Rigidbody rb;

    private Health health;

    public float Energy
    {
        get => health.HitPoints;
    }

    private void Awake()
    {
        Init();

        // TODO: make this less gross. Update gun/equip code
        InitializeStartingGun();
    }

    private void FixedUpdate()
    {
        ApplyForces();
    }

    private void Update()
    { 
    }

    /// <summary>Initialize this class's variables. A replacement for a constructor.</summary>
    private void Init() 
    {
        // The bike will begin at rest
        velocity = new Vector3(0, 0,0);
        acceleration = new Vector3(0,0, 0);
        rb = GetComponent<Rigidbody>();
        health = GetComponentInChildren<Health>();
    }

    /// <summary>Initialize the gun for the player to start with.</summary>
    private void InitializeStartingGun() 
    {
        // TODO: Make this better
        DoubleBarrelLMG[] guns = Object.FindObjectsOfType<DoubleBarrelLMG>();
        if (guns.Length <= 0)
        {
            Debug.LogError("BikeScript did not find any DoubleBarrelLMGs in scene");
        }
        else
        {
            EquipGun(guns[0]);
        }
    }

    /// <summary>Clears the rotation of the child mesh object.</summary>
    public void ClearRotation()
    {
        bikeMeshChild.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>Applies a force in the direction of the bike's forward vector.</summary>
    public void MoveForward()
    {
        velocity = (ForwardVector().normalized * engineForce);
    }

    /// <summary>Applies a force in the opposite direction of the bike's forward vector.</summary>
    public void MoveBackward()
    {
        
        Vector3 forward = ForwardVector().normalized;
        velocity = (-forward * engineForce);
        //rb.AddForce(-forward * engineForce);
    }

    /// <summary>Rotates the bike's mesh in a clockwise fashion.</summary>
    public void TurnRight()
    {
        bikeMeshChild.transform.localRotation = Quaternion.Euler(-maxLean, 0, 0);
        RotateYAxis(-rotationSpeed * Time.fixedDeltaTime); // A simple animation
    }

    /// <summary>Rotates the bike's mesh in a counter-clockwise fashion.</summary>
    public void TurnLeft()
    {
        bikeMeshChild.transform.localRotation = Quaternion.Euler(maxLean, 0, 0);
        RotateYAxis(rotationSpeed * Time.fixedDeltaTime); // A simple animation
    }

    /// <summary>This method gets the direction the bike's mesh is currently facing in world coordinates.</summary>
    /// <returns>A Vector3 of the bike's forward vector in world coordinates. The Vector's x represents the x direction 
    /// in world coordinates and the vector's y represents the z direction in world coordinates.</returns>
    private Vector3 ForwardVector()
    {
        return new Vector3(-bikeMeshParent.transform.right.x, bikeMeshParent.transform.right.y, -bikeMeshParent.transform.right.z);
    }

    /// <summary>This method gets the right direction of the bike's mesh in world coordinates.</summary>
    /// <returns>A Vector3 of the bike's right vector in world coordinates.</returns>
    private Vector2 RightVector() 
    {
        return new Vector2(-bikeMeshParent.transform.forward.x, bikeMeshParent.transform.forward.z);
    }

    /// <summary>Applies all of the bike's internaly applied forces. Also gets player input.</summary>
    public void ApplyForces()
    {
        Rigidbody rb = GetComponent<Rigidbody>();



        if (Input.GetKey(KeyCode.Mouse0))
        {
            currentGun.Shoot(rb.velocity);
        }


        velocity += ForwardVector().normalized * engineForce * MoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        rb.AddForce(velocity);

        float steerInupt = Input.GetAxis("Horizontal");
        bikeMeshChild.transform.localRotation = Quaternion.Euler(maxLean * steerInupt, 0, 0);
        bikeMeshParent.transform.Rotate(Vector3.up * steerInupt * velocity.magnitude * Time.deltaTime);


        velocity *= dragCoefficient;
        velocity = Vector3.ClampMagnitude(velocity, MaxSpeed);

        velocity = Vector3.Lerp(ForwardVector().normalized, transform.forward, Traction * Time.deltaTime) * velocity.magnitude;



        Debug.DrawRay(rb.transform.position, ForwardVector().normalized * 10, Color.red);
        Debug.DrawRay(rb.transform.position, velocity.normalized * 10, Color.blue);

    }

    /// <summary>Sets the bikeMeshParent's local yAngle to the unput float.</summary>
    /// <param name="speedAndDirection">The yAngle at which to set bikeMeshParent</param>
    private void RotateYAxis(float speedAndDirection)
    {
        bikeMeshParent.transform.Rotate(0, speedAndDirection, 0, Space.Self);
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

    /// <summary>Equips the gun to the bike.</summary>
    /// <param name="gunToEquip">The gun which will be hooked up to the bike's bl_ProcessCompleted. Will be set as a 
    /// child of bikeMeshParent.</param>
    public void EquipGun(Gun gunToEquip) 
    {
        if (currentGun != null) 
        {
            // Remove event handled from current gun
            currentGun.BulletShot -= bl_ProcessCompleted;
        }

        this.currentGun = gunToEquip;
        // Make gun child of TracerMeshParent
        currentGun.transform.parent = bikeMeshParent.transform;

        // Hook up event
        currentGun.BulletShot += bl_ProcessCompleted;
    }

}
