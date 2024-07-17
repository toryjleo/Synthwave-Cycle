﻿using System.Collections;

using System.Collections.Generic;
using UnityEngine;


// TODO: Make inherit from Pawn class
/// <summary>Class <c>BikeScript</c> A Unity Component which holds the logic for the Player movement.</summary>
/// Expects there to be an LMG spawned in-place on the bike's location
public class BikeScript : MonoBehaviour, IResettable
{
    private float distanceToHP; // The current distance to the healthpool.
    private float consecutiveDistanceToHP; // The distance to the healthpool the first time we raycast hit it.

    public TurretScript turret;
    [SerializeField]
    private Arsenal arsenal;

    public BikeMovementComponent movementComponent;
    private EmmissiveBikeScript emissiveBike;

    private int healthPoolLayer = 6;
    private int healthPoolLayerMask; // A mask that that represents the HealthPool layer


    #region Accessors
    public float Energy
    {
        get => movementComponent.HitPoints;
    }

    // TODO: see if we can make attribute
    /// <summary>This method gets the direction the bike's mesh is currently facing in world coordinates.</summary>
    /// <returns>A Vector3 of the bike's forward vector in world coordinates. The Vector's x represents the x direction
    /// in world coordinates and the vector's y represents the z direction in world coordinates.</returns>
    public Vector3 ForwardVector()
    {
        return movementComponent.ForwardVector();
    }

    public Vector3 GetForwardVector()
    {
        return ForwardVector();
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
    #endregion

    #region Monobehavior
    private void Awake()
    {
        emissiveBike = GetComponentInChildren<EmmissiveBikeScript>();
        movementComponent = GetComponent<BikeMovementComponent>();


        turret = GetComponentInChildren<TurretScript>();

        //Initializes Turret 
        if (turret != null)
        {
            turret.Init();
            turret.BulletShot += movementComponent.bl_ProcessCompleted;
        }

        healthPoolLayerMask = (1 << healthPoolLayer);
        consecutiveDistanceToHP = 0;
    }


    private void Update()
    {
        if (GameStateController.GameIsPlaying())
        {
            UpdateBikeEmission();
        }
    }


    private void FixedUpdate()
    {
        if (GameStateController.GameIsPlaying())
        {
            // Handle primary and secondary fire inputs
            if (Input.GetKey(KeyCode.Mouse0))
            {
                //currentGun.Shoot(movementComponent.rb.velocity);
                arsenal.PrimaryFire(movementComponent.rb.velocity);
                if (turret != null)
                {
                    turret.PrimaryFire(movementComponent.rb.velocity);
                }
            }
            else
            {
                arsenal.ReleasePrimaryFire(movementComponent.rb.velocity);
            }

            // Handle Secondary Fire Input
            if (Input.GetKey(KeyCode.Mouse1))
            {
                arsenal.SecondaryFire(movementComponent.rb.velocity);
            }
            else
            {
                arsenal.ReleaseSecondaryFire(movementComponent.rb.velocity);
            }
        }
    }

    void OnDestroy() 
    {
        if (GetComponentInChildren<TurretScript>() != null)
        {
            turret.BulletShot -= movementComponent.bl_ProcessCompleted;
        }
    }

    #endregion


    #region Health Related
    /// <summary>Checks to see if a HealthPool is in front of the bike.</summary>
    /// <returns>True when a HealthPool is in front of the bike.</returns>
    private bool HealthPoolCheck()
    {
        Ray ray = new Ray(transform.position, ForwardVector());
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, Mathf.Infinity,  healthPoolLayerMask))
        {
            //Debug.Log("Hit something: " + hitData.collider.gameObject.name);
            distanceToHP = hitData.distance;
            if (consecutiveDistanceToHP == 0) 
            {
                consecutiveDistanceToHP = hitData.distance;
            }
            return true;
        }
        else
        {
            consecutiveDistanceToHP = 0;
            return false;
        }
    }


    /// <summary>Sets the bike's emission material color to a specific color if the bike is or is not pointing at a
    /// healthpool.</summary>
    private void UpdateBikeEmission()
    {
        if (HealthPoolCheck())
        {
            emissiveBike.SetHPDistance(distanceToHP, consecutiveDistanceToHP);
        }
        else
        {
            emissiveBike.SetNotAheadColor();
        }
    }

    public void ResetGameObject()
    {
        //Initializes Turret 
        if (turret != null)
        {
            turret.Init();
        }

        consecutiveDistanceToHP = 0;
    }

    private void RagDoll()
    {
        //will make bike "ragdoll" but the camera has an anneurism so we'll leave it out for now

        movementComponent.rb.constraints = RigidbodyConstraints.None;
        float minMaxTorque = 1800f;
        movementComponent.rb.angularDrag = 1;
        movementComponent.rb.constraints = RigidbodyConstraints.None;
        movementComponent.rb.AddForce(new Vector3(0, 20f, 0), ForceMode.Impulse);
        movementComponent.rb.AddTorque(new Vector3(Random.Range(-minMaxTorque, minMaxTorque),
                                Random.Range(-minMaxTorque, minMaxTorque),
                                Random.Range(-minMaxTorque, minMaxTorque)),
                                ForceMode.Impulse);
    }

    internal void SpeedBoost(float boostAmount)
    {
        movementComponent.rb.AddForce(movementComponent.ForwardVector() * boostAmount, ForceMode.VelocityChange);
    }
    #endregion
}
