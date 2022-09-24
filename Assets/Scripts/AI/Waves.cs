using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#region Units
/// <summary>
/// A unit is a collection of enemies that the wave spawner can use to spawn enemies in groups. 
/// </summary>
[System.Serializable]
public class Unit
{
    public string UnitName; //The Name of the unit, like a squad name 
    public Enemy UnitType; //The type of Enemy contained in the Unit 
    public int Quantity; //The size of the unit 
     
    public Unit(string uN, Enemy uT, int N) //Constructor 
    {
        UnitName = uN;
        UnitType = uT;
        Quantity = N;
    }
}
#endregion
#region Waves

/// <summary>
/// This Wave class has a number to note it's difficulty/order, 
/// as well as a list of units contained in the wave. 
/// The Spawn delay is used to determine the time between spawning each unit. 
/// </summary>
[System.Serializable]
public class Wave
{
    public int WaveNumber;//this will be used to rank the difficulty of Units 
    public List<Unit> Units; //the given units used within a game 
    public float SpawningDelay; //will be used to determine the amount of time between each entity getting spawned

    public Wave(int waveNum, List<Unit> units, float SpawnDelay) //Constructor 
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

    public List<Wave> waves; //The list of the waves.
    public Dictionary<int, Wave> waveDictionary; //These are the keys, that corrispond to each waves number 
    public List<Unit> units; //these are the units contained in each wave. 
    public Dictionary<string, List<Unit>> unitDictionary; //These are the keys for the units 

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

    /// <summary>
    /// Public wave retreiver 
    /// </summary>
    /// <param name="number">Number will be calculated by another program and will corrisopond to a specific wave </param>
    /// <returns> returns the wave of the number</returns> 
    /// TODO: Add error checking if wave does not exist, return the largest available wave???
    public Wave GetWave(int number) => waveDictionary[number];//

}
