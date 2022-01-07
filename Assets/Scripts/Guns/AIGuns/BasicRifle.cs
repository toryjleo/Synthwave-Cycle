using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRifle : Gun
{
    private float timeBetweenTripleShot = .07f;

    public override void Init()
    {
        lastFired = 0;
        fireRate = .5f; // Every 2 seconds
        bulletPool = gameObject.AddComponent<BulletPool>();
        bulletPool.Init(bulletPrefab);
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
        int numberOfShots = 3;
        for (int i = 0; i < numberOfShots; i++)
        {
            Debug.Log("SHOT!");
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
