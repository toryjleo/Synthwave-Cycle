using EditorObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data structure to store all data used for randomly spawning a gun
/// </summary>
public class ProbabilityGun 
{
    private GunStats stats;
    private Color dropColor;
    private float chanceToDrop = 0;
    private int dropCount = 0;


    private int DROP_MAX = 3;

    public GunStats Stats 
    {
        get { return stats; }
    }

    public Color DropColor 
    {
        get { return dropColor; }
    }

    public float ChanceToDrop 
    { 
        get => chanceToDrop; 
    }

    public int DropCount 
    {
        get { return dropCount; }
    }

    public bool IsOverLimit 
    {
        get => dropCount >= DROP_MAX;
    }

    public ProbabilityGun(DefinedGun definedGun) 
    {
        stats = definedGun.stats;
        dropColor = definedGun.barrelColor;
        chanceToDrop = definedGun.ChanceToDrop;
        dropCount = 0;
    }

    public void IncreaseDropCount() 
    {
        dropCount++;
    }

    public void ResetDropCount() 
    {
        dropCount = 0;
    }
}


/// <summary>
/// Returns random drops to use.
/// Will remove possible gun drops when they drop too often
/// Will not work without an arsenal
/// </summary>
public class ProbablilityMachine
{
    /// <summary>
    /// Handles ever-changing chances for gun drops
    /// </summary>
    private static class DropChance 
    {
        private static float totalChance = 1;

        public static float RandomValue() 
        {
            return Random.Range(0, totalChance);
        }

        public static void IncreaseBy(float amount) 
        {
            totalChance += amount;
        }
        public static void DecreaseBy(float amount) 
        {
            totalChance -= amount;
        }

        public static void ResetChance() 
        {
            totalChance = 1;
        }
    }

    /// <summary>
    /// Guns that will currently spawn
    /// </summary>
    private List<ProbabilityGun> probabilities;
    /// <summary>
    /// Guns that will not currently spawn
    /// </summary>
    private List<ProbabilityGun> exclusions;

    /// <summary>
    /// Maximum times a gun can spawn before it is no longer an option to spawn
    /// </summary>
    private int MAX_EXCLUSIONS = 3;

    private bool IsOverMaxExclusionCount
    {
        get => (exclusions.Count > MAX_EXCLUSIONS);
    }

    private bool HasOneProbabilityOrLess
    {
        get => probabilities.Count <= 1;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="arsenal">Object to grab gun probabilities from</param>
    public ProbablilityMachine(EditorObject.Arsenal arsenal)
    {
        DropChance.ResetChance();
        DefinedGun[] allUnlockableGuns = arsenal.AllUnlockableGuns;
        probabilities = new List<ProbabilityGun>();
        exclusions = new List<ProbabilityGun>();

        for (int i = 0; i < allUnlockableGuns.Length; i++) 
        {
            probabilities.Add(new ProbabilityGun(allUnlockableGuns[i]));
        }
    }

    /// <summary>
    /// Retrieve a random gun from list of probabilies
    /// </summary>
    /// <returns>A gun type</returns>
    public ProbabilityGun RandomGun()
    {
        float randPercent = DropChance.RandomValue();
        float val = 0;
        ProbabilityGun toReturn = null;

        for (int i = 0; i < probabilities.Count; i++) 
        {
            val += probabilities[i].ChanceToDrop;
            if (val >= randPercent) 
            {
                toReturn = probabilities[i];
                break;
            }
        }

        
        toReturn.IncreaseDropCount();

        if (toReturn.IsOverLimit)
        {
            CreateNewExclusion(toReturn);
        }

        return toReturn;
    }

    /// <summary>
    /// Removes another gun from spawn possibilities
    /// </summary>
    /// <param name="newExclusion">New gun to exclude from spawn</param>
    private void CreateNewExclusion(ProbabilityGun newExclusion) 
    {
        // TODO: make sure exclusions reset when we have 
        if (IsOverMaxExclusionCount || HasOneProbabilityOrLess)
        {
            ResetExclusions();
        }

        newExclusion.ResetDropCount();

        // Create a new exclusion
        probabilities.Remove(newExclusion);
        DropChance.DecreaseBy(newExclusion.ChanceToDrop);
        exclusions.Add(newExclusion);
    }

    /// <summary>
    /// Clears all held exclusions
    /// </summary>
    private void ResetExclusions() 
    {
        // Add exclusions back
        for (int i = 0; i < exclusions.Count; i++)
        {
            DropChance.IncreaseBy(exclusions[i].ChanceToDrop);
        }

        probabilities.AddRange(exclusions);
        exclusions = new List<ProbabilityGun>();
    }

    public void Reset() 
    {
        // TODO: Reset code
    }
}
