using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void NotifyShot(Vector3 force);  // delegate
public delegate void NotifyOutOfAmmo(Gun self);

/// <summary>Class <c>Bullet</c> A Unity Component which spawns bullets.</summary>
public abstract class Gun : Weapon
{
    public Bullet bulletPrefab;
    protected BulletPool bulletPool;
    protected float lastFired;
    protected float fireRate = 0;  // The number of bullets fired per second
    protected int ammunition;
    public bool infiniteAmmo = false;
    public int bulletPoolSize = 100;

    public event NotifyShot BulletShot; // event
    public event NotifyOutOfAmmo OutOfAmmo; // event

    /// <summary>Initializes veriables. Specifically must initialize lastFired and fireRate variables.</summary>
    public override void Init()
    {
        bulletPool = gameObject.GetComponent<BulletPool>();
        if (bulletPool == null)
        {
            Debug.Log("Creating bullet pool");
            bulletPool = gameObject.AddComponent<BulletPool>();
        }
        bulletPool.Init(bulletPrefab, bulletPoolSize);
        lastFired = 0;
    }

    /// <summary>Basically a destructor. Calls bulletPool.DeInit().</summary>
    public override void DeInit() 
    {
        bulletPool.DeInit();
    }
    
    /// <summary>Calls OnBulletShot. Will apply variable recoil based on bullet's mass and muzzle velocity</summary>
    /// <param name="directionOfBullet">Non-normalized direction bullet is travelling.</param>
    /// <param name="bulletShot">The bullet that is currently being shot.</param>
    protected void ApplyRecoil(Vector3 directionOfBullet, Bullet bulletShot) 
    {
        OnBulletShot(directionOfBullet.normalized * bulletShot.Mass * bulletShot.MuzzleVelocity * bulletShot.Boost);
    }


    /// <summary>Notifies listeners that a bullet has been shot.</summary>
    /// <param name="forceOfBullet">The force of the actor on the bullet.</param>
    protected virtual void OnBulletShot(Vector3 forceOfBullet) //protected virtual method
    {
        //if BulletShot is not null then call delegate
        BulletShot?.Invoke(-forceOfBullet);
        if (!infiniteAmmo)
        {
            ammunition--;
        }
        if(!infiniteAmmo && ammunition <= 0)
        {
            OutOfAmmo?.Invoke(this);
        }
    }

    /// <summary>Will return true if the gun's cooldown has happened.</summary>
    /// <returns>True if the gun can shoot again.</returns>
    public bool CanShootAgain() 
    {
        return Time.time - lastFired > 1 / fireRate;
    }
}
