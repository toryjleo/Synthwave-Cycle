using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void NotifyHeal();

/// <summary>Class <c>Health</c> A Unity Component which tracks health.</summary>
public class Health : MonoBehaviour
{
    private float _hitPoints;
    public NotifyHeal healEvent;

    public bool isInvulnurable = false;


    public float HitPoints
    {
        get => _hitPoints;
    }


    //Whem do I call this method and why? Do I have to call it every time? 
    /// <summary>An itialization method.</summary>
    /// <param name="initialHealth">The number of hit points to start with.</param>
    public void Init(float initialHealth) 
    {
        _hitPoints = initialHealth;
    }


    /// <summary>Subtracts points to _hitPoints.</summary>
    /// <param name="hp">The number of points to subtract from _hitPoints.</param>
    public void TakeDamage(float hp)
    {
        if (isInvulnurable)
        {
            return;
        }
        _hitPoints -= hp;
    }

    /// <summary>Adds points to _hitPoints.</summary>
    /// <param name="hp">The number of points to add to _hitPoints.</param>
    public void Heal(float hp) 
    {
        // Notify effects that this is healing
        healEvent?.Invoke();

        // Ensure we do not overflow
        if (_hitPoints+hp < _hitPoints) 
        {
            _hitPoints = float.MaxValue;
        }
        _hitPoints += hp;
    }
}
