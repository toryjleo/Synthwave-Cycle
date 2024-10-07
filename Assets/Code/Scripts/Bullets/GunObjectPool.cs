using EditorObject;
using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace Gun
{
    public abstract class PoolableGunObject : Generic.Poolable
    {

        public abstract void Init(EditorObject.GunStats gunStats);
    }

    public class GunObjectPool : Generic.ObjectPool
    {
        private const int INFINITE_AMMO_COUNT = 200;

        private EditorObject.GunStats gunStats = null;

        public GunObjectPool(GunStats gunStats, Generic.Poolable bulletPrefab) : base(bulletPrefab)
        {
            this.gunStats = gunStats;

            if (gunStats.InfiniteAmmo)
            {
                instantiateCount = INFINITE_AMMO_COUNT * gunStats.ProjectileCountPerShot;
            }
            else
            {
                instantiateCount = gunStats.AmmoCount * gunStats.ProjectileCountPerShot;
            }

            // Catch errors
            if ((prefab as PoolableGunObject) == null)
            {
                Debug.LogError("GunObjectPool prefab must have component of type 'PoolableGunObject'");
            }
        }

        /// <summary>Should handle all initialization for a new bullet instance.</summary>
        /// <returns>A new gameObject instance with a Poolable component.</returns>
        protected override Generic.Poolable CreateNewPoolableObject()
        {
            PoolableGunObject newObject = GameObject.Instantiate(prefab as PoolableGunObject, new Vector3(0, 0, 0), Quaternion.identity);
            newObject.Init(gunStats);
            newObject.gameObject.SetActive(false);
            newObject.Despawn += DespawnObject;
            return newObject;
        }
    }

    public class ProjectileObjectPool : GunObjectPool
    {
        private Gun parent = null;

        public ProjectileObjectPool(GunStats gunStats, Poolable bulletPrefab, Gun parent) : base(gunStats, bulletPrefab)
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