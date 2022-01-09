using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float _hitPoints;

    public float HitPoints
    {
        get => _hitPoints;
    }

    public void Init(float initialHealth) 
    {
        _hitPoints = initialHealth;
    }

    public void TakeDamage(float hp)
    {
        _hitPoints -= hp;
    }

    public void Heal(float hp) 
    {
        // TODO: Make sure this does not go over max float
        _hitPoints += hp;
    }
}
