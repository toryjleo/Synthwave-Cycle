using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is the Ai to be used by Car Enemies, it requires a rigid body, health component, and a movement componenet to function properly
/// </summary>
public class CarAi : Ai
{
    public float Hitpoints
    {
        get => hp.HitPoints;
    }
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
        attackRange = 5;
        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        movementComponent = GetComponent<CarMovementComponent>();
        this.Despawn += op_ProcessCompleted;
        hp.Init(StartingHP);
    }
    public override void Update()
    {
        base.Update();
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

            //This causes the car to have a deadzone of steering 
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
            //These functions determine if the car will back up or try to turn around
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