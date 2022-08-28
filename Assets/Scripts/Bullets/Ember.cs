using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ember : Bullet
{
    const float DROP_RATE = 10f;
    bool collided = false;

    public override void Init()
    {
        muzzleVelocity = -100f;
        mass = 0.1f;
        damageDealt = 3200f;
        lifetime = 4f;
        boost = 10f;
        hasFiniteLifetime = true;
    }

    protected override void Move()
    {
        if(!collided)
        {
            transform.position -= new Vector3(0, DROP_RATE * Time.deltaTime, 0);
            base.Move();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            collided = true;
        }
        if (other.gameObject.tag == "Enemy")
        {
            Ai hitAi = other.GetComponentInChildren<Ai>();
            if(hitAi)
            {
                Burning burn = new Burning(10f, 200f);
                hitAi.AddTickCondition(burn);
            }
        }
    }
}
