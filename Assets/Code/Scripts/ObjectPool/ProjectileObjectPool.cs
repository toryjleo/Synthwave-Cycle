using EditorObject;
using Generic;
using Gun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
    /// <summary>
    /// A Unity Component works as an object pool for GunObjectPool objects. Adapted for use with Projectiles.
    /// </summary>
    public class ProjectileObjectPool : Generic.ObjectPool
    {
        BulletHitHandler hitHandler = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gunStats">stats for this gun</param>
        /// <param name="prefab">Prefab of projectile to fire</param>
        /// <param name="hitHandler">Reference to gun who will be using this object pool</param>
        /// <param name="instantiateCount">number of times to instantiate prefab</param>
        public ProjectileObjectPool(GunStats gunStats, Poolable prefab, BulletHitHandler hitHandler, int instantiateCount) : base(gunStats, prefab, instantiateCount)
        {
            this.hitHandler = hitHandler;
        }

        /// <summary>Should handle all initialization for a new bullet instance.</summary>
        /// <returns>A new gameObject instance with a Poolable component.</returns>
        protected override Generic.Poolable CreateNewPoolableObject()
        {
            Poolable poolable = base.CreateNewPoolableObject();
            Projectile projectile = poolable.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.notifyListenersHit += hitHandler;
            }
            else
            {
                Debug.LogError("ProjectileObjectPool needs to pool objects with Projectile components");
            }

            return poolable;
        }
    }
}