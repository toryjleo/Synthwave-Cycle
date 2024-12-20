using EditorObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generic
{

    /// <summary> Works as an object pool for Poolable objects.</summary>
    public class ObjectPool
    {
        /// <summary>
        /// Prefab to duplicate
        /// </summary>
        protected Poolable prefab;

        /// <summary>
        /// Data used to initialize a Poolable object
        /// </summary>
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

        public ArrayList ObjectsInWorld { get => objectsInWorld; }

        private bool NoObjectsAwaitingSpawn 
        {
            get => objectsAwaitingSpawn.Count == 0;
        }

        private bool NoObjectsInWorld 
        {
            get => objectsInWorld.Count == 0;
        }

        /// <summary>
        /// The class contructor
        /// </summary>
        /// <param name="prefab">The template object for this pool.</param>
        public ObjectPool(IPoolableInstantiateData stats, Poolable prefab)
        {

            this.stats = stats;

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

        /// <summary>
        /// Must be called after constructor. Fills object pool with objects
        /// </summary>
        /// <param name="instantiateCount">number of times to instantiate prefab</param>
        public void PoolObjects(int instantiateCount)
        {
            if (objectsAwaitingSpawn != null && objectsAwaitingSpawn.Count == 0)
            {
                while (objectsAwaitingSpawn.Count < instantiateCount)
                {
                    Poolable newBullet = CreateNewPoolableObject();
                    objectsAwaitingSpawn.Enqueue(newBullet);
                }
            }
            else
            {
                Debug.LogWarning("Trying to pool objects multiple times");
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
            if (NoObjectsAwaitingSpawn && NoObjectsInWorld) 
            {
                Debug.LogError("Trying to spawn objects before calling PoolObjects()");
            }

            if (NoObjectsAwaitingSpawn)
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
            // NOTE: Make sure to not trigger despawn event from SelfWorldDespawn in this method.
            // It will remove itself from objectsInWorld.
            for (int i = 0; i < objectsInWorld.Count; i++)
            {
                Poolable b = (Poolable)objectsInWorld[i];
                b.Reset();
                objectsAwaitingSpawn.Enqueue(b);
            }
            objectsInWorld = new ArrayList();
        }
    }
}