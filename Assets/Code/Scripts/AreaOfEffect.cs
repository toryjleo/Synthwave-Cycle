using EditorObject;
using Generic;
using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
    /// <summary>
    /// Phases an AOE can go through
    /// </summary>
    public enum AOEPhases
    {
        OnePhase,
        TwoPhase,
        Persistant,
    }

    /// <summary>
    /// Implementation of Projectile that deals damage and can grow/shrink over time.
    /// </summary>
    public class AreaOfEffect : Projectile
    {   
        private float timer = 0.0f;
        private AOEPhases currentPhase = 0;

        public override void Init(IPoolableInstantiateData stats)
        {
            base.Init(stats);
            Reset();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            switch (currentPhase) 
            {
                case AOEPhases.Persistant:
                    break;
                case AOEPhases.OnePhase:
                case AOEPhases.TwoPhase:
                    AdjustTimer(Time.deltaTime);
                    AdjustScale(Time.deltaTime);
                    break;
                default:
                    break;
            }

        }

        public override void Reset() 
        {
            base.Reset();

            timer = 0.0f;

            currentPhase = gunStats.NumPhases;
            if (currentPhase != AOEPhases.Persistant) 
            {
                currentPhase = AOEPhases.OnePhase;
            }
        }

        void OnTriggerStay(Collider other)
        {
            float deltaTime = Time.deltaTime;

            if (CanHitObject(other)) 
            {
                NotifyListenersHit();

                Health otherHealth = other.GetComponentInChildren<Health>();
                if (otherHealth == null)
                {
                    Debug.LogError("Object does not have Health component: " + gameObject.name);
                }
                else
                {
                    otherHealth.TakeDamage(gunStats.DamagePerSecond * deltaTime);
                }
            }
        }

        /// <summary>
        /// Adjusts the timer's time and can transition to sequential phases
        /// </summary>
        /// <param name="deltaTime">Amount of time passed since last frame</param>
        private void AdjustTimer(float deltaTime) 
        {
            timer += deltaTime;

            switch (currentPhase) 
            {
                case AOEPhases.TwoPhase:
                    if (timer >= gunStats.Phase2.Duration)
                    {
                        OnDespawn();
                    }
                    break;
                case AOEPhases.OnePhase:
                    if (timer >= gunStats.Phase1.Duration)
                    {
                        timer = 0;
                        currentPhase++;
                    }
                    break;
                default:
                    Debug.LogError("Invalid Phase");
                    break;
            }
        }

        /// <summary>
        /// Adjusts scale as specified in the phase
        /// </summary>
        /// <param name="deltaTime">Amount of time passed since last frame</param>
        private void AdjustScale(float deltaTime) 
        {
            float growth = 0;
            switch (currentPhase)
            {
                case AOEPhases.TwoPhase:
                    growth = gunStats.Phase2.RateOfScaleGrowth;
                    break;
                case AOEPhases.OnePhase:
                    growth = gunStats.Phase1.RateOfScaleGrowth;
                    break;
                default:
                    Debug.LogError("Invalid Phase");
                    break;
            }
            Vector3 curScale = transform.localScale;
            curScale.x += growth * deltaTime;
            curScale.z += growth * deltaTime;
            transform.localScale = curScale;
        }
    }
}
