using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    private const float HP_ON_START = 200.0f;

    private const float BAR_MAX_3_HP = 1300.0f;


    public bool isInvulnurable = false;

    private void Awake()
    {
        Init(HP_ON_START, BAR_MAX_3_HP);
        deadEvent += HandleDeath;
    }

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

    private void HandleDeath()
    {
        GameStateController.HandleTrigger(StateTrigger.ZeroHP);
    }
}
