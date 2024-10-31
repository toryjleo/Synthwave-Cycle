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
    RamCar,
    SBomber
}

/// <summary>
/// EnemyPool holds collections of all the AI we might end up spawning in. 
/// They are instead enabled and put into place when they are ready to be used so we don't have to
/// spawn a new enemy in every time we need one
/// </summary>
public class EnemyPooler : MonoBehaviour, IResettable
{
    // Infantry pool
    public Ai prefab;
    public AiStats testAi;
    private ObjectPool infantryPool;

    //Vehicle pool
    public Ai vehiclePrefab;
    public AiStats testVehicleAi;
    private ObjectPool vehiclePool;

    private Vector3 wanderDirection = new Vector3(0, 0, 1);

    public static EnemyPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    //Creates Pools for each object type 
    void Start()
    {
        //TODO: PoolObjects needs an accurate estimate of pool size
        infantryPool = new ObjectPool(testAi, prefab);
        infantryPool.PoolObjects(5);

        vehiclePool = new ObjectPool(testVehicleAi, vehiclePrefab);
        vehiclePool.PoolObjects(5);

        wanderDirection = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
    }

    void FixedUpdate()
    {
        ArrayList infantryInWorld = infantryPool.ObjectsInWorld;
        foreach (Ai ai in infantryInWorld)
        {
            ai.ManualUpdate(infantryInWorld, wanderDirection);
        }

        ArrayList vehiclesInWorld = vehiclePool.ObjectsInWorld;
        foreach (Ai ai in vehiclesInWorld)
        {
            ai.ManualUpdate(vehiclesInWorld, wanderDirection);
        }

        // TODO: Add another foreach to update their positions AFTER they decide where they're going
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
        Ai objectToSpawn = null;
        switch (tag)
        {
            case Enemy.Rifleman:
                objectToSpawn = (Ai)infantryPool.SpawnFromPool();
                break;

            case Enemy.RamCar:
                objectToSpawn = (Ai)vehiclePool.SpawnFromPool();
                break;

            default:
                Debug.LogError("Enemy tag does not match existing enemy types!");
                break;
        }

        if (!objectToSpawn)
        {
            Debug.LogError("Object pool didn't pool AI type!");
        }

        return objectToSpawn;
    }

    public void ResetGameObject()
    {
        infantryPool.ResetGameObject();
        vehiclePool.ResetGameObject();
    }
}
