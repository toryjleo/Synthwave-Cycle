using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntAI : AiTemplate
{
    public GameObject muzzleLocation; // Empty GameObject set to the location of the barrel

    public override void Init()
    {
        alive = true;
        StartingHP = 120;
        score = 300;
        maxSpeed = 10;
        attackRange = 2;

        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        animationStateController = GetComponent<CyborgAnimationStateController>();
        this.Despawn += op_ProcessCompleted;
        hp.Init(StartingHP);

        #region Error Checkers

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
        #endregion
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
    }


}
