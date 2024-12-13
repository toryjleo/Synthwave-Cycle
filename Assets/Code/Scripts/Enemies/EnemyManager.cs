using System.Collections;
using System.Collections.Generic;
using EditorObject;
using Generic;
using UnityEngine;

// Enemy types
public enum Enemy
{
    Rifleman,
    Ranger,
    Shotgun,
    RamCar,
    SBomber
}

// Spawn locations relative to player
public enum SpawnLocation
{
    Front,
    Behind,
    Sides,
    NotFront,
    Any
}

public class BiasSpawnVector
{
    //Spawning Variables
    [SerializeField] private int spawnDistance = 150;
    [SerializeField] private int spawnBiasAngle = 33;

    #region BiasSpawnVector
    /// <summary>
    /// This method returns a vector a set distance away from the player in an arc. With conditions specified in this class
    /// </summary>
    /// <returns></returns>
    public Vector3 BiasSpawnVectorLocation(SpawnLocation loc, GameObject player)
    {
        Vector3 forwardVector = player.transform.forward;
        List<Vector3> vectors = new List<Vector3>();
        switch (loc)
        {
            case SpawnLocation.Front:
                vectors.Add(forwardVector);
                break;
            case SpawnLocation.Sides:
                vectors.Add(Quaternion.AngleAxis(90, Vector3.up) * forwardVector);
                vectors.Add(Quaternion.AngleAxis(-90, Vector3.up) * forwardVector);
                break;
            case SpawnLocation.Behind:
                vectors.Add(Quaternion.AngleAxis(180, Vector3.up) * forwardVector);
                break;
            case SpawnLocation.NotFront:
                vectors.Add(Quaternion.AngleAxis(90, Vector3.up) * forwardVector);
                vectors.Add(Quaternion.AngleAxis(-90, Vector3.up) * forwardVector);
                vectors.Add(Quaternion.AngleAxis(180, Vector3.up) * forwardVector);
                break;
            case SpawnLocation.Any:
                vectors.Add(forwardVector);
                vectors.Add(Quaternion.AngleAxis(90, Vector3.up) * forwardVector);
                vectors.Add(Quaternion.AngleAxis(-90, Vector3.up) * forwardVector);
                vectors.Add(Quaternion.AngleAxis(180, Vector3.up) * forwardVector);
                break;
        }
        return BiasSpawnVectorCalculation(vectors[Random.Range(0, vectors.Count)], spawnBiasAngle, spawnDistance, player);
    }

    /// <summary>
    /// This method returns a vector
    /// </summary>
    /// <param name="bias"> this is the direction that the bike is already moving </param>
    /// <param name="angle"> the range of degrees that the vector can be rotated to ( 0 to 180 ) </param>
    /// <param name="distance"> the desired length of the spawn vector </param>
    /// <returns></returns>
    public Vector3 BiasSpawnVectorCalculation(Vector3 bias, int angle, int distance, GameObject player)
    {
        if (bias == new Vector3(0, 0, 0))// defaut case if bike isn't moving
        {
            bias = new Vector3(0, 0, 1);
        }

        Vector3 spawnVector = bias;
        Quaternion q = Quaternion.Euler(0, Random.Range(-angle, angle), 0);

        spawnVector = q * spawnVector;

        spawnVector.Normalize();
        spawnVector *= distance;
        spawnVector += player.transform.position;
        return spawnVector;
    }
    #endregion
}

/// <summary>
/// Handles enemy pooling and spawning logic, controls the enemies' manualUpdate call as well
/// </summary>
public class EnemyManager : MonoBehaviour, IResettable
{
    #region EnemyPooler
    /// <summary>
    /// EnemyPool holds collections of all the AI we might end up spawning in. 
    /// They are instead enabled and put into place when they are ready to be used so we don't have to
    /// spawn a new enemy in every time we need one
    /// </summary>
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
    #endregion

    #region Spawner
    /// <summary>
    /// Class <c>EnemySpawner</c> Handles the object creation side of spawning in a squad
    /// </summary>
    public GameObject player;

    private BiasSpawnVector spawnVector;

    internal void SpawnWave(List<Waves.WaveEnemyInfo> enemiesToSpawn)
    {
        foreach (Waves.WaveEnemyInfo enemyInfo in enemiesToSpawn)
        {
            for (int i = 0; i < enemyInfo.enemyAmount; i++)
            {
                SpawnNewEnemy(enemyInfo.enemyType, enemyInfo.spawnLocation, player.transform.position);
            }
        }
    }

    /// <summary>
    /// This will spawn an enemy of a specific type and then returns that enemy
    /// </summary>
    /// <param name="type"></param> TODO: Will abstractions in factory and eventually specify Enemy Type, AI type, and Gun loadout
    public Ai SpawnNewEnemy(Enemy type, SpawnLocation loc, Vector3 targetLocation)
    {
        Ai enemy = RetrieveFromPool(type);
        enemy.Spawn(spawnVector.BiasSpawnVectorLocation(loc, player), targetLocation);

        return enemy;
    }

    #endregion

    #region EnemyManager
    public static EnemyManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnVector = new BiasSpawnVector();
        player = GameObject.FindGameObjectWithTag("Player");

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

    // Update is called once per frame
    void Update()
    {

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
    #endregion
}
