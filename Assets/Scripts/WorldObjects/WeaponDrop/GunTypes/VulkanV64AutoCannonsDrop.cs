using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to be attached to the OctoLMGDrop model
/// </summary>
public class VulkanV64AutoCannonsDrop : WeaponDropGunType
{
    /// <summary>
    /// Returns the guntype PlayerGunType.VulkanV64AutoCannons
    /// </summary>
    /// <returns>PlayerGunType.VulkanV64AutoCannons</returns>
    public override PlayerGunType GetPlayerGunType()
    {
        return PlayerGunType.VulkanV64AutoCannons;
    }
}
