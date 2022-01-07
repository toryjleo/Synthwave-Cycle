using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolSpawner : MonoBehaviour
{

    public EnemyAI objectToPool;
    public BikeScript player; 
    private List<EnemyAI> pool;
    public float size;  
    

    // Start is called before the first frame update
    void Start()
    {
        pool = new List<EnemyAI>();
        

        for (int i = 0; i < size; i++)
        {
            pool.Add(objectToPool);

        }

        foreach (EnemyAI g in pool) //spawn the initial wave 
        {

            Vector3 ranPos = new Vector3(Random.RandomRange(-20, 20), 0, Random.RandomRange(-20, 20));
            Instantiate(g, ranPos, Quaternion.identity);



        }

    } 


    // Update is called once per frame
    void Update()
    {
        foreach (EnemyAI g in pool)
        {
            if (g.isAlive())
            {
                
            } 
            else
            {
                Vector3 ranPos = new Vector3(Random.RandomRange(-20, 20), 0, Random.RandomRange(-20, 20));
                Instantiate(g, ranPos, Quaternion.identity);
            }
        }
    }
}
