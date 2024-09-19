using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>Bullet</c> A Unity Component which moves a gameobject foreward.</summary>
public class Bullet : Gun.PoolableGunObject
{

    protected Vector3 shootDir;
    protected Vector3 initialVelocity;

    // Specific to gun
    protected bool isPlayerBullet = true;
    protected float muzzleVelocity = 0;
    protected float damageDealt = 0;
    protected bool overPenetrates = false;

    internal List<GameObject> alreadyHit = new List<GameObject>();

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        Move();
    }

    public override void Init(EditorObject.GunStats gunStats)
    {
        this.damageDealt = gunStats.DamageDealt;
        this.muzzleVelocity = gunStats.MuzzleVelocity;
        this.isPlayerBullet = gunStats.IsPlayerGun;
    }

    /// <summary>Updates the object's location this frame.</summary>
    protected virtual void Move()
    {
        Vector3 distanceThisFrame = ((shootDir * muzzleVelocity) + initialVelocity) * Time.deltaTime;
        transform.position = transform.position + distanceThisFrame;
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
        this.timeInWorld = 0;
    }

    /// <summary>
    /// Resets bullet properties. This is used when a bullet is spawned from a pool to ensure it gets a fresh start
    /// </summary>
    public override void Reset()
    {
        initialVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void DealDamageAndDespawn(GameObject other)
    {
        if (!alreadyHit.Contains(other))
        {
            alreadyHit.Add(other);
            Health otherHealth = other.GetComponentInChildren<Health>();
            if (otherHealth == null)
            {
                Debug.LogError("Object does not have Health component: " + gameObject.name);
            }
            else
            {
                otherHealth.TakeDamage(damageDealt);
            }
            if (!overPenetrates)
            {
                OnDespawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if ((other.gameObject.tag == "Enemy" && isPlayerBullet) ||
            (other.gameObject.tag == "Player" && !isPlayerBullet))
        {
            DealDamageAndDespawn(other.gameObject);
        }
    }
}
