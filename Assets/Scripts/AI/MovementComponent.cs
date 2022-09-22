using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementComponent : MonoBehaviour
{
    //Used for Designating Parts of Object
    public GameObject MeshParent;
    public GameObject MeshChild;

    //Physical Components 
    public Rigidbody rb;
    public Health health;

    //Controls 
    public float verticalInput;
    public float horizontalInput;

    //Movement
    public Vector3 appliedForce; // The force being applied to the bike
    public float Traction = 3; //How slippy the bike is when turning 
    public float dragCoefficient = .98f; // A linear scale of how much drag will be applied to the bike

    //Stearing
    public float SteerAngle = 10; //the angle at which the bike turns
    public float maxLean = 40.0f;

    
    public float ACCELERATION_SCALE = 5.0f;
    public float STARTING_HEALTH = 200.0f;
    public float MAX_ACCELERATION = 1000.0f;

    #region Getters


    
    /// <summary>
    /// The current acceleration of the bike. Is dependant on <see cref="health"/>
    /// </summary>
    protected float Acceleration
    {
        get
        {
           return Mathf.Clamp(HitPoints / ACCELERATION_SCALE, STARTING_HEALTH / ACCELERATION_SCALE, MAX_ACCELERATION);
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

    /// <summary>
    /// Depended on Mesh Of Prefab, this will yeild, the FORWARD Direction
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 ForwardVector()
    {
        return new Vector3(-MeshParent.transform.right.x, MeshParent.transform.right.y, -MeshParent.transform.right.z);
    }

    #endregion

    #region StartUp
    private void Awake()
    {
        Init();
    }
    /// <summary>Initialize this class's variables. A replacement for a constructor.</summary>
    private void Init()
    {
        // The bike will begin at rest
        appliedForce = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody>();
        health = GetComponentInChildren<Health>();
        health.Init(STARTING_HEALTH);
    }
    #endregion

    public virtual void FixedUpdate()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        ApplyForces();
    }

    public virtual void ApplyForces()
    {
        //Movement Forward and Back and applies velocity 
        appliedForce += ForwardVector().normalized * Acceleration * verticalInput * Time.fixedDeltaTime;

        //Steering Takes Horizontal Input and rotates both 
        float steerInupt = horizontalInput;
        MeshChild.transform.localRotation = Quaternion.Euler(maxLean * steerInupt, 0, 0);

        //Drag and MaxSpeed Limit to prevent infinit velocity  
        appliedForce *= dragCoefficient;
        appliedForce = Vector3.Lerp(appliedForce.normalized, ForwardVector().normalized, Traction * Time.fixedDeltaTime) * appliedForce.magnitude;
        rb.AddForce(appliedForce);
    }




}
