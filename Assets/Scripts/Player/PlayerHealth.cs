using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a variant of health made for the player
/// </summary>
public class PlayerHealth : Health
{
    public enum BarMax
    {
        Bar1,
        Bar2,
        Bar3,
    }


    private const float HP_ON_START = 200.0f;

    private const float BAR_MAX_1_HP = 1000.0f;
    private const float BAR_MAX_2_HP = 2000.0f;
    private const float BAR_MAX_3_HP = 3000.0f;


    public bool isInvulnurable = false;

    private BarMax currentBar;
    NotifyHealth barUpdated;

    /// <summary>
    /// The current bar to work toward
    /// </summary>
    public BarMax CurrentBar 
    {
        get { return currentBar; }
    }

    /// <summary>
    /// Gets the percent progress toward the next BarMax.
    /// </summary>
    public float PercentProgress
    {
        get 
        {
            switch (currentBar) 
            {
                case BarMax.Bar1:
                    return HitPoints / BAR_MAX_1_HP;
                case BarMax.Bar2:
                    return (HitPoints - BAR_MAX_1_HP) / (BAR_MAX_2_HP - BAR_MAX_1_HP);
                case BarMax.Bar3:
                    return (HitPoints - BAR_MAX_2_HP) / (BAR_MAX_3_HP - BAR_MAX_2_HP);
                default:
                    return -1;
            }
        }
    }



    private void Awake()
    {
        currentBar = BarMax.Bar1;
        Init(HP_ON_START, BAR_MAX_3_HP);

        deadEvent += HandleDeath;
        
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            Heal(1000);
        }
        if (Input.GetKeyDown(KeyCode.Minus)) 
        {
            TakeDamage(1000);
        }
#endif
    }

    /// <summary>Subtracts points to _hitPoints, handles invulnerability, and updates the bar max</summary>
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

            UpdateBarMax();
        }
    }

    /// <summary>
    /// Handles healing and updates the bar maximum
    /// </summary>
    /// <param name="hp">The number of points to add to _hitPoints.</param>
    public override void Heal(float hp)
    {
        base.Heal(hp);

        UpdateBarMax();
        
    }

    /// <summary>
    /// Call ZeroHP trigger
    /// </summary>
    private void HandleDeath()
    {
        GameStateController.HandleTrigger(StateTrigger.ZeroHP);
    }

    /// <summary>
    /// Handle updating the HP Bar Maximum
    /// </summary>
    private void UpdateBarMax()
    {
        BarMax newBarMax = BarMax.Bar1;

        if (HitPoints > BAR_MAX_2_HP)
        {
            newBarMax = BarMax.Bar3;
        }
        else if (HitPoints > BAR_MAX_1_HP)
        {
            newBarMax = BarMax.Bar2;
        }

        if (newBarMax != currentBar)
        {
            currentBar = newBarMax;
            barUpdated?.Invoke();
        }

    }
}
