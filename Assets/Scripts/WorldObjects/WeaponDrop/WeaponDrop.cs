using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : SelfWorldBoundsDespawn
{
    private Dictionary<PlayerGunType, GameObject> attachedGuns;

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
        // Initialize base value
        this.gunType = 0;

        // TODO: remove this and replace with a object pool 
        Despawn += op_SelfDelete;

        // Populate the "attachedGunTypes" var with references to available guns and their corresponding pickup models
        WeaponDropGunType[] attachedGunTypes = GetAttachedWeaponDropGunTypes();
        PopulateDictionary(attachedGunTypes);

        // Assign random gunType
        SetRandomGunType();
    }

    public void SetGunType(PlayerGunType gunType)
    {
        if (attachedGuns.ContainsKey(gunType)) 
        {
            // Make sure to disable old gunType
            if (attachedGuns.ContainsKey(this.gunType)) 
            {
                attachedGuns[this.gunType].SetActive(false);
            }

            this.gunType = gunType;
            attachedGuns[gunType].SetActive(true);
        }
        else
        {
            Debug.LogError("Tried to assign invalid gunType: " + gunType);
        }
    }

    public PlayerGunType SetRandomGunType()
    {
        PlayerGunType typeToAssign = (PlayerGunType) Random.Range((int)PlayerGunType.DefaultGun, (int)PlayerGunType.INVALID - 1);
        SetGunType(typeToAssign);
        return typeToAssign;
    }

    private WeaponDropGunType[] GetAttachedWeaponDropGunTypes()
    {
        return GetComponentsInChildren<WeaponDropGunType>();
    }

    private void PopulateDictionary(WeaponDropGunType[] attachedGunTypes)
    {
        attachedGuns = new Dictionary<PlayerGunType, GameObject>();
        foreach (WeaponDropGunType gunType in attachedGunTypes)
        {
            attachedGuns[gunType.GetPlayerGunType()] = gunType.gameObject;
            gunType.gameObject.SetActive(false);
        }
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
                arsenal.EquipGun(gunType);
                OnDespawn();
            }
        }
    }
}
