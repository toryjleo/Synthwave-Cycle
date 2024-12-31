using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Generic
{
    /// <summary>
    /// Used in initializing a Poolable object
    /// </summary>
    public interface IPoolableInstantiateData { };

    /// <summary>
    /// Component necessary for an object in the ObjectPool
    /// </summary>
    public abstract class Poolable : SelfWorldBoundsDespawn
    {
        /// <summary>
        /// Set the poolable object to its initial state before spawning. Does not need to enable
        /// or disable GameObject. That is handled by the ObjectPool.
        /// </summary>
        /// <param name="stats">The stats to apply to this poolable object</param>
        public abstract void Init(IPoolableInstantiateData stats);

        /// <summary>
        /// Initialize all data that changes throughout a level. Does not need to enable or 
        /// disable GameObject. That is handled by the ObjectPool.
        /// </summary>
        public abstract void Reset();
    }
}