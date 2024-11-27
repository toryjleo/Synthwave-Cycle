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
    /// <summary>
    /// Saved data for the Arsenal object
    /// NOTE: Set this value in the prefab to set the default savedData for test scenes
    /// </summary>
    // TODO: Make UI component that gives info
    [SerializeField] private EditorObject.Arsenal savedData;

    private Gun.Gun[] gunList = null;

    [SerializeField] private Gun.Gun selected;

    /// <summary>
    /// Represents visible guns player has. Functions as a lookup table that routes to the gun index in gunList.
    /// </summary>
    private int[] equippedGuns = null;
    private int currentEquippedSlot = -1;

    private Gun.Gun CurrentGun {
        get {

            if (equippedGuns[currentEquippedSlot] != -1)
            { 
                return gunList[equippedGuns[currentEquippedSlot]];
            }
            else 
            {
                return null; 
            }
        } 
    }

    private void Start()
    {
        TestSceneInit();
    }


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

    /// <summary>
    /// Initialize object in standalone test scene
    /// </summary>
    private void TestSceneInit()
    {
        if (!GameStateController.StateExists)
        {
            InstantiateAllGuns();
            SetStateToSaveData();
        }
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
            Gun.Gun gun = Instantiate<Gun.Gun>(savedData.AllUnlockableGunPrefabs[i], gameObject.transform);

            gun.Init();
            
            gunList[i] = gun;
        }
    }

    private void SetStateToSaveData() 
    {
        // A value of -1 is a "null" slot
        equippedGuns = new int[savedData.NumberOfGunSlots];

        for (int i = 0; i < savedData.NumberOfGunSlots; i++) 
        {
            equippedGuns[i] = savedData.EquippedGuns[i].listIdx;
        }

        currentEquippedSlot = savedData.LastEquippedSlot;
        HideAllGuns();
        EquipGunInSlot();
        SetGunAmmoToSaveData();

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

    private void HideAllGuns() 
    {
        for (int i = 0;i < gunList.Length; i++) 
        {
            gunList[i].gameObject.SetActive(false);
        }
    }

    private void EquipGunInSlot()
    {
        if (CurrentGun != null) 
        {
            CurrentGun.gameObject.SetActive(true);
            selected = CurrentGun;
        }
    }

    private void SetGunAmmoToSaveData() 
    {
        // TODO: All equipped guns have their ammo updated to the savedData.EquippedGuns[i].ammoCount value
    }
}
