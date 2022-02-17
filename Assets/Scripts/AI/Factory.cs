using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{

    public GameObject[] aiTemplates;
    public GameObject[] guns;

    public static ObjectPoolSpawner spawner;


    // Start is called before the first frame update
    void Start()
    {
        aiTemplates = Resources.LoadAll<GameObject>("EnemyPrefabs");
        guns = Resources.LoadAll<GameObject>("Guns");
    }

    // Update is called once per frame
    public GameObject createEnemy(int guntype, int enemyType)
    {
        switch (enemyType)
        {
            case 0:



            case 1:





        }



        return null;
    }
}
