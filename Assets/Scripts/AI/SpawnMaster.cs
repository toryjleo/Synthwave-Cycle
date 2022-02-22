using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMaster : MonoBehaviour
{
    // Start is called before the first frame update
    public Wave ops;

    public List<AiTemplate> aiList; 
    public GameObject player;
    private float spawnDistance = 40;
    private int firstWave = 4;
    private int dangerLevel = 0;

    void Start()
    {
        ops = Wave.Instance;

        print("Spawn master Awake");

        
    }

    public Vector3 generateSpawnVector()
    {
        //TODO:will have to create a general spawn method in the future so as not to doop code HERE&&HERE1
        Vector3 spawnVector = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + spawnDistance);
        Quaternion ranRot = Quaternion.Euler(0, Random.Range(0, 359), 0);

        spawnVector = ranRot * spawnVector;
        return spawnVector;
    }

    private void Awake()
    {
        print("SpawnMaster Start");

        
    }

    // Update is called once per frame
    void Update()
    {
        if(dangerLevel < firstWave)
        {
            SpawnAWave(firstWave);
            dangerLevel++;
        }



    }

    private void SpawnAWave(int firstWave)
    {

        GameObject enemy = ops.SpawnFromPool("Grunt", generateSpawnVector(), Quaternion.identity, player);
        enemy = ops.SpawnFromPool("Grunt", generateSpawnVector(), Quaternion.identity, player);
    }
}
