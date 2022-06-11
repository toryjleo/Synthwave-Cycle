using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailBlazer : Gun
{
    // Very specific to this gun
    public GameObject exhaust;

    public override void Init()
    {
        lastFired = 0;
        fireRate = 10;
        ammunition = 400;
        bulletPool = gameObject.AddComponent<BulletPool>();
        bulletPool.Init(bulletPrefab);
    }

    /// <summary>Fires a bullet out of either muzzle, alternating each turn.</summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public override void Shoot(Vector3 initialVelocity)
    {
        if (CanShootAgain())
        {
            lastFired = Time.time;
            Bullet bullet = bulletPool.SpawnFromPool();

            Vector3 shotDir;
            shotDir = exhaust.transform.right;
            bullet.Shoot(exhaust.transform.position, shotDir, initialVelocity);
            ApplyRecoil(shotDir, bullet);
        }
    }

}
