using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#region Units
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
#endregion
#region Waves

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
#endregion

/// <summary>
/// This class stores customizable waves and units of enemies to be spawned by the Enemy Spawner.
/// </summary>
public class Waves : MonoBehaviour
{

    public List<Wave> waves;
    public Dictionary<int, Wave> waveDictionary; //These are the keys 
    public List<Unit> units;
    public Dictionary<string, List<Unit>> unitDictionary; //These are the keys 

    #region Singleton

    public static Waves Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        waveDictionary = new Dictionary<int, Wave>();
        unitDictionary = new Dictionary<string, List<Unit>>();

        foreach (Wave wave in waves)
        {
            waveDictionary.Add(wave.WaveNumber, wave);
        }

    }

    public Wave GetWave(int number) => waveDictionary[number];

}
