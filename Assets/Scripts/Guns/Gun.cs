using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void NotifyShot(Vector3 force);  // delegate

public abstract class Gun : MonoBehaviour
{

    public Bullet bulletPrefab;
    protected static BulletPool bulletPool;
    protected float lastFired = 0;
    protected float fireRate = 0;  // The number of bullets fired per second


    public event NotifyShot BulletShot; // event


    protected virtual void Awake()
    {
        Init();
    }

    /// <summary>Initializes veriables. Specifically must initialize lastFired and fireRate variables.</summary>
    public abstract void Init();

    /// <summary>Fires the bullet from the muzzle of the gun. Is responsible for calling OnBulletShot and getting bullet from the object pool.</summary>
    /// <param name="initialDirection">The velocity of the gun when the bullet is shot.</param>
    public abstract void Shoot(Vector3 initialVelocity);

    /// <summary>Notifies listeners that a bullet has been shot.</summary>
    /// <param name="forceOfBullet">The force of the actor on the bullet.</param>
    protected virtual void OnBulletShot(Vector3 forceOfBullet) //protected virtual method
    {
        //if BulletShot is not null then call delegate
        BulletShot?.Invoke(-forceOfBullet);
    }

    protected bool CanShootAgain() 
    {
        return Time.time - lastFired > 1 / fireRate;
    }
}
