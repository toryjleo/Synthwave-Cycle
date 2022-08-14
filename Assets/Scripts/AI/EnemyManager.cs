using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;




/// <summary>
/// This class Controlls the rate at which enemies are spawned and knows which enemies are in the scene 
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public EnemySpawner enemySpawner; //Reference to Script in charge of spawning Enemies    
    public DLevel dl; //Danger Level Timer 
    public ScoreTracker scoreKeeper;


    public List<Ai> currentEnemies; //This is a list of Ai that are currently active in the scene. 
    

    void Start()
    {
        dl = DLevel.Instance;
    }

    void Update() //TODO add new spawning behavior 
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
                //Slowly Spawn more riflemen as the Danger Level increases 
                //TODO spawn a random Enemy 
                //currentEnemies.Add(enemySpawner.SpawnNewEnemy());
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
            if (a.IsAlive())
            {
                //Do Alive things
                a.Seperate(currentEnemies);
            }
            else
            {
                //Do Death Things 
                scoreKeeper.AddToScore((int)a.GetScore());
                //TODO: ADD Gore and soundeffects here? 
            }
        }
        currentEnemies.RemoveAll(a => a.alive == false); //UNF this shit is so sexy
        currentEnemies.RemoveAll(a => a.isActiveAndEnabled == false);
        
        
    }


}



