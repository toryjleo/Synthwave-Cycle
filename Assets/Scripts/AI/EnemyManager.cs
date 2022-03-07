using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;



/// <summary>
/// This class Controlls the rate at which enemies are spawned and knows which enemies are in the scene 
/// </summary>
public class EnemyManager : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemySpawner enemySpawner;
    
    public DLevel dl;
    public ScoreTracker scoreKeeper;
    public List<Ai> currentEnemies;

    void Start()
    {

        dl = DLevel.Instance;
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
                currentEnemies = enemySpawner.SpawnFirstWave(currentEnemies);
                
            }
            else
            {
                //Slowly Spawn more randos 
                currentEnemies.Add(enemySpawner.SpawnNewEnemy(Enemy.Rifelman).GetComponent<Ai>());
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
}



