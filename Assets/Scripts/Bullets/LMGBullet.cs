using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LMGBullet : Bullet
{
    public override void Init() 
    {
        muzzleVelocity = 180;
        mass = .5f;
    }
}
