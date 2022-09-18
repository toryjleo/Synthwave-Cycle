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
        maxSpeed = 100;
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

        float x = 0f;
        float z = 0f;

        float dot = Vector3.Dot(movementComponent.ForwardVector(), desiredVec);


        print(dot);
        
        movementComponent.control(x, z);
    }
  
}
