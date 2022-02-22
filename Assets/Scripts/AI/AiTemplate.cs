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


    public float StartingHP;
    public float maxSpeed;
    public float maxForce;
    public float score;
    public float attackRange;
    public bool alive;



    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        animationStateController.SetSpeed(rb.velocity.magnitude);


        if (hp.HitPoints <= 0) //this signifies that the enemy Died and wasn't merely Despawned 
        {
            myGun.StopAllCoroutines();
            animationStateController.StopAllCoroutines();
            alive = false;
            this.gameObject.SetActive(false);
        }
        else //Act natural 
        {
            if(target == null)
            {
                Debug.LogWarning("Target not set");
            } else
            {
                Move(target.transform.position);
                if(target.transform.position.magnitude < attackRange)
                {
                    Attack();
                }
            }
        }

    }
    public void Attack()
    {
        if (myGun.CanShootAgain())
        {
            this.myGun.Shoot(target.transform.position);
            animationStateController.Shoot();
        }
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
            desiredVec *= dMag;
            
        }
        else
        {
            desiredVec *= maxSpeed;
        }
        Vector3 steer = desiredVec - rb.velocity;
        applyForce(steer);
    }

    public void applyForce(Vector3 force)
    {
        rb.AddForce(force);
    }

    /// <summary>
    /// This method requires the entire of AI 
    /// </summary>
    /// <param name="pool"></param>
    public void seperate(List<GruntAI> pool) //this function will edit the steer of an AI so it moves away from nearby other AI 
    {
        float desiredSeperation = 5;

        Vector3 sum = new Vector3(); //the vector that will be used to calculate flee beheavior if a too close interaction happens 
        int count = 0; //this couunts how many TOOCLOSE interactions an entity has, if it has more than one
                       //it adds the sum vector  


        foreach (GruntAI g in pool)
        {

            float d = Vector3.Distance(g.transform.position, transform.position);

            if (g.transform.position != transform.position && d < desiredSeperation)
            {
                Vector3 diff = transform.position - g.transform.position;
                diff.Normalize();
                sum += diff;
                count++;
            }

            if (count > 0)
            {
                sum /= count;
                sum.Normalize();
                sum *= maxSpeed;

                Vector3 steer = sum - rb.velocity;
                if (steer.magnitude > maxForce)
                {
                    steer.Normalize();
                    steer *= maxForce;
                }

                applyForce(steer);

            }

        }
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
    public void loadout(GameObject targ)//sets the target of the entity and equips the gun
    {
        target = targ;
        //myGun = gunToEquip;
    }

    #endregion & Setup
}
