using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRifleBullet : Bullet
{
    public override void Init()
    {
        muzzleVelocity = 60;
        mass = .5f;
    }
}
