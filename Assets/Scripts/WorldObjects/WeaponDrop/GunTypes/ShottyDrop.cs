using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to be attached to the ShottyDrop model
/// </summary>
public class ShottyDrop : WeaponDropGunType
{
    /// <summary>
    /// Returns the guntype PlayerGunType.Shotty
    /// </summary>
    /// <returns>PlayerGunType.Shotty</returns>
    public override PlayerGunType GetPlayerGunType()
    {
        return PlayerGunType.Shotty;
    }
}
