using EditorObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gun
{
    /// <summary>
    /// Class that manages a trailrenderer visual for hitscans
    /// </summary>
    public class PooledHitScanBulletTrail : Generic.Poolable
    {
        private GunStats gunStats = null; // Used for visual pass
        private TrailRenderer trailRenderer = null;

        Vector3 endLocation = Vector3.zero;
        private bool hasMovedToEnd = false;

        private float timeTillBulletTrailDespawn = .25f;
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
                TimerUpdate(Time.deltaTime);
            }
            else
            {
                UpdateEnd();
            }
        }

        /// <summary>
        /// Moves the line to the end
        /// </summary>
        private void UpdateEnd()
        {
            hasMovedToEnd = true;
            transform.position = endLocation;
        }

        /// <summary>
        /// Updates the timer and despawns when timer has finished
        /// </summary>
        /// <param name="deltaTime">Amount of time since last frame</param>
        private void TimerUpdate(float deltaTime) 
        {
            timer += deltaTime;
            if (timer >= timeTillBulletTrailDespawn)
            {
                OnDespawn();
            }
        }

        /// <summary>
        /// Sets where the line is drawn.
        /// </summary>
        /// <param name="startLocation">The location to start drawing the trail in world coordinates</param>
        /// <param name="endLocation">The location to end drawing the trail in world coordinates</param>
        public void SetStartAndEndLocation(Vector3 startLocation, Vector3 endLocation)
        {
            transform.position = startLocation;
            this.endLocation = endLocation;
            trailRenderer.Clear();
        }

        public override void Reset()
        {
            hasMovedToEnd = false;
            endLocation = Vector3.zero;
            trailRenderer.Clear();
            trailRenderer.time = timeTillBulletTrailDespawn;
            timer = 0.0f;
        }
    }
}