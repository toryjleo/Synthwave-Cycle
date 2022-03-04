using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ai : SelfDespawn
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
                Wander();
            } else
            {
                Move(target.transform.position);
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

    // THis is the method that sets the entity to Deactive and bascially is uesd to kill the entitiy 
    public void op_ProcessCompleted(SelfDespawn entity)
    {
        entity.gameObject.SetActive(false);
        //TODO: Add Logic here to make sure Entity either remains in the pool or becomes a new entity
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
                Attack();
            }
            else
            {
                desiredVec *= maxSpeed;
            }
            Vector3 steer = desiredVec - rb.velocity; //Subtract Velocity so we are not constantly adding to the velocity of the Entity
            applyForce(steer);
    }

    public void Wander() //cause the character to wander 
    {

        Vector3 forward = rb.transform.forward; //The normaized vector of which direction the RB is facing 
        Vector3 offset = new Vector3(0,0,1); //This is the random change vector that is uses to create natural wandering movement
        Quaternion ranRot = Quaternion.Euler(0, Random.Range(0, 359), 0);
        Quaternion right = Quaternion.Euler(0, 90, 0);
        forward *= 10;
        offset = ranRot * offset; 


        Debug.DrawRay(rb.transform.position, forward, Color.blue);
        Debug.DrawRay(rb.transform.position+forward, offset, Color.red);
        Debug.DrawRay(rb.transform.position, forward + offset, Color.green);

        forward += offset; //adds a small offset to the forward vector. 

        transform.LookAt(forward+transform.position); //TODO make this look way nicer 

        Vector3 steer = forward - rb.velocity; //Subtract Velocity so we are not constantly adding to the velocity of the Entity
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
    public void seperate(List<Ai> pool) //this function will edit the steer of an AI so it moves away from nearby other AI 
    {
        float desiredSeperation = 5;

        Vector3 sum = new Vector3(); //the vector that will be used to calculate flee beheavior if a too close interaction happens 
        int count = 0; //this couunts how many TOOCLOSE interactions an entity has, if it has more than one
                       //it adds the sum vector  


        foreach (Ai g in pool)
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
