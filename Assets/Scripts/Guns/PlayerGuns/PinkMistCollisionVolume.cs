using Assets.Scripts.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkMistCollisionVolume : MonoBehaviour
{
    //Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ai>() != null)
        {
            Ai ai = other.GetComponent<Ai>();
            ai.Die();
        }
        if (other.GetComponent<EnemyBullet>() != null)
        {
            EnemyBullet eb = other.GetComponent<EnemyBullet>();
            eb.Despawn();
        }
    }
}
