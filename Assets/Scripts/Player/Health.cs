using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void NotifyHealth();

/// <summary>Class <c>Health</c> A Unity Component which tracks health.</summary>
public class Health : MonoBehaviour
{
    private float _hitPoints;
    private float _maxHitPoints;
    public NotifyHealth healEvent;
    public NotifyHealth deadEvent;


    public float HitPoints
    {
        get => _hitPoints;
    }


    //Whem do I call this method and why? Do I have to call it every time? 
    /// <summary>An itialization method.</summary>
    /// <param name="initialHealth">The number of hit points to start with.</param>
    /// <param name="maxHealth">The maximum amount of hitpoints which this entity will not surpass.</param>
    public void Init(float initialHealth, float maxHealth = float.MaxValue) 
    {
        _hitPoints    = initialHealth;
        _maxHitPoints = maxHealth;
    }


    /// <summary>Subtracts points to _hitPoints.</summary>
    /// <param name="hp">The number of points to subtract from _hitPoints.</param>
    public virtual void TakeDamage(float hp)
    {
        if (GameStateController.GameIsPlaying())
        {
            _hitPoints -= hp;

            if (_hitPoints <= 0)
            {
                deadEvent?.Invoke();
            }
        }
    }

    /// <summary>Adds points to _hitPoints.</summary>
    /// <param name="hp">The number of points to add to _hitPoints.</param>
    public void Heal(float hp) 
    {
        // Notify effects that this is healing
        healEvent?.Invoke();

        // Ensure we do not overflow
        if (_hitPoints + hp < _hitPoints) 
        {
            _hitPoints = float.MaxValue;
        }
        else 
        {
            // Ensure that _hitPoints does not go over the stated maximum
            _hitPoints = Mathf.Min(_hitPoints + hp, _maxHitPoints);
        }
        Debug.Log(_hitPoints);
    }
}
