using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerAI : InfantryAI
{
    public GameObject muzzleLocation; // Empty GameObject set to the location of the barrel

    void Awake()
    {
        Init();
    }

    public override void Update()
    {
        base.Update();
    }

    public override Enemy GetEnemyType()
    {
        return Enemy.Ranger;
    }
}