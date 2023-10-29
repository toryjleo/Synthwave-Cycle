using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiflemanAi : InfantryAI
{
    public GameObject muzzleLocation; 


    public override void Init()
    {
        alive = true;
        StartingHP = 20;
        score = 100;
        dlScore = 10;
        maxSpeed = 80;
        attackRange = 60;
        minimumRange = 0;
        speedBoost = 20;

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

    /// <summary>
    /// Instantiates a gun for this AI. Preferably called at Awake since Instantiate is expensive.
    /// </summary>
    /// <param name="gun">Prefab to instantiate</param>
    public void EquipGun(Gun gun)
    {
        if (myGun != null)
        {
            GameObject.Destroy(myGun.gameObject);
        }
        // This gameObject will be a child of muzzleLocation
        myGun = Instantiate<Gun>(gun, muzzleLocation.transform);
    }
}
