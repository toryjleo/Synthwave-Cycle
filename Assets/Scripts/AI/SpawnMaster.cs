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
    public DLevel dl;
    public ScoreTracker scoreKeeper;
    public List<Ai> currentEnemies;


    private float spawnDistance = 80;
    

    void Start()
    {
        ops = ObjectPool.Instance;
        dl = DLevel.Instance;
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

        if (currentEnemies.Count < dl.dangerLevel)
        {
            //This method sees if all enemies have been Killed
            if (currentEnemies.Count == 0)
            {
                //Refill the screen with Enemies 
                SpawnFirstWave();
            } else
            {
                //Slowly Spawn more randos 
                SpawnNewEnemy(Enemy.Blank);
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
    private void SpawnNewEnemy(Enemy type)
    {
        GameObject enemy;
        switch (type)
        {
            case Enemy.Grunt:
                 enemy = ops.SpawnFromPool("Grunt", generateSpawnVector(), Quaternion.identity, player);
                currentEnemies.Add(enemy.GetComponentInChildren<Ai>());
                break;
            case Enemy.Rifelman:
                 enemy = ops.SpawnFromPool("Rifleman", generateSpawnVector(), Quaternion.identity, player);
                currentEnemies.Add(enemy.GetComponentInChildren<Ai>());
                break;
            case Enemy.Blank:
                enemy = ops.SpawnFromPool("Blank", biasSpawnVector((player.GetComponent<BikeScript>().velocity),20,100), Quaternion.identity, null);
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

        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);
        SpawnNewEnemy(Enemy.Blank);

    }
}
