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
    public NotifyHealth onHealthChange;

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

        deadEvent += HandleDeath;

    }

    private void Start()
    {
        ApplyInitialState();

        // Handle reset state
        if (GameStateController.StateExists)
        {
            GameStateController.resetting.notifyListenersEnter += ApplyInitialState;
        }
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

    /// <summary>
    /// Sets the Player Health to the state it must be at for the game to start
    /// </summary>
    private void ApplyInitialState()
    {
        float maxHp = playerHealth.BarMax3HP + (playerHealth.BarMax3HP - playerHealth.BarMax2HP) / 2;
        Init(playerHealth.HpOnStart, maxHp);
        onHealthChange?.Invoke();
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
            onHealthChange?.Invoke();
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
        onHealthChange?.Invoke();
    }

    /// <summary>
    /// Kills the player instantly, triggers any triggers related to player death.
    /// </summary>
    /// <param name="timerOn">Not used</param>
    public void KillPlayer(bool timerOn)
    {
        TakeDamage(HitPoints);
    }

    /// <summary>
    /// Call ZeroHP trigger
    /// </summary>
    private void HandleDeath()
    {
        if (GameStateController.StateExists)
        {
            GameStateController.HandleTrigger(StateTrigger.ZeroHP);
        }
        else
        {
            Debug.LogWarning("Tried to trigger StateTrigger.ZeroHP but no state exists");
        }
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
