using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemySpawner : MonoBehaviour
{

    public ObjectPool ops;
    public GameObject player;


    //Spawning Variables 
    public int spawnDistance;
    public int spawnBiasAngle;
    // Start is called before the first frame update
    void Start()
    {
        ops = ObjectPool.Instance;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// This will spawn an enemy of a specifi type 
    /// </summary>
    /// <param name="type"></param> TODO: Will abstractions in factory and eventually specify Enenemy Type, AI type, and Gun loadout
    public GameObject SpawnNewEnemy(Enemy type)
    {
        GameObject enemy;
        Ai enemyAI;
        switch (type)
        {
            case Enemy.Grunt:
                enemy = ops.SpawnFromPool("Grunt", biasSpawnVector(), Quaternion.identity);
                
                break;
            case Enemy.Rifelman:
                enemy = ops.SpawnFromPool("Rifleman", biasSpawnVector(), Quaternion.identity);
                
                break;
            case Enemy.Blank:
                enemy = ops.SpawnFromPool("Blank", biasSpawnVector(), Quaternion.identity);
                
                break;
            default:

                enemy = null;
                Debug.LogError("SpawnNew Enemy Returning Null");
                break;

                

        }

        enemyAI = enemy.GetComponent<Ai>();
        enemyAI.loadout(player);
        enemyAI.alive = true;

        return enemy;
    }

    /// <summary>
    /// Spawns 5 Riflemen & 2 Shotgunners
    /// </summary>
    public List<Ai> SpawnFirstWave(List<Ai> currentEnemies)
    {
        GameObject enemy;
        Ai enemyAI;
        for(int i = 0; i< 10; i++)
        {
            enemy = ops.SpawnFromPool("Rifleman", biasSpawnVector(), Quaternion.identity);
            enemyAI = enemy.GetComponent<Ai>();
            enemyAI.loadout(player);
            enemyAI.alive = true;
            currentEnemies.Add(enemyAI);
        }

        return currentEnemies;
    }

    
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


}


