using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
    public class AreaOfEffect : Projectile
    {   
        private float timer = 0.0f;

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            switch (gunStats.NumPhases) 
            {
                case EditorObject.AOEPhases.Persistant:
                    break;
                case EditorObject.AOEPhases.OnePhase:
                    AdjustTimer(Time.deltaTime);
                    AdjustScale(Time.deltaTime);
                    break;
                default:
                    break;
            }

            // TODO: Adjust scale
        }

        public override void Reset() 
        {
            base.Reset();

            timer = 0.0f;

        }

        private void AdjustTimer(float deltaTime) 
        {
            timer += deltaTime;

            if (timer >= gunStats.Phase1.Duration)
            {
                Debug.Log("End Phase 1");
                OnDespawn();
            }
        }

        private void AdjustScale(float deltaTime) 
        {
            Vector3 curScale = transform.localScale;
            curScale.x += gunStats.Phase1.RateOfScaleGrowth * deltaTime;
            curScale.z += gunStats.Phase1.RateOfScaleGrowth * deltaTime;
            transform.localScale = curScale;
        }
    }
}
