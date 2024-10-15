using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract class. Must be implemented return a gun type and attached to a model to repesent the in-world drop of
/// the gun type.
/// </summary>
public abstract class WeaponDropGunType : MonoBehaviour
{
    //Must be implemented, if Gun is not designed to be equipped by the player, use the INVALID value
    // TODO: Reimplement
    // public abstract PlayerWeaponType GetPlayerGunType();
}
