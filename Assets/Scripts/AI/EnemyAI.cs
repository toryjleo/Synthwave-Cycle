using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    private Vector3 targetVec; //this is the vector to their quarry 
    private Vector3 location; //this is the vector of the entity 
    private Vector3 forward;
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

    
   

    // Update is called once per frame
    void Update()
    {

        targetVec = target.transform.position;
        //location = transform.position;
        //seek(targetVec);
        arrive(targetVec);
        //wander();
           
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
            
            float d = Vector3.Distance(g.location, transform.position);
            


            if (g.location != location && d < desiredSeperation)
            {
                
                print("TOO CLOSE");
                Vector3 diff = location - g.location;
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
    public Vector3 getPosition()
    {
        return transform.position;
    }

    void applyForce(Vector3 force)
    {
        rb.AddForce(force);
        
    }

    
    /// <summary>
    /// These Movement Methods SEEK ARRIVE WANDER are going to evenetually be seperated out into different movement methods 
    /// for Child classes of EnemyAI, currently SEEK and WANDER are not in use however 
    /// </summary>
    /// <param name="target"> target is the vector leading to the player </param>
    private void seek(Vector3 target)
    {
        //steering force = desired velocity - current velocity 
        //desired velocity = vector to player  
        //
        //the enemy moves towards the player at maximum speed
        //by normailizing the vector we can take into account the speed 

        Vector3 desiredVec = target - location;
        desiredVec.Normalize();
        desiredVec *= maxSpeed;

        Vector3 steer = desiredVec - rb.velocity;
        
        //steer.limit(maxForce);
        applyForce(steer); 
    } // simpler function to just chase directly after the player 


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

        if (dMag < maxSpeed)
        {
            desiredVec *= dMag;
        } else { 
            desiredVec *= maxSpeed; 
        }
        
        Vector3 steer = desiredVec - rb.velocity;
        //steer.limit(maxForce);

        this.myGun.Shoot(target); 

        applyForce(steer);
    } 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="traget"></param>
    private void wander(Vector3 traget
        ) //do this later 
    {

        Vector3 desiredVec = forward;

        float d = desiredVec.magnitude;




        
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0,359));
        //steer = rot * steer;



    }


}