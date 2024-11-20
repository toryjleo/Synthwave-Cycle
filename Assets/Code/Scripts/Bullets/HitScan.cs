using UnityEngine;
using Generic;

namespace Gun
{
    /// <summary>
    /// Class containing logic for raycast weapons
    /// </summary>
    /// 
    public class HitScan
    {
        private EditorObject.GunStats gunStats = null;

        public event BulletHitHandler notifyListenersHit;


        protected ObjectPool hitScanBulletTrailPool = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gunStats">Stats to use</param>
        public HitScan(EditorObject.GunStats gunStats, PooledHitScanBulletTrail bulletTrailPrefab, int bulletTrailCount)
        {
            this.gunStats = gunStats;

            if (hitScanBulletTrailPool == null)
            {
                hitScanBulletTrailPool = new ObjectPool(gunStats, bulletTrailPrefab);
                hitScanBulletTrailPool.PoolObjects(bulletTrailCount);
            }
        }

        public void Reset() 
        {
            hitScanBulletTrailPool.ResetGameObject();
        }

        /// <summary>
        /// Shoots in a specified direction
        /// </summary>
        /// <param name="curPosition">Place to spawn ray</param>
        /// <param name="direction">Normalized direction vector for ray</param>
        /// <param name="impactEffectPool">Effect to play on impact</param>
        public void Shoot(Vector3 curPosition, Vector3 direction)
        {
            // Get hits
            RaycastHit[] hits;
            hits = Physics.RaycastAll(curPosition, direction, gunStats.Range);
            System.Array.Sort(hits, (a, b) => (a.distance.CompareTo(b.distance)));

            int numberOfHitObjects = Mathf.Min(gunStats.BulletPenetration + 1, hits.Length);

            for (int i = 0; i < numberOfHitObjects; i++)
            {
                RaycastHit hit = hits[i];

                notifyListenersHit?.Invoke(hit.point);

                if ((hit.transform.tag == "Enemy" && gunStats.IsPlayerGun) ||
                    (hit.transform.tag == "Player" && !gunStats.IsPlayerGun))
                {

                    DealDamage(hit.transform.gameObject);
                }


                // TODO: Move the particle spawning logic to Gun.HandleBulletHit
                Material hitMaterial = GetHitMaterial(hit);
                // TODO: use opposite of travelling vector for forward instead of normal
                ImpactManager.Instance.SpawnBulletImpact(hit.point, hit.normal, hitMaterial);
                /*PooledParticle particle = impactEffectPool.SpawnFromPool() as PooledParticle;
                particle.transform.position = hit.transform.position;
                particle.transform.rotation = Quaternion.LookRotation(hit.normal);
                particle.Play();*/
            }

            // Logic for visuals
            Vector3 finalHitLocation = Vector3.zero;
            if (numberOfHitObjects == 0)
            {
                // Hit nothing case. Bullet Trail goes to gun max range
                finalHitLocation = (direction * gunStats.Range) + curPosition;
            }
            else
            {
                // Bullet trail goes to last target
                RaycastHit finalHit = hits[numberOfHitObjects - 1];
                finalHitLocation = finalHit.point;
            }

            DrawBulletTrail(curPosition, finalHitLocation);
        }

        /// <summary>
        /// Inflict gun damage on other
        /// </summary>
        /// <param name="other">Object with Health component</param>
        private void DealDamage(GameObject other)
        {

            Health otherHealth = other.GetComponentInChildren<Health>();
            if (otherHealth == null)
            {
                Debug.LogError("Object does not have Health component: " + other.name);
            }
            else
            {
                otherHealth.TakeDamage(gunStats.DamageDealt);
            }

        }

        /// <summary>
        /// Draws a bullet trail from the start to end location
        /// </summary>
        /// <param name="startLocation">The location to start drawing the trail in world coordinates</param>
        /// <param name="endLocation">The location to end drawing the trail in world coordinates</param>
        private void DrawBulletTrail(Vector3 startLocation, Vector3 endLocation)
        {

            PooledHitScanBulletTrail hitScanTrail = hitScanBulletTrailPool.SpawnFromPool() as PooledHitScanBulletTrail;
            if (hitScanTrail == null)
            {
                Debug.LogError("Pooled object needs component of type PooledHitScanBulletTrail");
            }
            else 
            {
                hitScanTrail.SetStartAndEndLocation(startLocation, endLocation);
            }
        }

        Material GetHitMaterial(RaycastHit hit) 
        {
            Renderer hitRenderer = hit.transform.gameObject.GetComponent<Renderer>();
            Material hitMaterial = hitRenderer == null ? null : hitRenderer.sharedMaterial;
            return hitMaterial;
        }
    }
}