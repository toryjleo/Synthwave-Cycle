using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeAI : Ai
{
    public GameObject muzzleLocation; // Empty GameObject set to the location of the barrel

    public GameObject[] trackingPoints;

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
        attackRange = 5;

        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        animationStateController = GetComponent<CyborgAnimationStateController>();
        this.Despawn += op_ProcessCompleted;
        hp.Init(StartingHP);

        trackingPoints = GameObject.FindGameObjectsWithTag("TrackerChild");




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

    public GameObject findNearestTrackingPoint()
    {
        GameObject nearestTrackingPoint = null;
        Vector3 trackingPoint;
        float shortestDistance = 0;
        foreach( GameObject ty in trackingPoints)
        {
            trackingPoint = ty.transform.position;
            Vector3 distance = trackingPoint - this.transform.position;
            float dMag = distance.magnitude;
            if(dMag < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = dMag;
                nearestTrackingPoint = ty;
            }
        }
        


        return nearestTrackingPoint;
    }

    public override void Move(Vector3 t)
    {
        t = findNearestTrackingPoint().transform.position;

        BikeMovementComponent bmc = target.GetComponent<BikeMovementComponent>();

        Vector3 desiredVec = t - transform.position; //this logic creates the vector between where the entity is and where it wants to be 


        Vector3 BikeForward = bmc.ForwardVector();
        
        Debug.DrawRay(rb.transform.position, BikeForward.normalized * 30, Color.red);


        //this.transform.LookAt(target.transform.position);
        this.transform.LookAt(t);

        float dMag = desiredVec.magnitude; 
        
        dMag -= attackRange; 

        desiredVec.Normalize();

        if (dMag < 5)
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
        base.Update();
        //Move(target.transform.position);
    }


}

