using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : AiTemplate
{


 
    public override void Init()
    {
        alive = true;
        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        animationStateController = GetComponent<CyborgAnimationStateController>();
        //location = transform.position;
        maxSpeed = 40; 
        maxForce = 1;
        
        StartingHP = 40;
        score = 100;
        hp.Init(StartingHP);

        

        if (animationStateController == null)
        {
            Debug.LogError("This object needs a CyborgAnimationStateController component");
        }
    }

    void Awake()
    {
        Init();
    }

    public override void Update()
    {
        base.Update();
    }


}
