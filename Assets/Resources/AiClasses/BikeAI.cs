using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeAI : Ai
{
    public GameObject muzzleLocation; // Empty GameObject set to the location of the barrel

    public Vector3 velocity;
    public Vector3 STR;
    public Vector3 TRG;
    public Vector3 offset;
    public override void Init()
    {
        alive = true;
        StartingHP = 40;
        score = 300;
        maxSpeed = 100;
        attackRange = 3;

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

    public override void Move(Vector3 t)
    {
        Vector3 desiredVec = t - transform.position + offset; //this logic creates the vector between where the entity is and where it wants to be 

        

        float dMag = desiredVec.magnitude; //this creates a magnitude of the desired vector. This is the distance between the points 
        
        dMag -= attackRange; // dmag is the distance between the two objects, by subtracking this, I make it so the object doesn't desire to move as far.  

        desiredVec.Normalize(); // one the distance is measured this vector can now be used to actually generate movement,
                                // but that movement has to be constant or at least adaptable, which is what the next part does  
        transform.LookAt(desiredVec);

        //Currently Walking twoards the target 

        
            desiredVec *= maxSpeed;
            Attack();
        
        Vector3 steer = desiredVec - rb.velocity; //Subtract Velocity so we are not constantly adding to the velocity of the Entity

        velocity = rb.velocity;
        STR = steer;
        TRG = desiredVec;
        
        applyForce(steer);
    }



    //stats used in construction

    public float hitpoints
    {
        get => hp.HitPoints;
    }

    void Awake()
    {
        Init();
    }

    public override void Update()
    {
        //base.Update();
        Move(target.transform.position);
    }


}

