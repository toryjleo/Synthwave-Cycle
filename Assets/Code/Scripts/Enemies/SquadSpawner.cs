using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnLocation
{
    Front,
    Behind,
    Sides,
    NotFront,
    Any
};

/// <summary>Class <c>SquadSpawner</c> Handles the object creation side of spawning in a squad</summary>
/// Currently creates a squad of 5 LMG gunners with the player as a target, will spawn different kinds of squads in the future
public class SquadSpawner : MonoBehaviour
{
    public GameObject player;
    public EnemyPooler ops;

    //Spawning Variables
    [SerializeField] private int spawnDistance;
    [SerializeField] private int spawnBiasAngle;

    // Start is called before the first frame update
    void Start()
    {
        ops = EnemyPooler.Instance;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void SpawnWave(List<Waves.WaveEnemyInfo> enemiesToSpawn)
    {
        foreach (Waves.WaveEnemyInfo enemyInfo in enemiesToSpawn)
        {
            SpawnNewEnemy(enemyInfo.enemyType, enemyInfo.spawnLocation, player.transform.position);
        }
    }

    /// <summary>
    /// This will spawn an enemy of a specific type and then returns that enemy
    /// </summary>
    /// <param name="type"></param> TODO: Will abstractions in factory and eventually specify Enemy Type, AI type, and Gun loadout
    public Ai SpawnNewEnemy(Enemy type, SpawnLocation loc, Vector3 targetLocation)
    {
        Ai enemy = ops.RetrieveFromPool(type);
        enemy.Spawn(biasSpawnVector(loc), targetLocation);

        //Initialize enemy health and life properties
        // TODO: Move NewLife to enemy's reset
        enemy.NewLife();
        return enemy;
    }


    #region biasSpawnVector
    /// <summary>
    /// This method returns a vector a set distance away from the player in an arc. With conditions specified in this class
    /// </summary>
    /// <returns></returns>
    public Vector3 biasSpawnVector(SpawnLocation loc)
    {
        Vector3 forwardVector = player.transform.forward;
        List<Vector3> vectors = new List<Vector3>();
        switch (loc)
        {
            case (SpawnLocation.Front):
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
        return biasSpawnVector(vectors[Random.Range(0, vectors.Count)], spawnBiasAngle, spawnDistance);

    }

    /// <summary>
    /// This method returns a vector
    /// </summary>
    /// <param name="bias"> this is the direction that the bike is already moving </param>
    /// <param name="angle"> the range of degrees that the vector can be rotated to ( 0 to 180 ) </param>
    /// <param name="distance"> the desired length of the spawn vector </param>
    /// <returns></returns>
    public Vector3 biasSpawnVector(Vector3 bias, int angle, int distance)
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
