using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum Enemy
{
    Grunt,Rifelman,Blank,Sniper,Dog
}
public class ObjectPool : MonoBehaviour
{

    [System.Serializable]
    public class Pool //One individual pool of objects that has a tag, a prefab, and a size 
    {
        public string tag;
        public GameObject prefab;
        public int size;

        public Pool(string Tag, GameObject Prefab, int Size)
        {
            tag = Tag;
            prefab = Prefab;
            size = Size;
        }
    }

    public static ObjectPool Instance;

    private void Awake()
    {
        Instance = this;
    }


    public List<Pool> pools;
    public Dictionary<string,Queue<GameObject>> poolDictionary;


    //Creates Pools for each object type 
    void Start()
    {   
        //Create Dictionary of tags for each if the pools
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            //instantiate the objects with 
            for(int i =0; i< pool.size; i++)
            {   
                GameObject obj = Instantiate(pool.prefab); 
                obj.SetActive(false);
                obj.GetComponent<Ai>();
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, GameObject target)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag+" doesn't exist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        
        objectToSpawn.GetComponent<Ai>().loadout(target);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
