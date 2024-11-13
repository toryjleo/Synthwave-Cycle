using EditorObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gun
{

    public class PooledHitScanBulletTrail : Generic.Poolable
    {
        private GunStats gunStats = null;
        private TrailRenderer trailRenderer = null;
        private bool hasMovedToEnd = false;
        Vector3 endLocation = Vector3.zero;

        private float timer = 0.0f;

        public override void Init(Generic.IPoolableInstantiateData stats)
        {
            EditorObject.GunStats gunStats = stats as EditorObject.GunStats;
            if (gunStats != null)
            {
                this.gunStats = gunStats;
            }
            else
            {
                Debug.LogError("Expects to be handed a GunStats reference.");
            }

            trailRenderer = GetComponent<TrailRenderer>();
            if (trailRenderer == null)
            {
                Debug.LogError("Object needs an assigned TrailRenderer component!");
            }
            Reset();
        }

        private void Update()
        {
            if (hasMovedToEnd)
            {
                timer += Time.deltaTime;
                if (timer >= gunStats.TimeTillBulletTrailDespawn) 
                {
                    OnDespawn();
                }
            }
            else
            {
                hasMovedToEnd = true;
                transform.position = endLocation;
            }
        }

        public void SetStartAndEndLocation(Vector3 startLocation, Vector3 endLocation)
        {
            transform.position = startLocation;
            this.endLocation = endLocation;
        }

        public override void Reset()
        {
            // TODO: Reset Bullet Trail Data (probably reset the BulletTrail
            hasMovedToEnd = false;
            endLocation = Vector3.zero;
            trailRenderer.Clear();
            trailRenderer.time = gunStats.TimeTillBulletTrailDespawn * .66f;
            timer = 0.0f;
        }
    }
}