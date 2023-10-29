using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the movement behavior our bike uses
/// </summary>
public class BikeMovementComponent : MonoBehaviour, IResettable
{
    // Parent of the bike mesh. This is used to get the forward vector of the bike. 
    // The forward vector of the bike will change as we alter the rotation of this variable.
    public GameObject bikeMeshParent;
    // The gameObject that holds the bike mesh. This will only be used for animations.
    public GameObject bikeMeshChild;

    //Transforms that float in front of the bike on either side for vehicles to chase
    public GameObject trackerFR;
    public GameObject trackerFL;

    public Vector3 appliedForce; // The force being applied to the bike
    public Rigidbody rb;

    // Dictates movement speed
    public Health health;


    public float MoveSpeed = 100; //The speed of the bike 

    public float Traction = 3; //How slippy the bike is when turning 

    public float SteerAngle = 10; //the angle at which the bike turns

    public float SideForce = 100; //The force imparted on the bike to allow for lateral movement 

    private float dragCoefficient = .99f; // A linear scale of how much drag will be applied to the bike

    private float maxLean = 40.0f;

    private const float ACCELERATION_SCALE = 10.0f;

    private const float MAX_SPEED_SCALE = 10;

    private const float STARTING_HEALTH = 200.0f;

    private const float MAX_ACCELERATION = 1000.0f;

    private const float MIN_ACCELERATION = 10.0f;

    [SerializeField]
    public bool Debug_Invulnurability = false;

    /// <summary>
    /// The current acceleration of the bike. Is dependant on health
    /// </summary>
    private float Acceleration 
    {
        get 
        {
            return Mathf.Clamp( (HitPoints / ACCELERATION_SCALE) + MIN_ACCELERATION, MIN_ACCELERATION, MAX_ACCELERATION);
        }
    }

    public float MaxSpeed 
    {
        get 
        {
            return (HitPoints / ACCELERATION_SCALE);
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

    private void Update()
    {
        rb.velocity = (Vector2)Vector3.ClampMagnitude(rb.velocity, MaxSpeed);
    }

    private void FixedUpdate()
    {
        if (GameStateController.GameIsPlaying())
        {
            ApplyForces();
            rb.rotation = new Quaternion(0, rb.rotation.y, 0, rb.rotation.w);
        }
    }

    public void TakeDamage(float damageTaken)
    {
        health.TakeDamage(damageTaken);
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



    #region Directional Vectors
    public Vector3 ForwardVector() //Returns a vector of the bike's FORWARD
    {
        return new Vector3(-bikeMeshParent.transform.right.x, bikeMeshParent.transform.right.y, -bikeMeshParent.transform.right.z);
    }

    public Vector3 RightVector() //Returns a vector of the bikes Right
    {
        return new Vector3(bikeMeshParent.transform.forward.x, bikeMeshParent.transform.right.y, bikeMeshParent.transform.forward.z); 
    }
    public Vector3 LeftVector() //Returns a vector of the bikes Left
    {
        return new Vector3(-bikeMeshParent.transform.forward.x, bikeMeshParent.transform.right.y, -bikeMeshParent.transform.forward.z);
    }
#endregion

    #region Forces
    /// <summary> Main method for controlling bike 
    /// Applies forces to Rigid body in relation to player input 
    /// </summary>
    public void ApplyForces()
    {

        //Movement Forward and Back and applies velocity 
        appliedForce += ForwardVector().normalized * Acceleration * Input.GetAxis("Vertical") * Time.fixedDeltaTime;

        
        //Latteral movement inputs, q and e add side force to the bike 
        //if (Input.GetKey(KeyCode.E))
        //{
        //    rb.AddForce(RightVector() * SideForce);
        //}
        //if (Input.GetKey(KeyCode.Q))
        //{
        //    rb.AddForce(LeftVector() * SideForce);
        //}

        //Debug.Log(Input.GetAxis("Horizontal"));
        //Steering Takes Horizontal Input and rotates both 
        float steerInput = Input.GetAxis("Horizontal");
        bikeMeshChild.transform.localRotation = Quaternion.Euler(maxLean * steerInput, 0, 0);
        bikeMeshParent.transform.Rotate(Vector3.up * steerInput * (appliedForce.magnitude + 100) * Time.fixedDeltaTime);
        //Drag and MaxSpeed Limit to prevent infinit velocity  
        appliedForce *= dragCoefficient;

        // Debug lines
        /*
        Debug.DrawRay(rb.transform.position, ForwardVector().normalized * 30, Color.red);
        Debug.DrawRay(rb.transform.position, appliedForce.normalized * 30, Color.blue);
        */
        //Lerp from actual vector to desired vector 
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

    public void ResetGameObject()
    {
        rb.angularVelocity = new Vector3(0, 0, 0);
        rb.velocity = new Vector3(0, 0, 0);
        rb.transform.rotation= Quaternion.Euler(new Vector3(0f, 0f, 0f));
        appliedForce = new Vector3(0, 0, 0);
        Init();
    }

    #endregion
}
