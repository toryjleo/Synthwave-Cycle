using EditorObject;
using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace Gun
{
    /// <summary>
    /// Component necessary for an object in the GunObjectPool
    /// </summary>
    public abstract class PoolableGunObject : Generic.Poolable
    {
        // TODO: Move down a level
        /// <summary>
        /// Set the poolable object to its initial state before spawning
        /// </summary>
        /// <param name="gunStats">The stats of this gun to use</param>
        public abstract void Init(EditorObject.GunStats gunStats);
    }

    /// <summary>
    /// A Unity Component works as an object pool for GunObjectPool objects. Adapted for use with guns.
    /// </summary>
    public class GunObjectPool : Generic.ObjectPool
    {

        protected GunStats GunStats 
        {
            get { return stats as GunStats; }
        }

        protected PoolableGunObject PoolableGunObject 
        {
            get { return prefab as PoolableGunObject; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stats">stats for this gun</param>
        /// <param name="prefab">prefab to instantiate</param>
        /// <param name="instantiateCount">number of times to instantiate prefab</param>
        public GunObjectPool(IPoolableInstantiateData stats, Generic.Poolable prefab, int instantiateCount) : base(stats, prefab, instantiateCount)
        {
            // Catch errors
            if ((base.prefab as PoolableGunObject) == null)
            {
                Debug.LogError("GunObjectPool prefab must have component of type 'PoolableGunObject'");
            }
            if (GunStats == null) 
            {
                Debug.LogError("GunObjectPool prefab must be passed stats of type 'EditorObject.GunStats'");
            }
        }

        /// <summary>Should handle all initialization for a new bullet instance.</summary>
        /// <returns>A new gameObject instance with a Poolable component.</returns>
        protected override Generic.Poolable CreateNewPoolableObject()
        {

            PoolableGunObject newObject = GameObject.Instantiate(prefab as PoolableGunObject, new Vector3(0, 0, 0), Quaternion.identity);
            // TODO: Move down a level
            newObject.Init(GunStats);
            newObject.gameObject.SetActive(false);
            newObject.Despawn += DespawnObject;
            return newObject;
        }
    }
}