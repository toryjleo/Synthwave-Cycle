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
    protected bool willPenetrate = false;

    // Determines who the bullet can effect ("Player", "Enemy", etc.)
    public List<string> targetTags = new List<string>();

    /// <summary>Speed of bullet out of the gun.</summary>
    public float MuzzleVelocity
    {
        get => muzzleVelocity;
        set => muzzleVelocity = value;
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
    private void OnTriggerEnter(Collider other)
    {
        if (targetTags.Contains(other.gameObject.tag))
        {
            // TracerMesh should have a Health component
            Health otherHealth = other.GetComponentInChildren<Health>();
            otherHealth.TakeDamage(damageDealt);
            if(!willPenetrate)
            {
                this.OnDespawn();
            }
        }
    }
}
