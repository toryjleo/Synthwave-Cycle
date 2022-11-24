using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : SelfWorldBoundsDespawn
{
    private PlayerGunType gunType;

    public PlayerGunType GunType 
    {
        get 
        {
            return gunType;
        }

        set 
        {
            if (value == PlayerGunType.INVALID) 
            {
                Debug.LogWarning("Tried to initialize WeaponDrop to use an invalid weapon type");
            }
            else 
            {
                gunType = value;
            }
        }
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        // TODO: remove this and replace with a object pool 
        Despawn += op_SelfDelete;
    }


    private PlayerGunType AssignRandomGunDrop() 
    {
        return PlayerGunType.INVALID;
    }


    /// <summary>
    /// Weapon drops clean themselves up
    /// </summary>
    /// <param name="entity">This GameObject</param>
    public void op_SelfDelete(SelfDespawn entity)
    {
        // TODO: Instead of this object destroying itself, it should be sent back to an object pool.
        Destroy(entity.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player hit pickup!");
            Arsenal arsenal = other.gameObject.GetComponentInChildren<Arsenal>();
            if (arsenal == null)
            {
                Debug.LogError("Cannot find Bike Arsenal Component");
            }
            else
            {
                arsenal.EquipGun(PlayerGunType.OctoLMG);
                OnDespawn();
            }
        }
    }
}
