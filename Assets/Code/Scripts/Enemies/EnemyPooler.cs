using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Enemy
{
    Rifleman,
    Ranger,
    Shotgun,
    Bike,
    Sniper,
    Dog,
    Cactus,
    RamCar,
    SBomber
}

/// <summary>
/// EnemyPool holds collections of all the AI we might end up spawning in. 
/// They are instead enabled and put into place when they are ready to be used so we don't have to
/// spawn a new enemy in every time we need one
/// </summary>
public class EnemyPooler : MonoBehaviour
{
    /// <summary>
    /// Creates a single Pool of objects for Enemies, each with their own Enemy Tag, prefab, and predetermined size.
    /// </summary>
    [System.Serializable]
    public class Pool //One individual pool of objects that has a tag, a prefab, and a size 
    {
        public Ai prefab;
        public int poolSize;

        public Pool(Ai Prefab, int Size)
        {
            prefab = Prefab;
            poolSize = Size;
        }

        public Enemy GetTag()
        {
            return prefab.GetEnemyType();
        }
    }

    public static EnemyPooler Instance;

    private void Awake()
    {
        Instance = this;
    }


    public List<Pool> pools; //Pools
    public Dictionary<Enemy, Queue<Ai>> poolDictionary; //These are the keys 


    //Creates Pools for each object type 
    void Start()
    {
        //Create Dictionary of tags for each if the pools
        poolDictionary = new Dictionary<Enemy, Queue<Ai>>();



        foreach (Pool pool in pools)
        {
            Queue<Ai> objectPool = new Queue<Ai>();

            //instantiate the objects with 
            for (int i = 0; i < pool.poolSize; i++)
            {
                Ai obj = Instantiate(pool.prefab);
                obj.InitStateController();
                obj.Despawn += ObjAi_Despawn;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.GetTag(), objectPool);
        }
    }

    //This Method is called When the Entity dies 
    private void ObjAi_Despawn(SelfDespawn entity)
    {
        //TODO fuuuuck, figure out how to use the enum to fix all of this shiiiit 
        entity.gameObject.SetActive(false);
        //poolDictionary.Enqueue(entity as Ai); // Make sure this is bullet in the future
        //Debug.Log("Despawned, Size of queue: " + bulletQueue.Count);
    }


    /// <summary>
    /// Spawns in a single enemy AI based on the given tag
    /// </summary>
    /// <param name="tag">Which enemy type to spawn</param>
    /// <param name="position">Spawn location for the enemy</param>
    /// <param name="playerLocation">Player's current location, the direction the enemy will spawn facing</param>
    /// <returns></returns>
    public Ai RetrieveFromPool(Enemy tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }

        // Moving the newly spawned enemy to the back of the queue
        Ai objectToSpawn = poolDictionary[tag].Dequeue();

        // objectToSpawn.transform.position = position;
        // objectToSpawn.transform.rotation = Quaternion.LookRotation(playerLocation - position);
        // objectToSpawn.gameObject.SetActive(true);
        // objectToSpawn.Spawn(position, playerLocation);

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }




}
