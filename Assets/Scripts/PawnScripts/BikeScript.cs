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
    public Vector3 velocity; // The velocity of the bike

    //private float mass = 8f; // The mass of the bike
    private float MaxSpeed = 80; //The max speed of the bike 
    public float MoveSpeed = 100; //The speed of the bike 
    public float Traction = 3; //How slippy the bike is when turning 

    public float SteerAngle = 10; //the angle at which the bike turns 

    public Vector3 MoveForce; 

    private float dragCoefficient = .98f; // A linear scale of how much drag will be applied to the bike

    private float maxLean = 40.0f;  

    public Gun currentGun;

    private Rigidbody rb;

    private Health health;


    #region OTHER STUFF
    public float Energy
    {
        get => health.HitPoints;
    }

    #region Camera
    // The empty object the camera follows
    public GameObject cameraFollower;

    // Height of the empty object the camera follows
    public float FollowerHeight 
    {
        get 
        {
            return cameraFollower.transform.position.y;
        }
        set 
        {
            cameraFollower.transform.position = new Vector3(cameraFollower.transform.position.x, value, cameraFollower.transform.position.z);
        }
    }

    // Returns the transform of the empty object the camera follows
    public Transform CameraFollower 
    {
        get 
        {
            return cameraFollower.transform;
        }
    }
    #endregion

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

    /// <summary>This method gets the direction the bike's mesh is currently facing in world coordinates.</summary>
    /// <returns>A Vector3 of the bike's forward vector in world coordinates. The Vector's x represents the x direction 
    /// in world coordinates and the vector's y represents the z direction in world coordinates.</returns>
    private Vector3 ForwardVector()
    {
        return new Vector3(-bikeMeshParent.transform.right.x, bikeMeshParent.transform.right.y, -bikeMeshParent.transform.right.z);
    }

    #endregion


    /// <summary> Main method for controlling bike 
    /// Applies forces to Rigid body in relation to player input 
    /// </summary>
    public void ApplyForces()
    {
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            currentGun.Shoot(rb.velocity);
        }

        //Movement Forward and Back and applies velocity 
        velocity += ForwardVector().normalized * MoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime; 
        

        //Steering Takes Horizontal Input and rotates both 
        float steerInupt = Input.GetAxis("Horizontal");
        bikeMeshChild.transform.localRotation = Quaternion.Euler(maxLean * steerInupt, 0, 0);
        bikeMeshParent.transform.Rotate(Vector3.up * steerInupt * (velocity.magnitude + 100) * Time.deltaTime);

        //Drag and MaxSpeed Limit to prevent infinit velocity  
        velocity *= dragCoefficient;
        velocity = Vector3.ClampMagnitude(velocity, MaxSpeed);


        Debug.DrawRay(rb.transform.position, ForwardVector().normalized * 30, Color.red);
        Debug.DrawRay(rb.transform.position, velocity.normalized * 30, Color.blue);
        velocity = Vector3.Lerp(velocity.normalized, ForwardVector().normalized, Traction * Time.deltaTime) * velocity.magnitude;


        rb.AddForce(velocity);
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
