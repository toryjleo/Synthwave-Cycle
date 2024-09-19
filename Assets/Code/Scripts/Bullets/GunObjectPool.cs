using EditorObject;
using System.Collections;
using System.Collections.Generic;
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

        private EditorObject.GunStats gunStats;
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
}