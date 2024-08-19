using Assets.Scripts.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BasicRifleBullet</c> Basic enemy bullet.</summary>
public class TurretBullet : PlayerBullet
{
    public override void Init()
    {
        muzzleVelocity = 60;
        mass = 2f;
        damageDealt = 60;
    }
}