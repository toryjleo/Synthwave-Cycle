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
        public void Shoot(Vector3 curPosition, Vector3 direction, Generic.ObjectPool impactEffectPool)
        {
            // Get hits
            RaycastHit[] hits;
            hits = Physics.RaycastAll(curPosition, direction, gunStats.Range);
            System.Array.Sort(hits, (a, b) => (a.distance.CompareTo(b.distance)));

            //Debug.Log("Direction " + direction);

            int n = Mathf.Min(gunStats.BulletPenetration + 1, hits.Length);

            for (int i = 0; i < n; i++)
            {
                RaycastHit hit = hits[i];
                Debug.Log(hit.transform.name);

                notifyListenersHit?.Invoke(hit.point);

                if ((hit.transform.tag == "Enemy" && gunStats.IsPlayerGun) ||
                    (hit.transform.tag == "Player" && !gunStats.IsPlayerGun))
                {

                    DealDamage(hit.transform.gameObject);
                }

                // TODO: Move the particle spawning logic to Gun.HandleBulletHit
                PooledParticle particle = impactEffectPool.SpawnFromPool() as PooledParticle;
                particle.transform.position = hit.transform.position;
                particle.transform.rotation = Quaternion.LookRotation(hit.normal);
                particle.Play();
            }

            Vector3 finalHitLocation = Vector3.zero;
            if (n == 0)
            {
                // Hit nothing case. Bullet Trail goes to gun max range
                finalHitLocation = (direction * gunStats.Range) + curPosition;
            }
            else
            {
                // Bullet trail goes to last target
                RaycastHit finalHit = hits[n - 1];
                finalHitLocation = finalHit.point;
            }


            Debug.Log("Drawing to: " + finalHitLocation);

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
    }
}