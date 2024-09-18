using EditorObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
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

        public void Play()
        {
            particleSystem.Play();
        }
    }
}