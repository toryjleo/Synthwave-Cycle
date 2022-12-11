using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player shotgun that has a low rate of fire and fires multiple pellets in various directions
/// As this gun levels up, the rate of fire steadily increases and it gets more pellets
/// </summary>
public class Shotty : LeveledGun
{
    public GameObject barrelL;
    public GameObject barrelR;
    public AudioSource muzzleAudio;

    float angleDifference = 5f;

    public override void BigBoom()
    {
        for(int i = 0; i < 60; i++)
        {
            Bullet bullet = bulletPool.SpawnFromPool();

            Vector3 shotDir = Quaternion.Euler(0, 180f * (i / 60f) , 0) * barrelL.transform.up;
            bullet.Shoot(barrelL.transform.position, shotDir, Vector3.zero);
            shotDir = Quaternion.Euler(0, 180f * (i / 60f) , 0) * barrelR.transform.up;
            bullet.Shoot(barrelR.transform.position, shotDir, Vector3.zero);
        }
    }

    public override PlayerWeaponType GetPlayerWeaponType()
    {
        return PlayerWeaponType.Shotty;
    }

    public override void Init()
    {
        base.Init();
        int currentLevel = GetCurrentLevel();
        fireRate = 0.5f * currentLevel;
        ammunition = 8 * currentLevel;
    }
    public override void PrimaryFire(Vector3 initialVelocity)
    {
        if(CanShootAgain())
        {
            lastFired = Time.time;
            int currentLevel = GetCurrentLevel();
            for (int i = 0; i < currentLevel * 4; i++)
            {
                Bullet bullet = bulletPool.SpawnFromPool();

                Vector3 shotDir;

                shotDir = Quaternion.Euler(0, Random.Range(-angleDifference, angleDifference), 0) * barrelL.transform.up;
                //shotDir = barrel.transform.up;

                bullet.Shoot(barrelL.transform.position, shotDir, initialVelocity);
                //muzzleAudio.Play();
                ApplyRecoil(shotDir, bullet);
            }
        }
    }
    public override void SecondaryFire(Vector3 initialVelocity)
    {
        if (CanShootAgain())
        {
            lastFired = Time.time;
            int currentLevel = GetCurrentLevel();
            for (int i = 0; i < currentLevel * 4; i++)
            {
                Bullet bullet = bulletPool.SpawnFromPool();

                Vector3 shotDir;

                shotDir = Quaternion.Euler(0, Random.Range(-angleDifference, angleDifference), 0) * barrelR.transform.up;
                //shotDir = barrel.transform.up;

                bullet.Shoot(barrelR.transform.position, shotDir, initialVelocity);
                //muzzleAudio.Play();
                ApplyRecoil(shotDir, bullet);
            }
        }
    }
}
