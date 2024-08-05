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
        base.Init();
    }

    public override void PrimaryFire(Vector3 initialVelocity)
    {
        if (CanShootAgain())
        {
            lastFired = Time.time;
            //Bullet bullet = bulletPool.SpawnFromPool();

            StartCoroutine(SpreadShot());

            //OnBulletShot(shotDir * bullet.Mass * bullet.muzzleVelocity);
        }
    }


    public override void ReleasePrimaryFire(Vector3 initialVelocity)
    {
    }

    public override void SecondaryFire(Vector3 initialVelocity)
    {
    }


    public override void ReleaseSecondaryFire(Vector3 initialVelocity)
    {
    }

    public override PlayerWeaponType GetPlayerWeaponType()
    {
        return PlayerWeaponType.INVALID;
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
            PrimaryFire(Vector3.zero);
        }
    }

}
