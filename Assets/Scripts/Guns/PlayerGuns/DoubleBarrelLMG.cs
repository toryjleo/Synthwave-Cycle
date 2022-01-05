using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoubleBarrelLMG : Gun
{

    // Very specific to this gun
    public GameObject muzzle1;
    public GameObject muzzle2;
    private bool muzzle1Turn = true;

    public override void Init() 
    {
        lastFired = 0;
        fireRate = 15;
        bulletPool = new BulletPool(bulletPrefab);
    }

    public override void Shoot(Vector3 initialDirection) 
    {
        if (Time.time - lastFired > 1 / fireRate)
        {
            lastFired = Time.time;
            Bullet bullet = bulletPool.SpawnFromPool();

            Vector3 shotDir;

            // Gun specific
            if (muzzle1Turn)
            {
                shotDir = muzzle1.transform.forward;
                bullet.Shoot(muzzle1.transform.position, shotDir, initialDirection);
            }
            else
            {
                shotDir = muzzle2.transform.forward;
                bullet.Shoot(muzzle2.transform.position, shotDir, initialDirection);
            }
            muzzle1Turn = !muzzle1Turn;
            OnBulletShot(shotDir * bullet.Mass * bullet.muzzleVelocity);
        }
    }
}
