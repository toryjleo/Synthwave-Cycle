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


    #region Movement
    /// <summary>
    /// This method works for ranged Enemies that do not get into direct melee range with the target 
    /// </summary>
    /// <param name="target"> Vector to target </param>
    public void Move(Vector3 target) //This can be used for Enemies that stay at range and dont run into melee. 
    {

        Vector3 desiredVec = target - transform.position; //this logic creates the vector between where the entity is and where it wants to be 
        float dMag = desiredVec.magnitude; //this creates a magnitude of the desired vector. This is the distance between the points 
        dMag -= attackRange; // dmag is the distance between the two objects, by subtracking this, I make it so the object doesn't desire to move as far.  

        desiredVec.Normalize(); // one the distance is measured this vector can now be used to actually generate movement,
                                // but that movement has to be constant or at least adaptable, which is what the next part does  
        transform.LookAt(target);

        //Currently Walking twoards the target 

        if (dMag < maxSpeed)
        {
            //Slowing down walking speed as AI approaches Target 

            desiredVec *= dMag;

            if (myGun.CanShootAgain())
            {
                this.myGun.Shoot(target);
                //TODO: the AI currently doesn't stand still when firing and I'm not sure if they should?
                animationStateController.Shoot();
            }

        }
        else
        {
            desiredVec *= maxSpeed;

            //walking speed is normal 
        }

        Vector3 steer = desiredVec - rb.velocity;
        //steer.limit(maxForce);



        applyForce(steer);
    }

    public void applyForce(Vector3 force)
    {
        rb.AddForce(force);

    }


    #endregion

    #region Getters & Setters
    public bool isAlive()
    {
        return alive;
    }
    public float getScore()
    {
        return score;
    }

    #endregion
}
