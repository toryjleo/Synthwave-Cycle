using EditorObject;
using Generic;
using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
    public class AreaOfEffect : Projectile
    {   
        private float timer = 0.0f;
        private EditorObject.AOEPhases currentPhase = 0;

        public override void Init(IPoolableInstantiateData stats)
        {
            base.Init(stats);
            Reset();
            countPenetration = false;
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            switch (currentPhase) 
            {
                case EditorObject.AOEPhases.Persistant:
                    break;
                case EditorObject.AOEPhases.OnePhase:
                case EditorObject.AOEPhases.TwoPhase:
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
            if (currentPhase != EditorObject.AOEPhases.Persistant) 
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

        private void AdjustTimer(float deltaTime) 
        {
            timer += deltaTime;

            switch (currentPhase) 
            {
                case EditorObject.AOEPhases.TwoPhase:
                    if (timer >= gunStats.Phase2.Duration)
                    {
                        OnDespawn();
                    }
                    break;
                case EditorObject.AOEPhases.OnePhase:
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

        private void AdjustScale(float deltaTime) 
        {
            float growth = 0;
            switch (currentPhase)
            {
                case EditorObject.AOEPhases.TwoPhase:
                    growth = gunStats.Phase2.RateOfScaleGrowth;
                    break;
                case EditorObject.AOEPhases.OnePhase:
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
