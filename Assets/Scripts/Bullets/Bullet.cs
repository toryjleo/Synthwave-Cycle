using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>Bullet</c> A Unity Component which moves a gameobject foreward.</summary>
public abstract class Bullet : SelfWorldBoundsDespawn
{

    protected Vector3 shootDir;
    protected Vector3 initialVelocity;

    // Specific to gun
    protected float muzzleVelocity = 0;
    protected float mass = 0;
    protected float boost = 1f;
    protected float damageDealt = 0;
    protected bool hasFiniteLifetime = false;
    protected bool overPenetrates = false;
    protected float lifetime = float.MaxValue;
    protected float timeSinceShot = 0;

    internal List<GameObject> alreadyHit = new List<GameObject>();

    /// <summary>Speed of bullet out of the gun.</summary>
    public float MuzzleVelocity
    {
        get => muzzleVelocity;
    }

    /// <summary>Mass of the bullet.</summary>
    public float Mass
    {
        get => mass;
    }

    /// <summary>Amount of additional force given by a bullet (make negative to dampen effect of a shot)</summary>
    public float Boost
    {
        get => boost;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        UpdateBulletLifeTime();
        Move();
    }
    /// <summary>Updates the object's location this frame.</summary>
    protected virtual void Move()
    {
        Vector3 distanceThisFrame = ((shootDir * muzzleVelocity) + initialVelocity) * Time.deltaTime;
        transform.position = transform.position + distanceThisFrame;
    }

    /// <summary>Code that applies changes to a bullet over its lifetime.</summary>
    protected virtual void UpdateBulletLifeTime() 
    {
        // Check if the bullet's lifetime is up
        if (hasFiniteLifetime)
        {
            timeSinceShot += Time.deltaTime;
            if (timeSinceShot >= lifetime) 
            {
                OnDespawn();
            }
        }
    }

    /// <summary>Initializes this bullet to start moving.</summary>
    /// <param name="curPosition">Location to start being shot from.</param>
    /// <param name="direction">Direction in which bullet will move.</param>
    /// <param name="initialVelocity">Velocity of the object shooting.</param>
    public virtual void Shoot(Vector3 curPosition, Vector3 direction, Vector3 initialVelocity)
    {
        transform.position = curPosition;
        direction.y = 0; // Do not travel vertically
        shootDir = direction.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        this.initialVelocity = initialVelocity;
        this.timeSinceShot = 0;
    }

    /// <summary>Deals damage to other and despawns this bullet.</summary>
    /// <param name="other">GameObject who we will deal damage to. Expects this GameObject to have a Health 
    /// component.</param>
    abstract internal void DealDamageAndDespawn(GameObject other);

    /// <summary>
    /// Resets bullet properties. This is used when a bullet is spawned from a pool to ensure it gets a fresh start
    /// </summary>
    public virtual void ResetBullet()
    {
        initialVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
