using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShottyDrop : WeaponDropGunType
{
    public override PlayerGunType GetPlayerGunType()
    {
        return PlayerGunType.Shotty;
    }
}
