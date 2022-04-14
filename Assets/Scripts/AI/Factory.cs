using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{

    public GameObject[] guns;
    public GameObject[] enemyPrefabs;



    // Start is called before the first frame update
    void Start()
    {
        guns = Resources.LoadAll<GameObject>("Guns");
        enemyPrefabs = Resources.LoadAll<GameObject>("EnemyPrefabs");
    }

    // Update is called once per frame
    public GameObject createEnemy(int guntype, int enemyType)
    {

        return null;
    }
}
