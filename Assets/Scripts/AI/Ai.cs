using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void NotifyDeath();  // delegate

/// <summary>
/// This is the base AI class for all enemies.
/// </summary>
public abstract class Ai : SelfWorldBoundsDespawn
{

    // THis is the method that sets the entity to Deactive and bascially is uesd to kill the entitiy
    public void op_ProcessCompleted(SelfDespawn entity)
    {
        entity.gameObject.SetActive(false);
        //TODO: Add Logic here to make sure Entity either remains in the pool or becomes a new entity
    }

    #region Variables for Setup.

    public GameObject target;
    public CyborgAnimationStateController animationStateController;
    public Rigidbody rb;
    public Gun myGun;
    public Health hp;
    public List<Condition> activeConditions = new List<Condition>();

    public float StartingHP;
    public float CurrentHP; //For Debugging TODO: Add tool implementation
    public float maxSpeed;
    public float maxForce;
    public float score;
    public float attackRange;
    public bool alive;

    public event NotifyDeath DeadEvent; // event

    #endregion

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        SetAnimationSpeed(rb.velocity.magnitude);


        //update conditions
        foreach(Condition cond in activeConditions)
        {
            cond.Tick();
        }


        //Dead
        if (hp.HitPoints <= 0) //this signifies that the enemy Died and wasn't merely Despawned
        {
            Die();
        }
        else //Alive
        {
            CurrentHP = hp.HitPoints;
            if(target == null)
            {
                Wander();
            } else
            {
                Vector3 desiredVec = target.transform.position - transform.position;
                if(desiredVec.magnitude < attackRange)
                {

                    Move(this.transform.position);
                    Aim(target.transform.position);
                    Attack();
                    SetAnimationSpeed(0);
                } else
                {
                    Move(target.transform.position);
                    SetAnimationSpeed(rb.velocity.magnitude / 40); //this devides them by a constant to allow for slower enemies to walk slower.
                }
            }
        }

    }

    public void Aim(Vector3 aimAt)
    {
        transform.LookAt(aimAt);
    }

    /// <summary>
    /// This method plays a death animation and the deactivates the enemy
    /// </summary>
    public void Die()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;


        if (alive == true)
        {
            // Notify all listeners that this AI has died
            DeadEvent?.Invoke();


            animationStateController.TriggerDeathA();
            rb.detectCollisions = false;
            animationStateController.SetAlive(false);
            alive = false;


            if (myGun != null)
            {
                myGun.StopAllCoroutines();
            }

        }

    }
    /// <summary>
    /// This method is called when the entitiy wants to attack. Checks if it has a gun
    /// </summary>
    public void Attack()
    {
        if (myGun != null && myGun.CanShootAgain() && alive)
        {

            this.myGun.Shoot(target.transform.position);
            animationStateController.AimWhileWalking(true);

        }
    }


    /// <summary>
    /// This Metod is used to set the animation speed without causing errors or Ai that do not have an animation state controller.
    /// </summary>
    /// <param name="animationSpeed"></param>
    public virtual void SetAnimationSpeed(float animationSpeed)
    {
        if (animationStateController != null)
        {
            animationStateController.SetSpeed(animationSpeed);
        }
    }

    #region MOVEMENT
    /// <summary>
    /// This method works for ranged Enemies that do not get into direct melee range with the target
    /// </summary>
    /// <param name="target"> Vector to target </param>
    public virtual void Move(Vector3 target) //This can be used for Enemies that stay at range and dont run into melee.
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

            //Attack();
        }
            else
            {
                desiredVec *= maxSpeed;
            }
            Vector3 steer = desiredVec - rb.velocity; //Subtract Velocity so we are not constantly adding to the velocity of the Entity
            applyForce(steer);
    }

    /// <summary>
    /// This method is used for when an AI has no target and will move around in a Boid fashoion
    /// </summary>
    public void Wander() //cause the character to wander
    {

        Vector3 forward = rb.transform.forward; //The normaized vector of which direction the RB is facing
        Vector3 offset = new Vector3(0,0,1); //This is the random change vector that is uses to create natural wandering movement
        Quaternion ranRot = Quaternion.Euler(0, Random.Range(0, 359), 0);
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

    #endregion

    #region SEPERATE

    /// <summary>
    /// This method requires the entire of AI
    /// </summary>
    /// <param name="pool"></param> Pool is the grouping of all of the AI controlled entities in the boid that need to be seperateed from one another
    public void Seperate(List<Ai> pool) //this function will edit the steer of an AI so it moves away from nearby other AI
    {
        float desiredSeperation = 110;

        Vector3 sum = new Vector3(); //the vector that will be used to calculate flee beheavior if a too close interaction happens
        int count = 0; //this couunts how many TOOCLOSE interactions an entity has, if it has more than one
                       //it adds the sum vector


        foreach (Ai g in pool)
        {

            float d = Vector3.Distance(g.transform.position, transform.position);

             if (g.transform.position != transform.position && d < desiredSeperation)
            {
                Vector3 diff = transform.position - g.transform.position; // creats vec between two objects
                diff.Normalize();
                sum += diff; // sum is the flee direction added together
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
    public bool IsAlive()
    {
        return alive;
    }
    public float GetScore()
    {
        return score;
    }
    /// <summary>
    /// This method sets the target of the entity TODO: Will eventually equip a gun?
    /// </summary>
    /// <param name="targ"></param>
    public void SetTarget(GameObject targ)//sets the target of the entity and equips the gun
    {
        target = targ;
        //myGun = gunToEquip;
    }
    /// <summary>
    /// This method is called to reset the entity's health and alive status. Use every time they spawn.
    /// </summary>
    public void NewLife()
    {
        alive = true;
        hp.Init(StartingHP);
        animationStateController.SetAlive(true);
        rb.detectCollisions = true;
        rb.constraints = RigidbodyConstraints.None;
    }// this restets the enemies HP and sets them to alive;

    /// <summary>
    /// This method applies a condition to the AI (assuming it doesn't already have the condition)
    /// </summary>
    public void ApplyCondition(Condition cond)
    {
        if(!activeConditions.Contains(cond))
        {
            cond.SetHostAi(this);
            activeConditions.Add(cond);
        }
    }
    #endregion & Setup
}
