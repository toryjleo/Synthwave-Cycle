using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>A big ass rip-off of the double barrel LMG</summary>
public class OctoBarrelLMG : LeveledGun
{
    
    // Very specific to this gun
    public GameObject muzzle1;
    public GameObject muzzle2;
    public AudioSource muzzle1Audio;
    public AudioSource muzzle2Audio;
    private bool muzzle1Turn = true;

    public override PlayerGunType GetPlayerGunType()
    {
        return PlayerGunType.OctoLMG;
    }

    public override void Init() 
    {
        base.Init();
        int currentLevel = GetCurrentLevel();
        fireRate = 60 * currentLevel;
        ammunition = 50 * currentLevel;
    }

    /// <summary>Fires a bullet out of either muzzle, alternating each turn.</summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public override void Shoot(Vector3 initialVelocity) 
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
}
