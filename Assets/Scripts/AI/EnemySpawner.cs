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
    /// This will spawn an enemy of a Random type with hard code specifications located in this method. 
    /// TODO: Potentially rework this method someday. 
    /// </summary>
    /// <param name="type"></param> TODO: Will abstractions in factory and eventually specify Enenemy Type, AI type, and Gun loadout
    public Ai SpawnNewEnemy()
    {
        GameObject enemy;
        Ai enemyAI;
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                enemy = ops.SpawnFromPool(Enemy.Grunt.ToString(), biasSpawnVector(), Quaternion.identity);
                break;
            case 1:
                enemy = ops.SpawnFromPool(Enemy.Rifleman.ToString(), biasSpawnVector(), Quaternion.identity);
                break;
            case 2:
                enemy = ops.SpawnFromPool(Enemy.Ranger.ToString(), biasSpawnVector(), Quaternion.identity);
                break;
            default:
                enemy = ops.SpawnFromPool(Enemy.Cactus.ToString(), biasSpawnVector(), Quaternion.identity);
                break;
        }

        //Init Enemy 
        enemyAI = enemy.GetComponent<Ai>();
        enemyAI.Loadout(player);
        enemyAI.NewLife();
        return enemyAI;
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
    /// <summary>
    /// creates vector of Spawn distance lenght in a random 360 degree rotation. 
    /// </summary>
    /// <returns></returns>
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


