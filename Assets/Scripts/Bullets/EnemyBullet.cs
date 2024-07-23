using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Bullets
{
    /// <summary>
    /// An EnemyBullet is the abstract class all hostile bullets inherit from. It will damage the player on hit
    /// </summary>
    public abstract class EnemyBullet : Bullet
    {
        internal override void DealDamageAndDespawn(GameObject other)
        {
            if (!alreadyHit.Contains(other))
            {
                alreadyHit.Add(other);
                Health otherHealth = other.GetComponentInChildren<Health>();
                if (otherHealth == null)
                {
                    Debug.LogError("Object does not have Health component: " + gameObject.name);
                }
                else
                {
                    otherHealth.TakeDamage(damageDealt);
                }
                if (!overPenetrates)
                {
                    OnDespawn();
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                DealDamageAndDespawn(other.gameObject);
            }
        }
    }
}
