using Assets.Scripts.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BasicRifleBullet</c> Basic enemy bullet.</summary>
public class BasicRifleBullet : EnemyBullet
{
    public override void Init()
    {
        muzzleVelocity = 60;
        mass = .5f;
        damageDealt = 70;
    }
}
