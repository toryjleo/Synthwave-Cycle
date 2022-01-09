using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BulletPool</c> A Unity Component which tracks health.</summary>
public class Health : MonoBehaviour
{
    private float _hitPoints;

    public float HitPoints
    {
        get => _hitPoints;
    }

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
        _hitPoints -= hp;
    }

    /// <summary>Adds points to _hitPoints.</summary>
    /// <param name="hp">The number of points to add to _hitPoints.</param>
    public void Heal(float hp) 
    {
        // TODO: Make sure this does not go over max float
        _hitPoints += hp;
    }
}
