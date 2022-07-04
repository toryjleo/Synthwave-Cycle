using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Waves : MonoBehaviour
{


    /// <summary>
    /// This Wave class stores data for the types of enemyies, the number of each type, and how fast they come in
    /// </summary>
    [System.Serializable]
    public class Wave
    {
        public int WaveNumber;//this will be used to rank the difficulty of Units 
        public List<Unit> Units; //the given units used within a game 
        public float SpawningDelay; //will be used to determine the amount of time between each entity getting spawned

        public Wave(int waveNum, List<Unit> units, float SpawnDelay)
        {
            WaveNumber = waveNum;
            Units = units;
            SpawningDelay = SpawnDelay;
        }
        public static Wave Instance;

        private void Awake()
        {
            Instance = this;
        }
 
    }

    /// <summary>
    /// This Class is used for storing different units of enemies by name. 
    /// </summary>
    [System.Serializable]
    public class Unit
    {
        public string UnitName;
        public Enemy UnitType;
        public int Quantity;

        public Unit(string uN, Enemy uT, int N)
        {
            UnitName = uN;
            UnitType = uT;
            Quantity = N;
        }
    }


    public List<Wave> waves;
    public List<Unit> units;
    public Dictionary<string, Queue<GameObject>> unitDictionary; //These are the keys 
    public Dictionary<int, Queue<GameObject>> waveDictionary; //These are the keys 

    // Start is called before the first frame update
    void Start()
    {
        waveDictionary = new Dictionary<int, Queue<GameObject>>();
        unitDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Wave wave in waves)
        {

        }

        foreach(Unit unit in units)
        {

        }
    }

    //public Wave GetWave(int number) => waves.IndexOf(0);

}
