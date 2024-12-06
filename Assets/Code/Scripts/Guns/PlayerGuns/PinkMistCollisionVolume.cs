using Assets.Scripts.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child Volume component for PinkMist that deals damage to enemies and their attacks
/// </summary>
public class PinkMistCollisionVolume : MonoBehaviour
{
    //Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ai>() != null)
        {
            Ai ai = other.GetComponent<Ai>();
            ai.health.Kill();
        }
        if (other.GetComponent<EnemyBullet>() != null)
        {
            EnemyBullet eb = other.GetComponent<EnemyBullet>();
            eb.Despawn();
        }
    }
}
