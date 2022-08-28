using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acid : HitCondition
{
    float _damageBoostAmount;

    public Acid(float damageBoostAmount)
    {
        _damageBoostAmount = damageBoostAmount;
    }

    //Must override the abstract method to implement base class
    internal override void InflictEffect(Ai host, float damage)
    {
        //Every time an acid effected AI is hit, take additional damage
        host.hp.TakeDamage(damage * _damageBoostAmount);

    }
}
