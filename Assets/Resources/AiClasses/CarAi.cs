using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAi : Ai
{

    private BikeMovementComponent movementComponent;
    public override void Init()
    {
        
    }

    // Start is called before the first frame update
    void Awake()
    {
        movementComponent = GetComponent<BikeMovementComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        float fowardAmount = 0;
        float turnAmount = 0f;
    }
}
