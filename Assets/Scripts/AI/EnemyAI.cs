using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : SelfDespawn
{

    private Vector3 targetVec; //this is the vector to their quarry 
    public GameObject target;
    public Rigidbody rb;
    public Gun myGun; 

    float maxSpeed;
    public float attackRange; //TODO: This will be set when creating different inherited classes for Monobehavior; 

    bool alive; 


    private void Awake()
    {
        alive = true;
        rb = GetComponent<Rigidbody>();
        //location = transform.position;
        maxSpeed = 40;

    }//we make sure it's alive and get the rigid body 

    public override void Init()
    {

    }

    public override void Update()
    {
        base.Update();
        targetVec = target.transform.position;

        arrive(targetVec);
    }



    /// <summary>
    /// This method requires the entire of AI 
    /// </summary>
    /// <param name="pool"></param>
    public void seperate(List<EnemyAI> pool) //this function will edit the steer of an AI so it moves away from nearby other AI 
    {
        float desiredSeperation = 5;

        Vector3 sum = new Vector3(); //the vector that will be used to calculate flee beheavior if a too close interaction happens 
        int count = 0; //this couunts how many TOOCLOSE interactions an entity has, if it has more than one
                       //it adds the sum vector  


        foreach (EnemyAI g in pool)
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
                //steer.limit(maxForce);
                applyForce(steer);

            }
            
        }
    }

    public bool isAlive()
    {
        return alive;
    }

    public void setUpEnemy(GameObject targ)//sets the target of the entity 
    {
        target = targ;
        //myGun = gunToEquip;
    }

    void applyForce(Vector3 force)
    {
        rb.AddForce(force);
        
    }

    #region Movement
    /// <summary>
    /// This method works for ranged Enemies that do not get into direct melee range with the target 
    /// </summary>
    /// <param name="target"> Vector to target </param>
    private void arrive(Vector3 target) //This can be used for Enemies that stay at range and dont run into melee. 
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

            this.myGun.Shoot(target);

            //TODO: the AI currently doesn't stand still when firing and I'm not sure if they should?

        } else { 
            desiredVec *= maxSpeed; 

            //walking speed is normal 
        }
        
        Vector3 steer = desiredVec - rb.velocity;
        //steer.limit(maxForce);

        

        applyForce(steer);
    } 


    /// <summary>
    /// The enemy will wander around aimlessly with disregard for the player 
    /// </summary>
    /// <param name="target"></param>
    private void wander(Vector3 target) //TODO: do this later 
    {

        //Vector3 desiredVec = forward;

        //float d = desiredVec.magnitude;




        
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0,359));
        //steer = rot * steer;



    }

    #endregion
}
