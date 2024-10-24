using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerAi : InfantryAI
{
    public GameObject muzzleLocation; // Empty GameObject set to the location of the barrel

    void Awake()
    {

    }

    public override Enemy GetEnemyType()
    {
        return Enemy.Ranger;
    }
}
