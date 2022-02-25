using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMaster : MonoBehaviour
{
    // Start is called before the first frame update
    public Wave ops;
    public GameObject player;
    public List<AiTemplate> currentEnemies;
    private float spawnDistance = 40;
    private int firstWave = 5;
    private int dangerLevel = 0;

    void Start()
    {
        ops = Wave.Instance;
    }

    public Vector3 generateSpawnVector()
    {
        //TODO:will have to create a general spawn method in the future so as not to doop code HERE&&HERE1
        Vector3 spawnVector = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + spawnDistance);
        Quaternion ranRot = Quaternion.Euler(0, Random.Range(0, 359), 0);

        spawnVector = ranRot * spawnVector;
        return spawnVector;
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
        GameObject enemy = ops.SpawnFromPool("RifleMan", generateSpawnVector(), Quaternion.identity, player);
        currentEnemies.Add(enemy.GetComponentInChildren<AiTemplate>());
    }
}
