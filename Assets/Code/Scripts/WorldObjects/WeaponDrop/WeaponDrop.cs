using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : SelfWorldBoundsDespawn
{

    // TODO: Fix
    /*
    // Tracks the possible guns which this drop can represent
    private Dictionary<PlayerWeaponType, GameObject> attachedGuns;

    // Current PlayerGunType this drop represents
    private PlayerWeaponType gunType;

    public PlayerWeaponType GunType
    {
        get
        {
            return gunType;
        }

        set
        {
            if (value == PlayerWeaponType.INVALID)
            {
                Debug.LogWarning("Tried to initialize WeaponDrop to use an invalid weapon type");
            }
            else
            {
                SetGunType(value);
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

    /// <summary>
    /// Sets this WeaponDrop's associated gunType and model to the new gunType
    /// </summary>
    /// <param name="gunType">The PlayerGunType to be assigned</param>
    private void SetGunType(PlayerWeaponType gunType)
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

    /// <summary>
    /// Assigns a random PlayerGunType to this WeaponDrop
    /// </summary>
    /// <returns>The selected PlayerGunType</returns>
    public PlayerWeaponType SetRandomGunType()
    {
        PlayerWeaponType typeToAssign = (PlayerWeaponType) Random.Range((int)PlayerWeaponType.OctoLMG, (int)PlayerWeaponType.DefaultGun);
        SetGunType(typeToAssign);
        return typeToAssign;
    }

    /// <summary>
    /// Returns a list of WeaponDropGunTypes attached to models undderneath this WeaponDrop object.
    /// </summary>
    /// <returns>List of child object WeaponDropGunType</returns>
    private WeaponDropGunType[] GetAttachedWeaponDropGunTypes()
    {
        return GetComponentsInChildren<WeaponDropGunType>();
    }

    /// <summary>
    /// Takes in a list of WeaponDropGunType to populate the internal attachedGuns dictionary.
    /// </summary>
    /// <param name="attachedGunTypes">List of child WeaponDropGunTypes which are attached to models.</param>
    private void PopulateDictionary(WeaponDropGunType[] attachedGunTypes)
    {
        attachedGuns = new Dictionary<PlayerWeaponType, GameObject>();
        foreach (WeaponDropGunType gunType in attachedGunTypes)
        {
            attachedGuns[gunType.GetPlayerGunType()] = gunType.gameObject;
            gunType.gameObject.SetActive(false);
        }
    }*/


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
        // When hitting a player, get the arsenal and equip the selected gun
        if (other.tag == "Player")
        {
            //Debug.Log("Player hit pickup!");
            Arsenal arsenal = other.gameObject.GetComponentInChildren<Arsenal>();
            if (arsenal == null)
            {
                Debug.LogError("Cannot find Bike Arsenal Component");
            }
            else
            {
                // TODO: Fix
                //Debug.Log("Equipping gun type: " + gunType);
                // arsenal.EquipGun(gunType);
                OnDespawn();
            }
        }
    }
}
