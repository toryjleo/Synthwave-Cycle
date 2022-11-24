using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player shotgun that has a low rate of fire and fires multiple pellets in various directions
/// As this gun levels up, the rate of fire steadily increases and it gets more pellets
/// </summary>
public class Shotty : LeveledGun
{
    public GameObject barrel;
    public AudioSource muzzleAudio;

    float angleDifference = 5f;

    public override void Init()
    {
        base.Init();
        int currentLevel = GetCurrentLevel();
        fireRate = 0.5f * currentLevel;
        ammunition = 8 * currentLevel;
    }
    public override void Shoot(Vector3 initialVelocity)
    {
        if(CanShootAgain())
        {
            lastFired = Time.time;
            int currentLevel = GetCurrentLevel();
            for (int i = 0; i < currentLevel * 4; i++)
            {
                Bullet bullet = bulletPool.SpawnFromPool();

                Vector3 shotDir;

                shotDir = Quaternion.Euler(0, Random.Range(-angleDifference, angleDifference), 0) * barrel.transform.up;
                //shotDir = barrel.transform.up;

                bullet.Shoot(barrel.transform.position, shotDir, initialVelocity);
                //muzzleAudio.Play();
                ApplyRecoil(shotDir, bullet);
            }
        }
    }
}
