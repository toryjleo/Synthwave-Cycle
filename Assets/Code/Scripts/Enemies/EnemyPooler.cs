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
    #region Infantry Pools
    // Rifleman pool
    public Ai riflemanPrefab;
    public AiStats riflemanAi;
    private ObjectPool riflemanPool;

    // Ranger pool
    public Ai rangerPrefab;
    public AiStats rangerAi;
    private ObjectPool rangerPool;

    // Shotgunner pool
    public Ai shotgunnerPrefab;
    public AiStats shotgunnerAi;
    private ObjectPool shotgunnerPool;

    #endregion

    #region Vehicle Pools
    // Ram Car pool
    public Ai ramCarPrefab;
    public AiStats ramCarAi;
    private ObjectPool ramCarPool;

    // S Bomber pool
    public Ai sbomberPrefab;
    public AiStats sbomberAi;
    private ObjectPool sbomberPool;
    #endregion

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
        riflemanPool = new ObjectPool(riflemanAi, riflemanPrefab);
        riflemanPool.PoolObjects(10);

        shotgunnerPool = new ObjectPool(shotgunnerAi, shotgunnerPrefab);
        shotgunnerPool.PoolObjects(10);

        rangerPool = new ObjectPool(rangerAi, rangerPrefab);
        rangerPool.PoolObjects(10);

        ramCarPool = new ObjectPool(ramCarAi, ramCarPrefab);
        ramCarPool.PoolObjects(5);

        sbomberPool = new ObjectPool(sbomberAi, sbomberPrefab);
        sbomberPool.PoolObjects(5);

        wanderDirection = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
    }

    void FixedUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;

        ArrayList riflemenInWorld = riflemanPool.ObjectsInWorld;
        foreach (Ai ai in riflemenInWorld)
        {
            ai.ManualUpdate(riflemenInWorld, wanderDirection, fixedDeltaTime);
        }

        ArrayList shotgunnersInWorld = shotgunnerPool.ObjectsInWorld;
        foreach (Ai ai in shotgunnersInWorld)
        {
            ai.ManualUpdate(shotgunnersInWorld, wanderDirection, fixedDeltaTime);
        }

        ArrayList rangersInWorld = rangerPool.ObjectsInWorld;
        foreach (Ai ai in rangersInWorld)
        {
            ai.ManualUpdate(rangersInWorld, wanderDirection, fixedDeltaTime);
        }

        ArrayList ramCarsInWorld = ramCarPool.ObjectsInWorld;
        foreach (Ai ai in ramCarsInWorld)
        {
            ai.ManualUpdate(ramCarsInWorld, wanderDirection, fixedDeltaTime);
        }

        ArrayList sbombersInWorld = sbomberPool.ObjectsInWorld;
        foreach (Ai ai in sbombersInWorld)
        {
            ai.ManualUpdate(sbombersInWorld, wanderDirection, fixedDeltaTime);
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
                objectToSpawn = (Ai)riflemanPool.SpawnFromPool();
                break;

            case Enemy.Shotgun:
                objectToSpawn = (Ai)shotgunnerPool.SpawnFromPool();
                break;

            case Enemy.Ranger:
                objectToSpawn = (Ai)rangerPool.SpawnFromPool();
                break;

            case Enemy.RamCar:
                objectToSpawn = (Ai)ramCarPool.SpawnFromPool();
                break;

            case Enemy.SBomber:
                objectToSpawn = (Ai)sbomberPool.SpawnFromPool();
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
        riflemanPool.ResetGameObject();
        shotgunnerPool.ResetGameObject();
        rangerPool.ResetGameObject();
        ramCarPool.ResetGameObject();
        sbomberPool.ResetGameObject();
    }
}
