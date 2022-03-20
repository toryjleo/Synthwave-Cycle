using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class is in charge of spawning enemies into the scene. 
/// 
/// Needs reference to Player to spawn objects in relation to player 
/// 
/// Needs reference to ObjectPool to spawn in pooled objects. 
/// 
/// You can Change spawn Distance and Angle to edit spawning. 
/// </summary>

public class EnemySpawner : MonoBehaviour
{

    public ObjectPool ops;
    public GameObject player;


    //Spawning Variables 
    public int spawnDistance;
    public int spawnBiasAngle;



    void Start()
    {
        ops = ObjectPool.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// This will spawn an enemy of a specific type and then returns that enemy 
    /// </summary>
    /// <param name="type"></param> TODO: Will abstractions in factory and eventually specify Enenemy Type, AI type, and Gun loadout
    public GameObject SpawnNewEnemy(Enemy type)
    {
        GameObject enemy;
        Ai enemyAI;

        enemy = ops.SpawnFromPool(type.ToString(), biasSpawnVector(), Quaternion.identity);


        //Init Enemy 
        enemyAI = enemy.GetComponent<Ai>();
        enemyAI.Loadout(player);
        enemyAI.NewLife();
        return enemy;
    }


    /// <summary>
    /// This Method is called at the beginning of the game to spawn in the first wave. 
    /// </summary>
    public List<Ai> SpawnFirstWave(List<Ai> currentEnemies)
    {
        GameObject enemy;
        Ai enemyAI;
        for(int i = 0; i< 10; i++)
        {
            enemy = ops.SpawnFromPool("Rifleman", generateSpawnVector(), Quaternion.identity);


            enemyAI = enemy.GetComponent<Ai>();
            enemyAI.Loadout(player);
            enemyAI.NewLife();
            currentEnemies.Add(enemyAI);
        }

        return currentEnemies;
    }


    //These methods generate spawn vectors 
    #region Spawning Vector Maths 
    public Vector3 biasSpawnVector()
    {
        return biasSpawnVector(player.GetComponent<BikeScript>().velocity, spawnBiasAngle, spawnDistance);
    }

    public Vector3 biasSpawnVector(Vector3 bias, int angle, int distance)
    {
        if (bias == new Vector3(0, 0, 0))
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

    public Vector3 generateSpawnVector()
    {
        //TODO: add Spawn Bias 
        Vector3 spawnVector = new Vector3(0, 0, spawnDistance);
        Quaternion ranRot = Quaternion.Euler(0, Random.Range(0, 359), 0);

        spawnVector = ranRot * spawnVector;
        spawnVector += player.transform.position;
        return spawnVector;
    }

#endregion 

}


