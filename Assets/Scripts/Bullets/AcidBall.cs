using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidBall : Bullet
{
    private const float DECELERATION_RATE = 200f;
    public override void Init()
    {
        muzzleVelocity = 100;
        mass = 1f; //The Mass controlls how slowed down the bike is by recoil
        damageDealt = 4;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //if we hit an Ai
            Ai hitAi = other.GetComponentInChildren<Ai>();
            if (hitAi)
            {
                Acid acid = new Acid(0.25f);
                hitAi.AddHitCondition(acid);
            }
            // TracerMesh should have a Health component
            hitAi.Hit(damageDealt);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        muzzleVelocity -= DECELERATION_RATE * Time.deltaTime;
        if(muzzleVelocity <= 0)
        {
            OnDespawn();
        }
    }
}
