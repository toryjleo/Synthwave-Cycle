using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
    /// <summary>Class <c>BulletProjectile</c> An implementation of the projectile class that deals damage and despawns.</summary>
    public class BulletProjectile : Projectile
    {

        protected int penetrationCount = 0;

        public override void Reset()
        {
            penetrationCount = 0;
            base.Reset();
        }

        private void OnTriggerEnter(Collider other)
        {

            if (CanHitObject(other))
            {
                NotifyListenersHit();
                DealDamageAndDespawn(other.gameObject);
            }
            Vector3 positon = other.ClosestPoint(transform.position);
            Vector3 normal = transform.position - positon;
            Material material = GetGameObjectMaterial(other.gameObject);
            Vector3 particleSprayDir = -shootDir.normalized;
            ImpactManager.Instance.SpawnBulletImpact(positon, particleSprayDir, material);
        }

        /// <summary>
        /// Deals damage to the enemy and will despawn bullet if exceeding penetration count.
        /// </summary>
        /// <param name="other">Object hit</param>
        private void DealDamageAndDespawn(GameObject other)
        {
            if (!alreadyHit.Contains(other))
            {

                penetrationCount++;

                alreadyHit.Add(other);
                Health otherHealth = other.GetComponentInChildren<Health>();
                if (otherHealth == null)
                {
                    Debug.LogError("Object does not have Health component: " + gameObject.name);
                }
                else
                {
                    otherHealth.TakeDamage(gunStats.DamageDealt);
                }
                if (penetrationCount > gunStats.BulletPenetration)
                {
                    OnDespawn();
                }
            }
        }
        Material GetGameObjectMaterial(GameObject hitObject)
        {
            Renderer hitRenderer = hitObject.GetComponent<Renderer>();
            Material hitMaterial = hitRenderer == null ? null : hitRenderer.sharedMaterial;
            return hitMaterial;
        }
    }
}
