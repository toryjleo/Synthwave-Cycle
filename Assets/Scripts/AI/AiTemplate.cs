using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiTemplate : SelfDespawn
{


    public GameObject target;
    public CyborgAnimationStateController animationStateController;
    public Rigidbody rb;
    public Gun myGun;
    public Health hp;




    public float score;
    public float StartingHP;
    public float maxSpeed;
    public float maxForce;
    public float attackRange;
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
