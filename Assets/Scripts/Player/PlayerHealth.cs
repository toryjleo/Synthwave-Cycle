using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

    public bool isInvulnurable = false;


    /// <summary>Subtracts points to _hitPoints.</summary>
    /// <param name="hp">The number of points to subtract from _hitPoints.</param>
    public override void TakeDamage(float hp)
    {
        if (isInvulnurable)
        {
            return;
        }
        else 
        {
            base.TakeDamage(hp);
        }
    }
}
