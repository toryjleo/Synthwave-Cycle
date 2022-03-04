using System.Collections;
using System.Collections.Generic;
using System.Timers;
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
    public List<Ai> currentEnemies;


    private float spawnDistance = 80;
    public static int dangerLevel;
    public Timer xTimer;

    void Start()
    {
        ops = ObjectPool.Instance;

        //This timer increases the danger level and is used for determining the amount and difficulty of enemies being spawned
        xTimer = new Timer(3000);
        dangerLevel = 10;
        xTimer.AutoReset = true;
        xTimer.Enabled = true;
        xTimer.Elapsed += XTimer_Elapsed;

    }

    /// <summary>
    /// When xTimer Elapses every 3 seconds, increase the danger level by 1. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void XTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        dangerLevel++;
    }

    public Vector3 generateSpawnVector()
    {
        //TODO: add Spawn Bias 
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
            //This method sees if all enemies have been Killed
            if (currentEnemies.Count == 0)
            {
                //Refill the screen with Enemies 
                SpawnFirstWave();
            } else
            {
                //Slowly Spawn more randos 
                SpawnNewEnemy("Grunt");
            }
            
        }


        
    }


    /// <summary>
    /// This Method Checks how many enemies are currently alive in the scene, if any are dead it adds those to the score and Begins 
    /// the respawn countdown. Then it removes all dead enemies from the list of currently alive ones. 
    /// </summary>
    private void UpdateEnemyStates()
    {

        

        foreach (Ai a in currentEnemies)
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
                currentEnemies.Add(enemy.GetComponentInChildren<Ai>());
                break;
            case "Riflemen":
                 enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
                currentEnemies.Add(enemy.GetComponentInChildren<Ai>());
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
        currentEnemies.Add(enemy.GetComponentInChildren<Ai>());
        enemy = ops.SpawnFromPool("Grunt", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<Ai>());

        enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<Ai>());
        enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<Ai>());
        enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<Ai>());
        enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<Ai>());

    }
}
