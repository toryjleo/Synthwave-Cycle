using EditorObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generic
{
    public interface IPoolableInstantiateData {};

    /// <summary>
    /// Component necessary for an object in the ObjectPool
    /// </summary>
    public abstract class Poolable : SelfWorldBoundsDespawn
    {
        /// <summary>
        /// Set the poolable object to its initial state before spawning
        /// </summary>
        /// <param name="gunStats">The stats of this gun to use</param>
        public abstract void Init(IPoolableInstantiateData stats);

        /// <summary>
        /// Initialize all data that changes throughout a level
        /// </summary>
        public abstract void Reset();
    }


    /// <summary> A Unity Component works as an object pool for Poolable objects.</summary>
    public abstract class ObjectPool : IResettable
    {
        /// <summary>
        /// Prefab to duplicate
        /// </summary>
        protected Poolable prefab;
        /// <summary>
        /// Number of prefabs to instantiate
        /// </summary>
        protected int instantiateCount = 0;

        protected IPoolableInstantiateData instantiateData;

        /// <summary>
        /// Pooled objects that can be returned at a moment's notice
        /// </summary>
        private Queue<Poolable> objectsAwaitingSpawn;
        /// <summary>
        /// Objects that are tracked but are spawned in the world
        /// </summary>
        private ArrayList objectsInWorld;



        protected IPoolableInstantiateData stats = null;

        /// <summary>
        /// The class contructor
        /// </summary>
        /// <param name="prefab">The template object for this pool.</param>
        /// <param name="instantiateCount">number of times to instantiate prefab</param>
        public ObjectPool(IPoolableInstantiateData stats, Poolable prefab, int instantiateCount)
        {

            this.stats = stats;
            this.instantiateCount = instantiateCount;

            this.prefab = prefab;

            if (objectsAwaitingSpawn == null)
            {
                objectsAwaitingSpawn = new Queue<Poolable>();
            }
            if (objectsInWorld == null)
            {
                objectsInWorld = new ArrayList();
            }
        }

        public void PoolObjects()
        {
            while (objectsAwaitingSpawn.Count < instantiateCount)
            {
                Poolable newBullet = CreateNewPoolableObject();
                objectsAwaitingSpawn.Enqueue(newBullet);
            }
        }

        /// <summary>
        /// Instantiates a new poolable object
        /// </summary>
        /// <returns>A newly instantiated GameObject with a poolable component</returns>
        /// <summary>Should handle all initialization for a new Poolable instance.</summary>
        /// <returns>A new gameObject instance with a Poolable component.</returns>
        protected virtual Generic.Poolable CreateNewPoolableObject()
        {
            Poolable newObject = GameObject.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
            newObject.Init(stats);
            newObject.gameObject.SetActive(false);
            newObject.Despawn += DespawnObject;
            return newObject;
        }

        /// <summary>Returns an instance of a Poolable object from the pool.</summary>
        /// <returns>A currently unused Poolable object.</returns>
        public Poolable SpawnFromPool()
        {
            if (objectsAwaitingSpawn.Count == 0)
            {
                Poolable newObject = CreateNewPoolableObject();
                objectsAwaitingSpawn.Enqueue(newObject);
            }

            Poolable objectToSpawn = objectsAwaitingSpawn.Dequeue();
            objectsInWorld.Add(objectToSpawn);

            // Check if object already in world
            objectToSpawn.Reset();
            objectToSpawn.gameObject.SetActive(true);

            return objectToSpawn;
        }

        /// <summary>
        /// Handles SelfDespawn.Despawn. Sets object to inactive and adds it to queue.
        /// </summary>
        /// <param name="objectToDespawn">The object which called Despawn that passes itself into the Despawn method that needs to 
        /// be added back to the pool.</param>
        public void DespawnObject(SelfDespawn objectToDespawn)
        {
            objectToDespawn.gameObject.SetActive(false);
            objectsInWorld.Remove((objectToDespawn as Poolable));
            objectsAwaitingSpawn.Enqueue(objectToDespawn as Poolable);
        }

        /// <summary>
        /// Returns all object in the world to the queue
        /// </summary>
        public void ResetGameObject()
        {
            for (int i = 0; i < objectsInWorld.Count; i++)
            {
                Poolable b = (Poolable)objectsInWorld[i];
                b.Reset();
                objectsInWorld.Remove(b);
                objectsAwaitingSpawn.Enqueue(b);
            }
        }
    }
}