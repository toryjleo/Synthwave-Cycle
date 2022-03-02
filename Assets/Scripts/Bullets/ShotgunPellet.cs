using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunPellet : Bullet
{
    public override void Init()
    {
        muzzleVelocity = 60;
        mass = .1f;
        damageDealt = 10;
    }
}
