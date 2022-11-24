using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponDropGunType : MonoBehaviour
{
    //Must be implemented, if Gun is not designed to be equipped by the player, use the INVALID value
    public abstract PlayerGunType GetPlayerGunType();
}
