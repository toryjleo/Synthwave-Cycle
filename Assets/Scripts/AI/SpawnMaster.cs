using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class Controlls the rate at which enemies are spawned and knows which enemies are in the scene 
/// </summary>
public class SpawnMaster : MonoBehaviour
{
    // Start is called before the first frame update
    public ObjectPool ops;
    public GameObject player;
    public ScoreTracker scoreKeeper;
    public List<AiTemplate> currentEnemies;
    private float spawnDistance = 80;
    private int dangerLevel = 10;

    void Start()
    {
        ops = ObjectPool.Instance;
    }

    public Vector3 generateSpawnVector()
    {
        //TODO:will have to create a general spawn method in the future so as not to doop code HERE&&HERE1
        Vector3 spawnVector = new Vector3(0,0, spawnDistance);
        Quaternion ranRot = Quaternion.Euler(0, Random.Range(0, 359), 0);

        spawnVector = ranRot * spawnVector;
        spawnVector += player.transform.position;
        return spawnVector;
    }

    // Update is called once per frame
    void Update()
    {        
        UpdateEnemyStates();
        
        if(currentEnemies.Count < dangerLevel)
        {
            SpawnNewEnemy("Grunt");
        }


        
    }

    /// <summary>
    /// This Method Checks how many enemies are currently alive in the scene, if any are dead it adds those to the score and Begins 
    /// the respawn countdown. Then it removes all dead enemies from the list of currently alive ones. 
    /// </summary>
    private void UpdateEnemyStates()
    {

        //This method sees if this is the first wave of enemies. 
        if(currentEnemies.Count == 0)
        {
            SpawnFirstWave();
        } 

        foreach (AiTemplate a in currentEnemies)
        {
            if (a.isAlive())
            {
                //Do Alive things
                a.seperate(currentEnemies);
            }
            else
            {
                //Do Death Things 
                scoreKeeper.AddToScore((int)a.getScore());
                //TODO: ADD Gore and soundeffects here? 
            }
        }
        currentEnemies.RemoveAll(a => a.alive == false); //UNF this shit is so sexy
        currentEnemies.RemoveAll(a => a.isActiveAndEnabled == false);
    }

    /// <summary>
    /// This will spawn an enemy of a specifi type 
    /// </summary>
    /// <param name="type"></param> TODO: Will abstractions in factory and eventually specify Enenemy Type, AI type, and Gun loadout
    private void SpawnNewEnemy(string type)
    {
        GameObject enemy;
        switch (type)
        {
            case "Grunt":
                 enemy = ops.SpawnFromPool("Grunt", generateSpawnVector(), Quaternion.identity, player);
                currentEnemies.Add(enemy.GetComponentInChildren<AiTemplate>());
                break;
            case "Riflemen":
                 enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
                currentEnemies.Add(enemy.GetComponentInChildren<AiTemplate>());
                break;
            default:

                break;
            
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
