using Assets.Scripts.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LMGBullet : PlayerBullet
{
    public override void Init()
    {
        muzzleVelocity = 180;
        mass = .1f; //The Mass controlls how slowed down the bike is by recoil
        damageDealt = 20;
    }
}
