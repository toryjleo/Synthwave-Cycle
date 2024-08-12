using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerHealth;

public delegate void NotifyPlayerHealth(BarMax oldMax, BarMax newMax, bool hpIsOverBarMax3);

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
            if (playerHealth == null) 
            {
                Debug.LogError("Please assign player health EditorObject to PlayerHealth componenet");
                return 0f;
            }
            switch (currentBar) 
            {
                case BarMax.Bar1:
                    return HitPoints / playerHealth.BarMax1HP;
                case BarMax.Bar2:
                    return (HitPoints - playerHealth.BarMax1HP) / (playerHealth.BarMax2HP - playerHealth.BarMax1HP);
                case BarMax.Bar3:
                    return (HitPoints - playerHealth.BarMax2HP) / (playerHealth.BarMax3HP - playerHealth.BarMax2HP);
                default:
                    return -1;
            }
        }
    }



    private void Awake()
    {

        currentBar = BarMax.Bar1;
        float maxHp = playerHealth.BarMax3HP + (playerHealth.BarMax3HP - playerHealth.BarMax2HP) / 2;
        Init(playerHealth.HpOnStart, maxHp);

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

    public void HealFromHealthPool() 
    {
        Heal(playerHealth.HpFromHealthPool);
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

        if (HitPoints > playerHealth.BarMax2HP)
        {
            newBarMax = BarMax.Bar3;
        }
        else if (HitPoints > playerHealth.BarMax1HP)
        {
            newBarMax = BarMax.Bar2;
        }

        if (newBarMax != currentBar || currentBar == BarMax.Bar3)
        {
            onBarUpdate?.Invoke(currentBar, newBarMax, HitPoints > playerHealth.BarMax3HP);

            currentBar = newBarMax;
        }

    }
}
