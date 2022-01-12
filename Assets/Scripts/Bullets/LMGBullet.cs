using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LMGBullet : Bullet
{
    public override void Init() 
    {
        muzzleVelocity = 180;
        mass = .5f;
        damageDealt = 20;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            // TracerMesh should have a Health component
            Health tracerHealth = other.GetComponent<Health>();
            float z = tracerHealth.HitPoints;
            tracerHealth.TakeDamage(damageDealt);
            //Debug.Log("Hit Player!");
            
        }
    }
}
