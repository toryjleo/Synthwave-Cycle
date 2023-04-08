using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryAi : Ai
{
    public GameObject muzzleLocation;
    public override void Init()
    {
        alive = true;
        StartingHP = 1;
        score = 5;
        maxSpeed = 0;
        attackRange = 0;
        minimumRange = 0;
        speedBoost = 0;

        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        animationStateController = null;
        this.Despawn += op_ProcessCompleted;
        hp.Init(StartingHP);


        #region Error Checkers
        if (hp == null)
        {
            Debug.LogError("This object needs a health component");
        }
        #endregion


    }

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
        if(hitpoints <=0)
        {
            this.gameObject.SetActive(false);
        }
    }

}
