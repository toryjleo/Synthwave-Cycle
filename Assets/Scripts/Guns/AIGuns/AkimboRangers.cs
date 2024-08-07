using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AkimboRangers : Gun
{
    public override void Init()
    {
        lastFired = 0;
        fireRate = 6f; // Every 1 second
        base.Init();
    }

    public override void PrimaryFire(Vector3 initialVelocity)
    {
        if (CanShootAgain())
        {
            lastFired = Time.time;
            StartCoroutine(SprayShot());
        }
    }

    public override void ReleasePrimaryFire(Vector3 initialVelocity)
    {
    }

    //Only fires a single shot, but the time between firing for a ranger is very short
    IEnumerator SprayShot()
    {
        Bullet bullet = bulletPool.SpawnFromPool();
        Vector3 shotDir = gameObject.transform.forward;
        Vector3 bulletPosition = transform.position;
        Vector3 initialVelocity = Vector3.zero;
        bullet.Shoot(bulletPosition, shotDir, initialVelocity);
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


    public override void ReleaseSecondaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }
}
