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

    float maxSpeed;
    public float attackRange; 

    bool alive;


    private void Awake()
    {
        alive = true;
        rb = GetComponent<Rigidbody>();
        location = transform.position;
        maxSpeed = 100;

    }//we make sure it's alive and get the rigid body 

    
   

    // Update is called once per frame
    void Update()
    {

        targetVec = target.transform.position;
        location = transform.position;
        //seek(targetVec);
        arrive(targetVec);
        //wander();
           
    }

    public void seperate(List<EnemyAI> pool) //this function will edit the steer of an AI so it moves away from nearby other AI 
    {
        float desiredSeperation = 20;


        foreach(EnemyAI g in pool)
        {
            
            float d = Vector3.Distance(g.location, transform.position);

            if (g.location != location && d < desiredSeperation)
            {
                desiredSeperation = 20;
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
        

    }
    public Vector3 getPosition()
    {
        return location;
    }

    void applyForce(Vector3 force)
    {
        rb.AddForce(force);
        
    }

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

    private void arrive(Vector3 target) //This can be used for Enemies that stay at range and dont run into melee. 
    {

        Vector3 desiredVec = target - location; //this logic creates the vector between where the entity is and where it wants to be 
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
        
        applyForce(steer);
    } 

    private void wander() //do this later 
    {

        Vector3 desiredVec = forward;

        float d = desiredVec.magnitude;




        
        Quaternion rot = Quaternion.Euler(0, 0, Random.RandomRange(0,359));
        //steer = rot * steer;



    }


}
