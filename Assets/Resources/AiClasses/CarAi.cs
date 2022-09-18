using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAi : Ai
{

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

    // Start is called before the first frame update
    void Awake()
    {
        movementComponent = GetComponent<CarMovementComponent>();
    }

    // Update is called once per frame
    public override void Move(Vector3 target)
    {
        Vector3 desiredVec = target - rb.transform.position;
        float distanceToTarget = desiredVec.magnitude;
        float reachTargetDistance = 7f;
        float z = 0f;

        if(distanceToTarget > reachTargetDistance)
        {
            desiredVec = desiredVec.normalized;


            float dot = Vector3.Dot(movementComponent.ForwardVector(), desiredVec); //This dot protduct returns -1 to 1 if the car is behind to infront of the target. 

            float angleToDir = Vector3.SignedAngle(movementComponent.ForwardVector(), desiredVec, Vector3.up);

            if (angleToDir > 0)
            {
                z = 1;
            }
            else
            {
                z = -1;
            }

            Debug.Log(angleToDir);

            movementComponent.control(dot, z);
        }


        
    }
  
}
