using EditorObject;
using Generic;
using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
    /// <summary>
    /// A Unity Component works as an object pool for GunObjectPool objects. Adapted for use with Projectiles.
    /// </summary>
    public class ProjectileObjectPool : GunObjectPool
    {
        private Gun parent = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gunStats">stats for this gun</param>
        /// <param name="prefab">Prefab of projectile to fire</param>
        /// <param name="parent">Reference to gun who will be using this object pool</param>
        /// <param name="instantiateCount">number of times to instantiate prefab</param>
        public ProjectileObjectPool(GunStats gunStats, Poolable prefab, Gun parent, int instantiateCount) : base(gunStats, prefab, instantiateCount)
        {
            this.parent = parent;
        }

        /// <summary>Should handle all initialization for a new bullet instance.</summary>
        /// <returns>A new gameObject instance with a Poolable component.</returns>
        protected override Generic.Poolable CreateNewPoolableObject()
        {
            Poolable poolable = base.CreateNewPoolableObject();
            Projectile projectile = poolable.GetComponent<Projectile>();

            if (projectile)
            {
                projectile.notifyListenersHit += parent.HandleBulletHit;
            }
            else
            {
                Debug.LogError("ProjectileObjectPool needs to pool objects with Projectile components");
            }

            return poolable;
        }
    }
}