using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>Rammable</c> A Unity Component which deals damage to enemies on collision.</summary>
/// Expects same gameObject to have a Rigidbody
public class Rammable : MonoBehaviour
{
    Health health;
    private const float STARTING_HEALTH = 3000000.0f;
    private const float DAMAGE_DONE_WHEN_RAMMING = 100.0f;
    private const float REQUIRED_RAMMING_SPEED = 45.0f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponentInChildren<Health>();
        health.Init(STARTING_HEALTH);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy") 
        {
            float relativeVelocity = collision.relativeVelocity.magnitude;
            if (relativeVelocity > REQUIRED_RAMMING_SPEED)
            {
                Health otherHealth = collision.gameObject.GetComponentInChildren<Health>();
                otherHealth.TakeDamage(DAMAGE_DONE_WHEN_RAMMING);
            }
        }
        
    }

}
