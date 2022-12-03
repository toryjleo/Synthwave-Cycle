using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A LevelledGun is a player weapon that gets stronger every time it is equiped (Init)
/// </summary>
public abstract class LeveledGun : Gun
{
    //Denotes the current level of each gun. Starting at 1 and getting higher every DeInit
    private static Dictionary<PlayerGunType, int> gunLevels = new Dictionary<PlayerGunType, int>();

    //Returns the current level of the LeveledGun, if the gun is not present in the dictionary, add it at level 1
    protected int GetCurrentLevel()
    {
        PlayerGunType gunType = GetPlayerGunType();
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
        gunLevels[GetPlayerGunType()]++;
        base.DeInit();
    }

    //increases the gun's level by one
    public void LevelUp()
    {
        PlayerGunType gunType = GetPlayerGunType();
        if (!gunLevels.ContainsKey(gunType))
        {
            gunLevels.Add(gunType, 1);
        }
        gunLevels[gunType]++;
    }

    //overriden, this methos is called when the last shot is fired, or the gun is discarded
    public abstract void BigBoom();
}
