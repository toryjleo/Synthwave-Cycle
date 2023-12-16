using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>SquadSpawner</c> Handles the object creation side of spawning in a squad</summary>
/// Currently creates a squad of 5 LMG gunners with the player as a target, will spawn different kinds of squads in the future

public class SquadSpawner : MonoBehaviour
{
    public GameObject player;
    public EnemyPool ops;

    //Spawning Variables
    [SerializeField] private int spawnDistance;
    [SerializeField] private int spawnBiasAngle;

    internal SquadManager squadManager;


    // Start is called before the first frame update
    void Start()
    {
        ops = EnemyPool.Instance;
        player = GameObject.FindGameObjectWithTag("Player");
        squadManager = GameObject.FindObjectOfType<SquadManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Squad SpawnSquad(Enemy aiType)
    {
        Ai enemyAi = SpawnNewEnemy(aiType);
        if(enemyAi is VehicleAI)
        {
            VehicleSquad squad = new VehicleSquad(squadManager);
            squad.SetTarget(player);
            enemyAi.SetTarget(player);
            squad.AddToSquad(enemyAi);
            squadManager.currentEnemies.Add(enemyAi);
            return squad;
        }
        else if (enemyAi is InfantryAI)
        {
            InfantrySquad s = new InfantrySquad(squadManager);
            s.SetTarget(player);
            s.AddToSquad(enemyAi);
            for (int i = 0; i < 4; i++) //for now, a squad will always have 5 units
            {
                enemyAi = SpawnNewEnemy(aiType) as InfantryAI;
                s.AddToSquad(enemyAi);
                squadManager.currentEnemies.Add(enemyAi);
            }
            return s;
        }
        else
        {
            return null;
        }
    }

    //TODO: Make this smarter, perhaps using the Wave.cs class
    public InfantrySquad SpawnInfantrySquad(Enemy type)
    {
        InfantrySquad s = new InfantrySquad(squadManager);
        s.SetTarget(player);
        for(int i = 0; i < 5; i++) //for now, a squad will always have 5 units
        {
            InfantryAI ai = SpawnNewEnemy(type) as InfantryAI;
            s.AddToSquad(ai);
            squadManager.currentEnemies.Add(ai);
        }
        return s;
    }

    public VehicleSquad SpawnVehicleSquad(Enemy type)
    {
        VehicleSquad s = new VehicleSquad(squadManager);
        s.SetTarget(player);

        VehicleAI ai = SpawnNewEnemy(type) as VehicleAI;
        s.AddToSquad(ai);
        squadManager.currentEnemies.Add(ai);
        return s;
    }

    internal void SpawnWave(List<Enemy> ais)
    {
        foreach (Enemy aiType in ais)
        {
            squadManager.RegisterSquad(SpawnSquad(aiType));
        }
    }

    /// <summary>
    /// This will spawn an enemy of a specific type and then returns that enemy
    /// </summary>
    /// <param name="type"></param> TODO: Will abstractions in factory and eventually specify Enenemy Type, AI type, and Gun loadout
    public Ai SpawnNewEnemy(Enemy type)
    {
        Ai enemy;

        enemy = ops.SpawnFromPool(type, biasSpawnVector(), Quaternion.identity);

        //Init Enemy
        enemy.NewLife();
        return enemy;
    }


    #region biasSpawnVector
    /// <summary>
    /// This method returns a vector a set dinstance away from the player in an arc. With conditions specified in this class
    /// </summary>
    /// <returns></returns>
    public Vector3 biasSpawnVector()
    {
        return biasSpawnVector(player.GetComponent<BikeScript>().ForwardVector(), spawnBiasAngle, spawnDistance);
    }
    /// <summary>
    /// This method returns a vector
    /// </summary>
    /// <param name="bias"> this is the direction that the bike is already moving </param>
    /// <param name="angle"> the range of degrees that the vector can be rotated to ( 0 to 180 ) </param>
    /// <param name="distance"> the desired lenght of the spawn vector </param>
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
