using EditorObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Arsenal keeps track of guns that the player bike can equip, as well as handling the equip/dequip code
/// </summary>
public class Arsenal : MonoBehaviour, IResettable
{
    EditorObject.Arsenal savedData;

    private Gun.Gun[] gunList = null;

    [SerializeField] private Gun.Gun selected;

    private int[] equippedGuns = null;


    private void Update()
    {
        if (selected != null) 
        {
            selected.ExternalFire = CheckCanShootGun();
        }
    }

    public void Init(GameSave gameSave) 
    {
        this.savedData = gameSave.arsenal;

        InstantiateAllGuns();
        SetStateToSaveData();
    }

    public void ResetGameObject()
    {
        SetStateToSaveData();
    }

    private void InstantiateAllGuns() 
    {
        gunList = new Gun.Gun[savedData.AllUnlockableGunPrefabs.Length];
        for (int i = 0; i < savedData.AllUnlockableGunPrefabs.Length; i++)
        {
            Gun.Gun gun = Instantiate<Gun.Gun>(savedData.AllUnlockableGunPrefabs[i]);
            gun.transform.parent = gameObject.transform;
            gunList[i] = gun;
        }
    }

    private void SetStateToSaveData() 
    {
        equippedGuns = new int[savedData.NumberOfGunSlots];

        // TODO: Set selected gun to selected gun
        // TODO: Set guns to savedData.equippedGuns
        // TODO: Set gun ammo to savedData.equippedGuns
    }

    private bool CheckCanShootGun() 
    {
        if (selected.IsAutomatic && Input.GetButton("Fire1"))
        {
            // Automatic fire case
            return true;
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            // Semi-Automatic fire case
            return true;
        }
        else 
        { 
            return false;
        }
    }

    public void GameComplete() 
    {
        // TODO: Update the savedData
    }
}
