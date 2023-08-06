using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>SquadSpawner</c> Handles the object creation side of spawning in a squad</summary>
/// Currently creates a squad of 5 LMG gunners with the player as a target, will spawn different kinds of squads in the future

public class SquadSpawner : MonoBehaviour
{
    public GameObject player;
    public ObjectPool ops;

    //Spawning Variables
    public int spawnDistance;
    public int spawnBiasAngle;

    SquadManager squadManager;


    // Start is called before the first frame update
    void Start()
    {
        ops = ObjectPool.Instance;
        player = GameObject.FindGameObjectWithTag("Player");
        squadManager = GameObject.FindObjectOfType<SquadManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO: Make this smarter, perhaps using the Wave.cs class
    public InfantrySquad SpawnInfantrySquad()
    {
        InfantrySquad s = new InfantrySquad(squadManager);
        s.SetTarget(player);
        for(int i = 0; i < 5; i++) //for now, a squad will always have 5 units
        {
            InfantryAI ai = SpawnNewEnemy(Enemy.Rifleman) as InfantryAI;
            s.AddToSquad(ai);
            squadManager.currentEnemies.Add(ai);
        }

        return s;
    }

    public VehicleSquad SpawnVehicleSquad()
    {
        VehicleSquad s = new VehicleSquad(squadManager);
        s.SetTarget(player);

        VehicleAI ai = SpawnNewEnemy(Enemy.Car) as VehicleAI;
        s.AddToSquad(ai);
        squadManager.currentEnemies.Add(ai);
        return s;
    }

    /// <summary>
    /// This will spawn an enemy of a specific type and then returns that enemy
    /// </summary>
    /// <param name="type"></param> TODO: Will abstractions in factory and eventually specify Enenemy Type, AI type, and Gun loadout
    public Ai SpawnNewEnemy(Enemy type)
    {
        GameObject enemy;
        Ai enemyAI;

        enemy = ops.SpawnFromPool(type, biasSpawnVector(), Quaternion.identity);

        //Init Enemy
        enemyAI = enemy.GetComponent<Ai>();
        enemyAI.NewLife();
        return enemyAI;
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
