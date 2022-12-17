using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShottyPellet : Bullet
{
    private float growthSpeed = 4f;
    private float maxSize = 10f;
    public override void Init()
    {
        muzzleVelocity = 90;
        mass = .2f;
        damageDealt = 2;
        boost = 2f;
    }

    public override void ResetBullet()
    {
        this.gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
    }

    public override void Update()
    {
        float resize = this.gameObject.transform.localScale.x + (growthSpeed * Time.deltaTime);
        if (resize <= maxSize)
        {
            this.gameObject.transform.localScale = new Vector3(resize, resize, resize);
        }
        base.Update();
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
