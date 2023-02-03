using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Bullets
{
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
                otherHealth.TakeDamage(damageDealt);
                if (!overPenetrates)
                {
                    OnDespawn();
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "PlayerHealth")
            {
                DealDamageAndDespawn(other.gameObject);
            }
        }
    }
}
