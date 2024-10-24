using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiflemanAi : InfantryAI
{
    public override Enemy GetEnemyType()
    {
        return Enemy.Rifleman;
    }
}
