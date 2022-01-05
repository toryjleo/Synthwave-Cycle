using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolSpawner : MonoBehaviour
{

    public EnemyAI objectToPool;

    private List<EnemyAI> pool;
    float size; 
    

    // Start is called before the first frame update
    void Start()
    {
        pool = new List<EnemyAI>();
        EnemyAI temp; 

        for (int i = 0; i < size; i++)
        {
            temp = objectToPool;
            
            pool.Add(temp);
        }
    } 


    // Update is called once per frame
    void Update()
    {
        foreach(EnemyAI g in pool)
        {
          if (g.isAlive())
            {

            }

        }
    }
}
