using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BasicRifleBullet</c> Basic enemy bullet.</summary>
public class TurretBullet : Bullet
{
    public override void Init()
    {
        muzzleVelocity = 60;
        mass = .5f;
        damageDealt = 60;
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Enemy")
        {
            // TracerMesh should have a Health component
            Health otherHealth = other.GetComponentInChildren<Health>();
            float z = otherHealth.HitPoints;
            otherHealth.TakeDamage(damageDealt);

        }

    }
}