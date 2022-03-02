using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShottyGun : Gun
{
    public GameObject barrel;
    const int PELLETS_PER_SHOT = 8;
    const float MAX_SPREAD = 90f/360;
    const float MAX_CHANGE_IN_MUZZLE_VELOCITY = 0.5f;
    public override void Init() 
    {
        lastFired = 0;
        fireRate = 2;
        bulletPool = gameObject.AddComponent<BulletPool>();
        bulletPool.Init(bulletPrefab);
    }

    /// <summary>Fires a spread of pellets </summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public override void Shoot(Vector3 initialVelocity) 
    {
        if (CanShootAgain())
        {
            lastFired = Time.time;
            for(int i = 0; i < PELLETS_PER_SHOT; i++)
            {
                Bullet bullet = bulletPool.SpawnFromPool();
                Vector3 shotDir = barrel.transform.up;
                //give every pellet a slight variance in angle
                shotDir.z += Random.Range(-(MAX_SPREAD / 2), MAX_SPREAD / 2);
                bullet.Shoot(barrel.transform.position, shotDir, initialVelocity);
                //give every pellet a slight variance in exit velocity
                bullet.MuzzleVelocity = bullet.MuzzleVelocity * 
                    (1 - Random.Range(-MAX_CHANGE_IN_MUZZLE_VELOCITY/2, MAX_CHANGE_IN_MUZZLE_VELOCITY / 2));
                OnBulletShot(shotDir * bullet.Mass * bullet.MuzzleVelocity);
            }
        }
    }
}
