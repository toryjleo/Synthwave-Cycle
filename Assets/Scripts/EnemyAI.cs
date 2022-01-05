using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public Vector3 targetVec;
    public Vector3 location;
    public Vector3 velocity;
    public Vector3 acceleration;
    public GameObject target;
    public Rigidbody rb;

    float maxSpeed;
    float maxForce;
    bool alive; 
    // Start is called before the first frame update
    void Start()
    {
        //action selection 
        //movement 
        //locomotion 

        location = transform.position;
        maxSpeed = 2;
        
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
        velocity += acceleration;
        //velocity.limit(maxSpeed);
        location += velocity;
        acceleration *= 0;


        

        seek(targetVec);

           
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

        Vector3 steer = desiredVec - velocity;

        

        //steer.limit(maxForce);

        applyForce(steer); 

        
    }


}
