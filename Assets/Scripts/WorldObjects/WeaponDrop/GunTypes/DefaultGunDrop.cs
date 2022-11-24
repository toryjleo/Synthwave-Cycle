using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultGunDrop : WeaponDropGunType
{
    public override PlayerGunType GetPlayerGunType()
    {
        return PlayerGunType.DefaultGun;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
