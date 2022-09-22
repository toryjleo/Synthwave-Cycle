using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAi : Ai
{
    public void Start()
    {
        Init();
    }
    private CarMovementComponent movementComponent;
    public override void Init()
    {
        alive = true;
        StartingHP = 100;
        score = 300;
        attackRange = 30;

        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();

        this.Despawn += op_ProcessCompleted;
        hp.Init(StartingHP);
        

    }

    public override void Update()
    {
        //print(hp.HitPoints);
        base.Update();
    }

    public float hitpoints
    {
        get => hp.HitPoints;
    }

    void Awake()
    {
        movementComponent = GetComponent<CarMovementComponent>();
    }

    /// <summary>
    /// This method is called during update to Move the Ai. 
    /// </summary>
    /// <param name="target"></param>
    public override void Move(Vector3 target)
    {
        Vector3 desiredVec = target - rb.transform.position;
        float distanceToTarget = desiredVec.magnitude;
        float reachTargetDistance = 7f;
        float z = 0f;
        float deadZone = 5;

        if(distanceToTarget > reachTargetDistance)
        {
            desiredVec = desiredVec.normalized;
            
            //Values 
            float dot = Vector3.Dot(movementComponent.ForwardVector(), desiredVec); //This dot protduct returns -1 to 1 if the car is behind to infront of the target. 
            float angleToDir = Vector3.SignedAngle(movementComponent.ForwardVector(), desiredVec, Vector3.up);

            if (angleToDir > deadZone)
            {
                z = 1;
            }
            else if (angleToDir < -deadZone)
            {
                z = -1;
            } else
            {
                z = 0;
            }

            print(alive);

            if (distanceToTarget < attackRange)
            {
                movementComponent.control(dot, z);
            } else if (distanceToTarget >attackRange)
            {
                movementComponent.control(1, z);
            } 

            
           
        }


        
    }
  
}
