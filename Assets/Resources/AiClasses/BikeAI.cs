using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the Ai component used by the bikes. It has a unique movement component and TODO: Will be adding a ramming feature and gun on these bikes
/// </summary>
public class BikeAI : Ai
{
    public GameObject muzzleLocation; // Empty GameObject set to the location of the barrel

    public float Hitpoints
    {
        get => hp.HitPoints;
    }


    void Awake()
    {
        Init();
    }
    
    public Vector3 velocity;
    public Vector3 STR;
    public Vector3 TRG;
    public Vector3 offset;
    public override void Init()
    {
        alive = true;
        StartingHP = 40;
        score = 300;
        maxSpeed = 80;
        attackRange = 1;

        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        animationStateController = GetComponent<CyborgAnimationStateController>();
        this.Despawn += op_ProcessCompleted;
        hp.Init(StartingHP);

        


        #region Error Checkers


        if (rb == null)
        {
            Debug.LogError("This object needs a rigidBody component");
        }
        if (hp == null)
        {
            Debug.LogError("This object needs a health component");
        }
        #endregion
    }
    /// <summary>
    /// This is a custom move method for the bikes since they will move vastly differently from other AI
    /// </summary>
    /// <param name="t"></param>
    public override void Move(Vector3 t)
    {
        BikeMovementComponent bmc = target.GetComponent<BikeMovementComponent>();

        Vector3 desiredVec = t - transform.position; //this logic creates the vector between where the entity is and where it wants to be 
        float dMag = desiredVec.magnitude;
        desiredVec.Normalize();
        Vector3 BikeForward = bmc.ForwardVector();
        
        Debug.DrawRay(rb.transform.position, BikeForward.normalized * 30, Color.red);


        transform.LookAt(t);


        if (dMag < maxSpeed)
        {
            desiredVec *= dMag;

            //Attack();
        }
        else
        {
            desiredVec *= maxSpeed;
        }
        Vector3 steer = desiredVec - rb.velocity; //Subtract Velocity so we are not constantly adding to the velocity of the Entity
        applyForce(steer);
    }



    //stats used in construction




    public override void Update()
    {
        base.Update();
        //Move(target.transform.position);
    }


}

