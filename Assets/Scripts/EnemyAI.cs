using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public Vector3 targetVec;
    public Vector3 location;
    public GameObject target;
    public Rigidbody rb;

    float maxSpeed;
    float maxForce;
    public float attackRange; 

    bool alive; 
    // Start is called before the first frame update
    void Start()
    {
        //action selection 
        //movement 
        //locomotion 

        location = transform.position;
        maxSpeed = 10;
        
    }

    private void Awake()
    {
        alive = true;
        rb = GetComponent<Rigidbody>();
    }

    
   

    // Update is called once per frame
    void Update()
    {

        targetVec = target.transform.position;
        location = transform.position;
        //seek(targetVec);
        arrive(targetVec);
           
    }

    public bool isAlive()
    {
        return alive;
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
    }

    private void arrive(Vector3 target) //This can be used for Enemies that stay at range and dont run into melee. 
    {

        Vector3 desiredVec = target - location; 
        
        float dMag = desiredVec.magnitude;
        dMag -= attackRange; // dmag is the distance between the two objects, by subtracking this, I make it so the object doesn't desire to move as far.  

        desiredVec.Normalize();

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





}
