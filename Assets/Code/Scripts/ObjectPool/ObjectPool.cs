using EditorObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generic
{
    /// <summary>
    /// Component necessary for an object in the ObjectPool
    /// </summary>
    public abstract class Poolable : SelfWorldBoundsDespawn
    {
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

        /// <summary>
        /// Pooled objects that can be returned at a moment's notice
        /// </summary>
        private Queue<Poolable> objectsAwaitingSpawn;
        /// <summary>
        /// Objects that are tracked but are spawned in the world
        /// </summary>
        private ArrayList objectsInWorld;

        /// <summary>
        /// The class contructor
        /// </summary>
        /// <param name="prefab">The template object for this pool.</param>
        public ObjectPool(Poolable prefab)
        {
            this.prefab = prefab;

            if (objectsAwaitingSpawn == null)
            {
                objectsAwaitingSpawn = new Queue<Poolable>();
            }
            if (objectsInWorld == null)
            {
                objectsInWorld = new ArrayList();
            }
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
        protected abstract Poolable CreateNewPoolableObject();

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