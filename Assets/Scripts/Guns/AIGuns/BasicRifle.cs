using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BulletPool</c> A unity component which acts as the basic enemy 3-shot gun.</summary>
public class BasicRifle : Gun
{
    private float timeBetweenTripleShot = .2f;

    public override void Init()
    {
        lastFired = 0;
        fireRate = .25f; // Every 2 seconds
        bulletPrefab.sender = this.gameObject.transform.parent.gameObject;
        bulletPool = gameObject.AddComponent<BulletPool>();
        bulletPool.Init(bulletPrefab);
    }

    /// <summary>Fires a sequence of 3 bullets.</summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
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

    /// <summary>Will wait some time before firing the next bullet.</summary>
    IEnumerator TripleShot() 
    {
        int numberOfShots = 3;
        for (int i = 0; i < numberOfShots; i++)
        {
            //Debug.Log("SHOT!");
            Bullet bullet = bulletPool.SpawnFromPool();
            Vector3 shotDir = gameObject.transform.forward;
            Vector3 bulletPosition = transform.position;
            Vector3 initialVelocity = Vector3.zero;
            bullet.Shoot(bulletPosition, shotDir, initialVelocity);
            yield return new WaitForSeconds(timeBetweenTripleShot);
        }
        yield return null;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.M)) 
        {
            Shoot(Vector3.zero);
        }
    }
}
