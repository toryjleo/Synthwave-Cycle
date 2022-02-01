using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolSpawner : MonoBehaviour
{

    public GruntAI objectToPool; 
    public Gun gunToPool;
    public GameObject player;
    public ScoreTracker scoreKeeper;
    private List<GruntAI> pool;
    public float size;
    public float spawnDistance = 30; //the number of units away from the player that the enemy spawns 

    // Start is called before the first frame update
    void Start()
    {
        INIT(); //TODO: call init somewhere else if warrented. 
    }

    private void INIT()
    {
        pool = new List<GruntAI>();

        for (int i = 0; i < size; i++)
        {

            //TODO:will have to create a general spawn method in the future so as not to doop code HERE&&HERE1
            Vector3 spawnVector = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z+spawnDistance);
            Quaternion ranRot = Quaternion.Euler(0, Random.Range(0, 359), 0);

            spawnVector = ranRot * spawnVector;

           
            GruntAI newEnemy = Instantiate(objectToPool, spawnVector, Quaternion.identity);
            newEnemy.loadout(player);
            newEnemy.Init();
            newEnemy.Despawn += op_ProcessCompleted; //this line adds the despawn event to this entity 
            newEnemy.gameObject.SetActive(true);

            pool.Add(newEnemy);
        }
    }

    // THis is the method that sets the entity to Deactive and bascially is uesd to kill the entitiy 
    public void op_ProcessCompleted(SelfDespawn entity)
    {
        entity.gameObject.SetActive(false);
        //TODO: Add Logic here to make sure Entity either remains in the pool or becomes a new entity
    }

    /// <summary>
    /// Respawns Enemy when they are detected as dead by the update functio
    /// </summary>
    /// <param name="deddude"> is the Enemy Ai that died </param>
    public void Respawn(GruntAI deddude)
    {

        //TODO: Doop code HERE&&HERE1 
        Vector3 spawnVector = new Vector3(0, player.transform.position.y, spawnDistance);
        Quaternion ranRot = Quaternion.Euler(0, Random.Range(0, 359), 0);
        spawnVector = ranRot * spawnVector;

        spawnVector += player.transform.position;

        deddude.gameObject.transform.position = spawnVector;
        deddude.Init();
        deddude.gameObject.SetActive(true);


    }

    // Update is called once per frame
    void Update()
    {

        foreach (GruntAI g in pool)
        {

            if(g.gameObject.activeSelf)
            {
                g.seperate(pool); //
            } else
            {
                if (g.isAlive() == false) // Checks if Enemy Got shot or if they just got despawned 
                {
                    scoreKeeper.AddToScore(((int)g.getScore()));                   
                }
                Respawn(g);
            }

        }
    }
}
