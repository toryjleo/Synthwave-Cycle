using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    private float timeBetweenBlastWaves = .06f;

    public override void Init()
    {
        lastFired = 0;
        fireRate = .7f; // Every 2 seconds
        bulletPool = gameObject.AddComponent<BulletPool>();
        bulletPool.Init(bulletPrefab);
    }

    public override void Shoot(Vector3 initialVelocity)
    {
        if (CanShootAgain())
        {
            lastFired = Time.time;
            //Bullet bullet = bulletPool.SpawnFromPool();

            StartCoroutine(SpreadShot());

            //OnBulletShot(shotDir * bullet.Mass * bullet.muzzleVelocity);
        }
    }

    

    IEnumerator SpreadShot()
    {
        int numberOfShots = 2;
        int numberOfPellets = 8;

        int degreeOff = -8;

        Vector3 shotDir = gameObject.transform.forward;
        Vector3 bulletPosition = transform.position;
        Vector3 initialVelocity = Vector3.zero;

        for (int i = 0; i < numberOfShots; i++)
        {
            //Debug.Log("SHOT!");

            for(int p = 0; p < numberOfPellets; p++)
            {
                Bullet bullet = bulletPool.SpawnFromPool();               
                bullet.Shoot(bulletPosition, Quaternion.Euler(0, degreeOff, 0)*shotDir, initialVelocity);
                degreeOff += 2;
            }
            numberOfPellets--;

            degreeOff = -7;
          
            
            yield return new WaitForSeconds(timeBetweenBlastWaves);
        }
        yield return null;
    }

    public  void stopFiring()
    {
        StopCoroutine("SpreadShot");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Shoot(Vector3.zero);
        }
    }
}
