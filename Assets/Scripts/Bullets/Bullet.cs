using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>Bullet</c> A Unity Component which moves a gameobject foreward.</summary>
public abstract class Bullet : SelfDespawn
{

    private Vector3 shootDir;
    private Vector3 initialVelocity;

    // Specific to gun
    protected float muzzleVelocity = 0;
    protected float mass = 0;
    protected float damageDealt = 0;

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

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        Move();
    }

    /// <summary>Updates the object's location this frame.</summary>
    private void Move()
    {
        Vector3 distanceThisFrame = ((shootDir * muzzleVelocity) + initialVelocity) * Time.deltaTime;
        transform.position = transform.position + distanceThisFrame;
    }

    /// <summary>Initializes this bullet to start moving.</summary>
    /// <param name="curPosition">Location to start being shot from.</param>
    /// <param name="direction">Direction in which bullet will move.</param>
    /// <param name="initialVelocity">Velocity of the object shooting.</param>
    public void Shoot(Vector3 curPosition, Vector3 direction, Vector3 initialVelocity)
    {
        transform.position = curPosition;
        direction.y = 0; // Do not travel vertically
        shootDir = direction.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        this.initialVelocity = initialVelocity;
    }

    /// <summary>Deals damage to other and despawns this bullet.</summary>
    /// <param name="other">GameObject who we will deal damage to. Expects this GameObject to have a Health 
    /// component.</param>
    protected void DealDamageAndDespawn(GameObject other) 
    {
        Health otherHealth = other.GetComponentInChildren<Health>();
        if (otherHealth == null) 
        {
            Debug.LogError("Object does not have Health component: " + gameObject.name);
        }
        otherHealth.TakeDamage(damageDealt);
        OnDespawn();
    }

}
