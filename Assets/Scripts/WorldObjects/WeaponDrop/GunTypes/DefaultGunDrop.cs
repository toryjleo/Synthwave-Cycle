using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to be attached to the DefaultGunDrop model
/// </summary>
public class DefaultGunDrop : WeaponDropGunType
{
    /// <summary>
    /// Returns the guntype PlayerGunType.DefaultGun
    /// </summary>
    /// <returns>PlayerGunType.DefaultGun</returns>
    public override PlayerGunType GetPlayerGunType()
    {
        return PlayerGunType.DefaultGun;
    }
}
