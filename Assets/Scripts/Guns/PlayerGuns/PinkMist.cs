using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerHealth;

public class PinkMist : Weapon
{
    public GameObject CollisionVolume;

    public override PlayerWeaponType GetPlayerWeaponType()
    {
        return PlayerWeaponType.PinkMist;
    }

    public override void Init()
    {
    }

    public override void PrimaryFire(Vector3 initialVelocity)
    {
        Debug.Log("Pink Mist triggered");
    }

    public override void ReleasePrimaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }

    public override void ReleaseSecondaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }

    public override void SecondaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }

    public void HandleBarUpdate(BarMax oldMax, BarMax newMax) 
    {
        if (newMax >= oldMax) 
        {
            PrimaryFire(Vector3.zero);
        }
    }
}
