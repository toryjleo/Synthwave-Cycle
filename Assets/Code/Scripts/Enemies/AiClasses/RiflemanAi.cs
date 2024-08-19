using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiflemanAi : InfantryAI
{
    public GameObject muzzleLocation; 


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
        return Enemy.Rifleman;
    }
}
