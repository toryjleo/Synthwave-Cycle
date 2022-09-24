using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the Ai component used by the bikes. It has a unique movement component and TODO: Will be adding a ramming feature and gun on these bikes
/// </summary>
public class BikeAI : Ai
{
    public GameObject muzzleLocation; // Empty GameObject set to the location of the barrel

    public EnemyTurret turret;
    public GameObject[] trackingPoints;


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

        maxSpeed = 100;
        attackRange = 30;

        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        this.Despawn += op_ProcessCompleted;
        hp.Init(StartingHP);
        trackingPoints = GameObject.FindGameObjectsWithTag("TrackerChild");

        //Initializes Turret 
        if (GetComponentInChildren<EnemyTurret>() != null)
        {
            turret = GetComponentInChildren<EnemyTurret>();
            turret.Init();
            //turret.BulletShot += bl_ProcessCompleted;
        }



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


    public override void Move(Vector3 target)
    {
        base.Move(findNearestTrackingPoint().transform.position);
    }

    //stats used in construction

    public override void Attack()
    {
        turret.Shoot(rb.velocity);
    }

    public override void Aim(Vector3 aimAt)
    {
        
    }




    public override void Update()
    {
        base.Update();
    }


}

