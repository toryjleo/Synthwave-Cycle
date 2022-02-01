using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntAI : AiTemplate
{


    public override void Init()
    {
        alive = true;
        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        animationStateController = GetComponent<CyborgAnimationStateController>();
        //location = transform.position;
        maxSpeed = 20;
        maxForce = 1;

        StartingHP = 80;
        score = 300;
        hp.Init(StartingHP);

        attackRange = 4;

        if (animationStateController == null)
        {
            Debug.LogError("This object needs a CyborgAnimationStateController component");
        }
        if (rb == null)
        {
            Debug.LogError("This object needs a rigidBody component");
        }
        if (hp == null)
        {
            Debug.LogError("This object needs a health component");
        }
        if (myGun == null)
        {
            Debug.LogError("This object needs a Gun component");
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
