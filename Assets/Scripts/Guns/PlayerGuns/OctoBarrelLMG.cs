using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>A big ass rip-off of the double barrel LMG</summary>
public class OctoBarrelLMG : Gun
{

    // Very specific to this gun
    public GameObject muzzle1;
    public GameObject muzzle2;
    public AudioSource muzzle1Audio;
    public AudioSource muzzle2Audio;
    private bool muzzle1Turn = true;

    public override void BigBoom()
    {
        for (int i = 0; i < 60; i++)
        {
            Bullet bullet = bulletPool.SpawnFromPool();

            GameObject curMuzzle = i % 2 == 0 ? muzzle1 : muzzle2;
            Vector3 shotDir = Quaternion.Euler(0, 360f * (i / 60f), 0) * curMuzzle.transform.forward;
            //shotDir = barrel.transform.up;

            bullet.Shoot(curMuzzle.transform.position, shotDir, Vector3.zero);
        }
    }

    public override PlayerWeaponType GetPlayerWeaponType()
    {
        return PlayerWeaponType.OctoLMG;
    }

    public override void Init()
    {
        bulletPoolSize = 200;
        base.Init();
        int currentLevel = GetCurrentLevel();
        fireRate = 60 * currentLevel;
        ammunition = 50 * currentLevel;
    }

    /// <summary>Fires a bullet out of either muzzle, alternating each turn.</summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public override void PrimaryFire(Vector3 initialVelocity)
    {
        if (CanShootAgain())
        {
            lastFired = Time.time;
            Bullet bullet = bulletPool.SpawnFromPool();

            Vector3 shotDir;

            // Gun specific
            if (muzzle1Turn)
            {
                shotDir = muzzle1.transform.forward;
                bullet.Shoot(muzzle1.transform.position, shotDir, initialVelocity);
                muzzle1Audio.Play();
            }
            else
            {
                shotDir = muzzle2.transform.forward;
                bullet.Shoot(muzzle2.transform.position, shotDir, initialVelocity);
                muzzle2Audio.Play();
            }
            muzzle1Turn = !muzzle1Turn;
            ApplyRecoil(shotDir, bullet);
            //OnBulletShot(shotDir * bullet.Mass * bullet.MuzzleVelocity);
        }
    }
    //TODO: Implement secondary fire
    public override void SecondaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }
}
