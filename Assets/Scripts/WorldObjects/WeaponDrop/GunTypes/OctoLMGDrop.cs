using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to be attached to the OctoLMGDrop model
/// </summary>
public class OctoLMGDrop : WeaponDropGunType
{
    /// <summary>
    /// Returns the guntype PlayerGunType.OctoLMG
    /// </summary>
    /// <returns>PlayerGunType.OctoLMG</returns>
    public override PlayerGunType GetPlayerGunType()
    {
        return PlayerGunType.OctoLMG;
    }
}
