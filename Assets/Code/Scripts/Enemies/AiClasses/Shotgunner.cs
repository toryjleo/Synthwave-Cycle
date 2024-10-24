using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgunner : InfantryAI
{
    public GameObject muzzleLocation; // Empty GameObject set to the location of the barrel


    void Awake()
    {

    }

    public override Enemy GetEnemyType()
    {
        return Enemy.Shotgun;
    }

    public override void Initialize()
    {

    }
}
