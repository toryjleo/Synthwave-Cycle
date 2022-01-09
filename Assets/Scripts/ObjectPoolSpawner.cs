using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolSpawner : MonoBehaviour
{

    public EnemyAI objectToPool;
    public Gun gunToPool;
    public GameObject player; 
    private List<EnemyAI> pool;
    public float size;
    

    // Start is called before the first frame update
    void Start()
    {
        INIT(); //TODO: call init somewhere else if warrented. 
    }

    private void INIT()
    {
        pool = new List<EnemyAI>();
        for (int i = 0; i < size; i++)
        {
            Vector3 ranPos = new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
            EnemyAI newEnemy = Instantiate(objectToPool, ranPos, Quaternion.identity);
            newEnemy.setUpEnemy(player);
            pool.Add(newEnemy);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (EnemyAI g in pool)
        {
            Vector3 var = g.getPosition();

            //hey you G! Make sure to move in accordance with the other entities, Here's the list get it done! 
            
            g.seperate(pool); 

          
        }
    }
}
