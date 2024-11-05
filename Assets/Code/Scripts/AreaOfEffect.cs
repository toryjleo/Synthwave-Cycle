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


            AdjustTimer(Time.deltaTime);
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

            if (timer >= gunStats.Phase1Length)
            {
                Debug.Log("End Phase 1");
                // TODO: End Phase1
            }
        }
    }
}
