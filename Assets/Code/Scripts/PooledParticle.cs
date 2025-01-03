using EditorObject;
using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
    /// <summary>
    /// Defines behavior that a particle must exhibit when used on a gun.
    /// Requires: ParticleSystem Component
    /// </summary>
    public class PooledParticle : Poolable
    {
        private ParticleSystem particleSystem;
        private bool isSpawnedAndPlaying = false;

        public override void Init(IPoolableInstantiateData data)
        {
            particleSystem = GetComponent<ParticleSystem>();
            isSpawnedAndPlaying = false;

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

        public override void Update()
        {
            base.Update();

            if (isSpawnedAndPlaying && particleSystem != null && !particleSystem.isPlaying) 
            {
                isSpawnedAndPlaying = false;
                OnDespawn();
            }
        }

        public override void Reset()
        {
            isSpawnedAndPlaying = false;
        }

        /// <summary>
        /// Play the attached particle system
        /// </summary>
        /// <param name="position">The new position of the particle to play</param>
        /// <param name="forward">The new forward of this particle</param>
        public void Play(Vector3 position, Vector3 forward)
        {
            transform.position = position;
            transform.forward = forward;

            particleSystem.Play();
            isSpawnedAndPlaying = true;
        }
    }
}