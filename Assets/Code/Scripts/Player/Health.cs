using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void NotifyHealth();

/// <summary>Class <c>Health</c> A Unity Component which tracks health.</summary>
public class Health : MonoBehaviour
{
    [SerializeField] private float hitPoints;
    private float maxHitPoints;
    public NotifyHealth healEvent;
    public NotifyHealth hpHitZero;


    public float HitPoints
    {
        get => hitPoints;
    }


    //Whem do I call this method and why? Do I have to call it every time? 
    /// <summary>An itialization method.</summary>
    /// <param name="initialHealth">The number of hit points to start with.</param>
    /// <param name="maxHealth">The maximum amount of hitpoints which this entity will not surpass.</param>
    public void Init(float initialHealth, float maxHealth = float.MaxValue)
    {
        hitPoints = initialHealth;
        maxHitPoints = maxHealth;
    }


    /// <summary>Subtracts points to _hitPoints.</summary>
    /// <param name="hp">The number of points to subtract from _hitPoints.</param>
    public virtual void TakeDamage(float hp)
    {
        if (GameStateController.CanRunGameplay)
        {
            hitPoints -= hp;

            if (hitPoints <= 0)
            {
                hpHitZero?.Invoke();
            }
        }
    }

    /// <summary>Adds points to _hitPoints.</summary>
    /// <param name="hp">The number of points to add to _hitPoints.</param>
    public virtual void Heal(float hp)
    {
        // Notify effects that this is healing
        healEvent?.Invoke();

        // Ensure we do not overflow
        if (hitPoints + hp < hitPoints)
        {
            hitPoints = float.MaxValue;
        }
        else
        {
            // Ensure that _hitPoints does not go over the stated maximum
            hitPoints = Mathf.Min(hitPoints + hp, maxHitPoints);
        }
    }

    public void Kill()
    {
        TakeDamage(HitPoints);
    }
}
