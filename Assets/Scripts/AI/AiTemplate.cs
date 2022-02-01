using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiTemplate : SelfDespawn
{

    //Vars for hunting the player 
    public Vector3 targetVec; //this is the vector to their quarry 
    public GameObject target;

    //potential inports 
    public CyborgAnimationStateController animationStateController;
    public Rigidbody rb;
    public Gun myGun;

    //stats used in construction 
    public Health hp;
    public float score;
    public float StartingHP;
    public float maxSpeed;
    public float maxForce;
    public float attackRange; //TODO: This will be set when creating different inherited classes for Monobehavior; 
    public bool alive;


    // Start is called before the first frame update
    public void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
