using Assets.Scripts.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BasicRifleBullet</c> Basic enemy bullet.</summary>
public class RangerBullet : EnemyBullet
{
    public override void Init()
    {
        muzzleVelocity = 60;
        mass = .1f;
        damageDealt = 5;
    }
}
