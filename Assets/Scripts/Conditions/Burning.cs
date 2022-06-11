using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burning : Condition
{
    protected float fireDamage = 10f;

    public Burning(float damage, float burnTickRate)
    {
        tickRate = burnTickRate;
        fireDamage = damage;
    }


    internal override void InflictEffect()
    {
        Debug.Log("Fire Damage Tick!");
        hostAi.hp.TakeDamage(fireDamage);
    }
}
