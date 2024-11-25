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
        public void Shoot(Vector3 curPosition, Vector3 direction)
        {
            // Make sure to hit the correct things get hit with a layer mask
            int mask = 0;
            int layerEnemy = 9;
            int layerPlayer = 7;
            if (gunStats.IsPlayerGun) 
            {
                mask = 1 << layerEnemy;
            }
            else 
            {
                mask = 1 << layerPlayer;
            }

            // Get hits
            RaycastHit[] hits;
            hits = Physics.RaycastAll(curPosition, direction, gunStats.Range, mask);
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


                Material hitMaterial = GetHitMaterial(hit);
                Vector3 particleSprayDir = (curPosition - hit.point).normalized;
                ImpactManager.Instance.SpawnBulletImpact(hit.point, particleSprayDir, hitMaterial);
            }

            // Logic for visuals
            Vector3 finalHitLocation = Vector3.zero;
            if (numberOfHitObjects == 0)
            {
                // Hit nothing case. Bullet Trail goes to gun max range
                finalHitLocation = (direction * gunStats.Range) + curPosition;
                ImpactManager.Instance.SpawnHitScanBulletMiss(finalHitLocation);
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

        /// <summary>
        /// Gets the shared material of a raycast hit
        /// </summary>
        /// <param name="hit">Hit object from a raycast</param>
        /// <returns>The shared material</returns>
        Material GetHitMaterial(RaycastHit hit) 
        {
            Renderer hitRenderer = hit.transform.gameObject.GetComponent<Renderer>();
            Material hitMaterial = hitRenderer == null ? null : hitRenderer.sharedMaterial;
            return hitMaterial;
        }
    }
}