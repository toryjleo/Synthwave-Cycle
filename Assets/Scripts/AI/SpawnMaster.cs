using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class Controlls the rate at which enemies are spawned and knows which enemies are in the scene 
/// </summary>
public class SpawnMaster : MonoBehaviour
{
    // Start is called before the first frame update
    public Wave ops;
    public GameObject player;
    public ScoreTracker scoreKeeper;
    public List<AiTemplate> currentEnemies;
    private float spawnDistance = 80;
    private int dangerLevel = 0;

    void Start()
    {
        ops = Wave.Instance;

        //GameObject enemy = ops.SpawnFromPool("Grunt", generateSpawnVector(), Quaternion.identity, player);
        //currentEnemies.Add(enemy.GetComponentInChildren<AiTemplate>());
    }

    public Vector3 generateSpawnVector()
    {
        //TODO:will have to create a general spawn method in the future so as not to doop code HERE&&HERE1
        Vector3 spawnVector = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + spawnDistance);
        Quaternion ranRot = Quaternion.Euler(0, Random.Range(0, 359), 0);

        spawnVector = ranRot * spawnVector;
        return spawnVector;
    }

    // Update is called once per frame
    void Update()
    {
        
        CheckIfEnoughEnemies(dangerLevel);

        
    }

    /// <summary>
    /// This Method Checks how many enemies are currently alive in the scene and increases the number of enemies accordingly. 
    /// </summary>
    /// <param name="dangerLevel"></param> This is the number of enemies nessicary to be spawned. 
    private void CheckIfEnoughEnemies(int dangerLevel)
    {
        if(currentEnemies.Count == 0)
        {
            SpawnFirstWave();
        }

        foreach (AiTemplate a in currentEnemies)
        {
            if (a.isAlive())
            {
                //Do Alive things
                //a.seperate(currentEnemies);
            }
            else
            {
                //Do Death Things 
                scoreKeeper.AddToScore((int)a.getScore());
                //currentEnemies.Remove(a); //TODO figure out why this causes eneumeration Error and how to fix it 
            }
        } 
    }

    /// <summary>
    /// Spawns 5 Riflemen & 2 Shotgunners
    /// </summary>
    private void SpawnFirstWave()
    {
        
        GameObject enemy = ops.SpawnFromPool("Grunt", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<AiTemplate>());
        enemy = ops.SpawnFromPool("Grunt", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<AiTemplate>());

        enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<AiTemplate>());
        enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<AiTemplate>());
        enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<AiTemplate>());
        enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<AiTemplate>());

    }
}
