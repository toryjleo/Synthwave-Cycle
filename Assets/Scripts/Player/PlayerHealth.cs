using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerHealth;

public delegate void NotifyPlayerHealth(BarMax oldMax, BarMax newMax);

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

    #region UpdatedAtAwake
    private float hpOnStart = 200.0f;
    private float barMax1HP = 1000.0f;
    private float barMax2HP = 2000.0f;
    private float barMax3HP = 3000.0f;
    #endregion

    #region DefinedInPrefab
    /// <summary>
    /// Defines values for this object
    /// </summary>
    [SerializeField] private EditorObject.PlayerHealth playerHealth;
    #endregion

    public bool isInvulnurable = false;

    private BarMax currentBar;
    public NotifyPlayerHealth onBarUpdate;

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
                    return HitPoints / barMax1HP;
                case BarMax.Bar2:
                    return (HitPoints - barMax1HP) / (barMax2HP - barMax1HP);
                case BarMax.Bar3:
                    return (HitPoints - barMax2HP) / (barMax3HP - barMax2HP);
                default:
                    return -1;
            }
        }
    }



    private void Awake()
    {
        hpOnStart = playerHealth.HpOnStart;
        barMax1HP = playerHealth.BarMax1HP;
        barMax2HP = playerHealth.BarMax2HP;
        barMax3HP = playerHealth.BarMax3HP;

        currentBar = BarMax.Bar1;
        Init(hpOnStart, barMax3HP);

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

        if (HitPoints > barMax2HP)
        {
            newBarMax = BarMax.Bar3;
        }
        else if (HitPoints > barMax1HP)
        {
            newBarMax = BarMax.Bar2;
        }

        if (newBarMax != currentBar)
        {
            onBarUpdate?.Invoke(currentBar, newBarMax);

            currentBar = newBarMax;
        }

    }
}
