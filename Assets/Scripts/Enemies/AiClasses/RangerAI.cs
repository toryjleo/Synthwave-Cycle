using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerAi : InfantryAI
{
    public GameObject muzzleLocation; // Empty GameObject set to the location of the barrel

    public override void Init()
    {
        alive = true;

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

    public override Enemy GetEnemyType()
    {
        return Enemy.Ranger;
    }
}
