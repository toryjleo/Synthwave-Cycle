using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : AiTemplate
{


 
    public override void Init()
    {
        alive = true;
        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        animationStateController = GetComponent<CyborgAnimationStateController>();
        //location = transform.position;
        maxSpeed = 40; 
        maxForce = 1;
        
        StartingHP = 40;
        score = 100;
        hp.Init(StartingHP);

        

        if (animationStateController == null)
        {
            Debug.LogError("This object needs a CyborgAnimationStateController component");
        }
    }

    void Awake()
    {
        Init();
    }

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
        } else
        {
            Move(target.transform.position);
        }
        
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
                if(steer.magnitude > maxForce)
                {
                    steer.Normalize();
                    steer *= maxForce;
                }
                
                applyForce(steer);

            }
            
        }
    }


    


}
