using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Spits out balls of acid that lowers an enemiy's health</summary>
public class AcidSpitter : Gun
{
    public GameObject cannon;
    public AudioSource cannonAudio;

    public override void Init()
    {
        fireRate = 10;
        bulletPool = gameObject.AddComponent<BulletPool>();
        bulletPool.Init(bulletPrefab);
        ammunition = 30;
    }

    /// <summary>Launches an acid ball</summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public override void Shoot(Vector3 initialVelocity)
    {
        if (CanShootAgain())
        {
            lastFired = Time.time;
            Bullet bullet = bulletPool.SpawnFromPool();
            Vector3 shotDir = cannon.transform.forward;
            bullet.Shoot(cannon.transform.position, shotDir, initialVelocity);
            cannonAudio.Play();
            ApplyRecoil(shotDir, bullet);
        }
    }
}
