using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// enum for determining gun type quickly
// Ensure that DefaultGun is always the first and INVALID is always last.
public enum PlayerWeaponType
{
    DefaultGun,
    OctoLMG,
    VulkanV64AutoCannons,
    Shotty,
    INVALID
}

/// <summary>
/// This class is the base abstract class that will be used for all weapons and guns. 
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    /// <summary>
    /// The Awake Function, initializes the Weapon 
    /// </summary>
    protected virtual void Awake()
    {
        Init();
    }
    /// <summary>
    /// Used to deInit the weapon 
    /// </summary>
    protected virtual void OnDestroy()
    {
        DeInit();
    }
    /// <summary>Initializes veriables. Specifically must initialize lastFired and fireRate variables.</summary>
    public abstract void Init();

    /// <summary>Basically a destructor. Calls bulletPool.DeInit().</summary>
    public virtual void DeInit()
    {
       //TODO: add more featuers? Thoughts
    }

    /// <summary>Fires the bullet from the muzzle of the gun. Is responsible for calling OnBulletShot and getting 
    /// bullet from the object pool.</summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public abstract void PrimaryFire(Vector3 initialVelocity);

    /// <summary>
    /// Does necessary logic for the primary fire being released.
    /// </summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public abstract void ReleasePrimaryFire(Vector3 initialVelocity);

    /// <summary>Fires the bullet from the muzzle of the gun. Is responsible for calling OnBulletShot and getting 
    /// bullet from the object pool.</summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public abstract void SecondaryFire(Vector3 initialVelocity);

    /// <summary>
    /// Does necessary logic for the secondary fire being released.
    /// </summary>
    /// <param name="initialVelocity">The velocity of the gun when the bullet is shot.</param>
    public abstract void ReleaseSecondaryFire(Vector3 initialVelocity);

    //Must be implemented, if Weapon is not designed to be equipped by the player, use the INVALID value
    public abstract PlayerWeaponType GetPlayerWeaponType();

    //TODO: Add Code for Weapons and items being able to be picked up here! 
}
