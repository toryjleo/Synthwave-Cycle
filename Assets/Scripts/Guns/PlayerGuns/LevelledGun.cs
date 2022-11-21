using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A LevelledGun is a player weapon that gets stronger every time it is equiped (Init)
/// </summary>
public abstract class LeveledGun : Gun
{
    //Denotes the current level of each gun. Starting at 1 and getting higher every DeInit
    private static Dictionary<System.Type, int> gunLevels = new Dictionary<System.Type, int>();

    //Returns the current level of the LeveledGun, if the gun is not present in the dictionary, add it at level 1
    protected int GetCurrentLevel()
    {
        System.Type gunType = this.GetType();
        if (!gunLevels.ContainsKey(gunType))
        {
            gunLevels.Add(gunType, 1);
        }
        //Debug.Log("GetCurrentLevel for " + gunType.ToString() + " = " + gunLevels[gunType]);
        return gunLevels[gunType];
    }

    //De-Initialize the gun and level it up by one so it will be stronger next time
    public override void DeInit()
    {
        gunLevels[this.GetType()]++;
        //Debug.Log("DeInit called for LeveledGun of type: " + this.GetType().ToString() + "\nLevel is now: " + gunLevels[this.GetType()]);
        base.DeInit();
    }
}
