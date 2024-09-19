using EditorObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
    /// <summary>
    /// Defines behavior that a particle must exhibit when used on a gun.
    /// Requires: ParticleSystem Component
    /// </summary>
    public class PooledParticle : PoolableGunObject
    {
        private ParticleSystem particleSystem;

        public override void Init(GunStats gunStats)
        {
            particleSystem = GetComponent<ParticleSystem>();
            hasFiniteLifetime = true;

            if (particleSystem == null)
            {
                Debug.LogError("PooledParticle instance requires a ParticleSystem component");
            }
            if (lifetime == 0)
            {
                Debug.LogError("PooledParticle requires a lifetime to remain in world");
            }
            Reset();
        }

        public override void Reset()
        {
            timeInWorld = 0;
        }

        /// <summary>
        /// Play the attached particle system
        /// </summary>
        public void Play()
        {
            particleSystem.Play();
        }
    }
}