using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : AiTemplate
{


    public GameObject muzzleLocation; // Empty GameObject set to the location of the barrel

    //stats used in construction
    private Health hp;
    private float score;
    public float StartingHP;
    float maxSpeed;
    float maxForce;
    public float attackRange; //TODO: This will be set when creating different inherited classes for Monobehavior;
    bool alive;


    public float hitpoints
    {
        get => hp.HitPoints;
    }
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
