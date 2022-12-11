using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AkimboRangers : Gun
{
    private float timeBetweenBlastWaves = .02f;

    public override void Init()
    {
        lastFired = 0;
        fireRate = 2f; // Every 1 second
        bulletPool = gameObject.AddComponent<BulletPool>();
        bulletPool.Init(bulletPrefab);
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



    IEnumerator SpreadShot()
    {
        int numberOfShots = 1;
        int numberOfPellets = 4;

        int degreeOff = -4;

        Vector3 shotDir = gameObject.transform.forward;
        Vector3 bulletPosition = transform.position;
        Vector3 initialVelocity = Vector3.zero;

        for (int i = 0; i < numberOfShots; i++)
        {
            //Debug.Log("SHOT!");

            for (int p = 0; p < numberOfPellets; p++)
            {
                Bullet bullet = bulletPool.SpawnFromPool();
                bullet.Shoot(bulletPosition, Quaternion.Euler(0, degreeOff, 0) * shotDir, initialVelocity);
                degreeOff += 2;
            }
            numberOfPellets--;

            degreeOff = -7;


            yield return new WaitForSeconds(timeBetweenBlastWaves);
        }
        yield return null;
    }

    public void stopFiring()
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

    public override PlayerWeaponType GetPlayerWeaponType()
    {
        return PlayerWeaponType.INVALID;
    }

    public override void SecondaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }
}
