using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Burning condition damages enemies on a set tick rate</summary>
public class Burning : TickCondition
{
    protected float fireDamage = 10f;

    public Burning(float damage, float burnTickRate)
    {
        tickRate = burnTickRate;
        fireDamage = damage;
    }


    internal override void InflictEffect(Ai host)
    {
        if (CheckTick())
        {
            host.hp.TakeDamage(fireDamage);
        }
    }
}
