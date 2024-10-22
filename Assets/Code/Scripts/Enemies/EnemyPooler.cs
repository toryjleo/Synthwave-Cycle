using System.Collections;
using System.Collections.Generic;
using EditorObject;
using Generic;
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
    public Ai prefab;
    public TestAi testAi;
    public int poolSize;

    public Enemy GetTag()
    {
        return prefab.GetEnemyType();
    }
    ObjectPool objectPool;

    public static EnemyPooler Instance;

    private void Awake()
    {
        Instance = this;
    }


    // public List<Pool> pools; //Pools
    public Dictionary<Enemy, ObjectPool> poolDictionary; //These are the keys 


    //Creates Pools for each object type 
    void Start()
    {
        objectPool = new ObjectPool(testAi, prefab);
        objectPool.PoolObjects(5);
    }

    void Update()
    {
        ArrayList enemiesInWorld = objectPool.ObjectsInWorld;
        foreach (Ai ai in enemiesInWorld)
        {
            ai.ManualUpdate();
        }
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
        Ai objectToSpawn = (Ai)objectPool.SpawnFromPool();

        if (!objectToSpawn)
        {
            Debug.LogError("Object pool didn't pool AI type!");
        }

        return objectToSpawn;
    }
}
