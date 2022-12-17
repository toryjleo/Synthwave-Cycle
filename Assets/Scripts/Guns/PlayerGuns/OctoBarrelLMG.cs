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

    // Determines rotation movement of barrels
    [SerializeField] private bool rotateOutward = true;
    private const float MIN_ROTATION_ANGLE = 90.0f;
    private const float MAX_OUTWARD_ROTATION_AMOUNT = 140.0f;
    private float currentRotationFromMin = 0.0f;
    private float rotationRate = 70f;
    private bool secondaryFireJustHit = false;

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
        ResetRotation();
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

    public override void ReleasePrimaryFire(Vector3 initialVelocity)
    {
    }

    /// <summary>
    /// Rotates the barrels of this gun
    /// </summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public override void SecondaryFire(Vector3 initialVelocity)
    {
        secondaryFireJustHit = true;
        float rotationThisFrame = Time.deltaTime * rotationRate;

        if (rotateOutward) 
        {
            currentRotationFromMin += rotationThisFrame;
            // Have met maximum rotation
            if (currentRotationFromMin > MAX_OUTWARD_ROTATION_AMOUNT) 
            {
                rotateOutward = false;
                currentRotationFromMin = MAX_OUTWARD_ROTATION_AMOUNT;
            }
        }
        else 
        {
            currentRotationFromMin -= rotationThisFrame;
            // Have met maximum rotation
            if (currentRotationFromMin < 0)
            {
                rotateOutward = true;
                currentRotationFromMin = 0;
            }
        }

        // Finally, update the rotation
        muzzle1.transform.localRotation = Quaternion.AngleAxis(-currentRotationFromMin + MIN_ROTATION_ANGLE, Vector3.up);
        muzzle2.transform.localRotation = Quaternion.AngleAxis(currentRotationFromMin + MIN_ROTATION_ANGLE, Vector3.up);
    }

    /// <summary>
    /// Swaps the rotation direction of the barrels
    /// </summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public override void ReleaseSecondaryFire(Vector3 initialVelocity)
    {
        if (secondaryFireJustHit) 
        {
            rotateOutward = !rotateOutward;
            secondaryFireJustHit = false;
        }
    }

    /// <summary>
    /// Sets the rotation so both barrels are pointed forward
    /// </summary>
    private void ResetRotation()
    {
        muzzle1.transform.rotation = Quaternion.AngleAxis(MIN_ROTATION_ANGLE, Vector3.up);
        muzzle2.transform.rotation = Quaternion.AngleAxis(MIN_ROTATION_ANGLE, Vector3.up);
        rotateOutward = true;
        currentRotationFromMin = 0.0f;
    }
}
