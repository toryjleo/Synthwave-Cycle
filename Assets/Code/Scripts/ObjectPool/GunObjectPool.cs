using EditorObject;
using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace Gun
{

    /// <summary>
    /// A Unity Component works as an object pool for GunObjectPool objects. Adapted for use with guns.
    /// </summary>
    public class GunObjectPool : Generic.ObjectPool
    {

        protected GunStats GunStats 
        {
            get { return stats as GunStats; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stats">stats for this gun</param>
        /// <param name="prefab">prefab to instantiate</param>
        /// <param name="instantiateCount">number of times to instantiate prefab</param>
        public GunObjectPool(IPoolableInstantiateData stats, Generic.Poolable prefab, int instantiateCount) : base(stats, prefab, instantiateCount)
        {
            if (GunStats == null) 
            {
                Debug.LogError("GunObjectPool prefab must be passed stats of type 'EditorObject.GunStats'");
            }
        }
    }
}