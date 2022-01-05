using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRifle : Gun
{
    private float timeBetweenTripleShot = .3f;

    public override void Init()
    {
        lastFired = 0;
        fireRate = .5f; // Every 2 seconds
        bulletPool = new BulletPool(bulletPrefab);
    }

    public override void Shoot(Vector3 initialVelocity)
    {
        if (CanShootAgain()) 
        {
            lastFired = Time.time;
            //Bullet bullet = bulletPool.SpawnFromPool();

            StartCoroutine(TripleShot());

            //OnBulletShot(shotDir * bullet.Mass * bullet.muzzleVelocity);
        }
    }

    IEnumerator TripleShot() 
    {
        Debug.Log("SHOT!");
        yield return new WaitForSeconds(timeBetweenTripleShot);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.M)) 
        {
            //Shoot(gameObject.transform.forward);
        }
    }
}
