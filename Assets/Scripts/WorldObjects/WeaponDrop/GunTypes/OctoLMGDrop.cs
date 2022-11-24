using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoLMGDrop : WeaponDropGunType
{
    public override PlayerGunType GetPlayerGunType()
    {
        return PlayerGunType.OctoLMG;
    }
}
