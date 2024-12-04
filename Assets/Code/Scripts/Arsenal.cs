using EditorObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Gun;

/// <summary>
/// The Arsenal keeps track of guns that the player bike can equip, as well as handling the equip/dequip code
/// </summary>
public class Arsenal : MonoBehaviour, IResettable
{
    /// <summary>
    /// Saved data for the Arsenal object
    /// NOTE: Set this value in the prefab to set the default savedData for test scenes
    /// </summary>
    [SerializeField] private EditorObject.Arsenal savedData;

    /// <summary>
    /// List of instantiated guns -- created by this class
    /// </summary>
    private Gun.Gun[] gunList = null;

    /// <summary>
    /// Current gun the arsenal is shooting
    /// </summary>
    [SerializeField] private Gun.Gun selected;

    /// <summary>
    /// Prefab used to generate the gunList
    /// </summary>
    [SerializeField] private Gun.Gun gunPrefab = null;

    #region Equipped Guns
    /// <summary>
    /// Represents visible guns player has. Functions as a lookup table that routes to the gun index in gunList.
    /// </summary>
    private int[] equippedGunSlots = null;
    /// <summary>
    /// Index of equippedGuns array of the gun the player is currently holding
    /// </summary>
    private int currentEquippedSlot = -1;
    #endregion

    /// <summary>
    /// Returns a reference to the Gun component of the player's currently held gun or null
    /// </summary>
    private Gun.Gun CurrentGun {
        get {

            if (currentEquippedSlot != -1 && equippedGunSlots[currentEquippedSlot] != -1)
            { 
                return gunList[equippedGunSlots[currentEquippedSlot]];
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
        GatherPlayerInput();
    }


    /// <summary>
    /// Initialize this Arsenal
    /// </summary>
    /// <param name="gameSave">Save which contains our current arsenal</param>
    public void Init(GameSave gameSave) 
    {
        this.savedData = gameSave.arsenal;

        GameStateController.levelComplete.notifyListenersEnter += LevelComplete;

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

    /// <summary>
    /// Generate all guns the player can equip
    /// </summary>
    private void InstantiateAllGuns() 
    {
        gunList = new Gun.Gun[savedData.AllUnlockableGuns.Length];
        for (int i = 0; i < savedData.AllUnlockableGuns.Length; i++)
        {
            // Create new instance of prefab
            Gun.Gun gun = Instantiate<Gun.Gun>(gunPrefab, gameObject.transform);

            gun.Init(savedData.AllUnlockableGuns[i].stats);
            gun.UpdateBarrelColor(savedData.AllUnlockableGuns[i].barrelColor);
            gun.onOutOfAmmo += RemoveEquippedGun;


            gunList[i] = gun;
        }
    }

    /// <summary>
    /// Sets the gun state to the state outlined in SaveData
    /// </summary>
    private void SetStateToSaveData() 
    {
        // A value of -1 is a "null" slot
        equippedGunSlots = new int[savedData.NumberOfGunSlots];

        for (int i = 0; i < savedData.NumberOfGunSlots; i++) 
        {
            equippedGunSlots[i] = savedData.EquippedGuns[i].listIdx;
        }

        currentEquippedSlot = savedData.LastEquippedSlot;
        HideAllGuns();
        EquipGunInSlot();
        SetGunAmmoToSaveData();

    }

    #region Input

    /// <summary>
    /// Get player input this frame
    /// </summary>
    private void GatherPlayerInput() 
    {
        if (selected != null)
        {
            selected.ExternalFire = CheckCanShootGun();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            EquipGunInSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            EquipGunInSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            EquipGunInSlot(2);
        }
    }

    /// <summary>
    /// Gathers input and returns if the arsenal can shoot
    /// </summary>
    /// <returns>True if the player can fire this frame</returns>
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

    #endregion

    /// <summary>
    /// Equips gun at the desired slot
    /// </summary>
    /// <param name="slot">Index of the equippedGunSlots to set the quipped gun</param>
    private void EquipGunInSlot(int slot) 
    {
        if (CurrentGun != null) 
        {
            CurrentGun.gameObject.SetActive(false);
        }
        currentEquippedSlot = slot;
        EquipGunInSlot();
    }

    /// <summary>
    /// Enables gun in the currentEquippedSlot
    /// </summary>
    private void EquipGunInSlot()
    {
        if (CurrentGun != null)
        {
            CurrentGun.gameObject.SetActive(true);
            selected = CurrentGun;
        }
    }

    /// <summary>
    /// Triggers logic that should happen on level complete
    /// </summary>
    public void LevelComplete() 
    {
        savedData.UpdateSaveData(gunList, equippedGunSlots, currentEquippedSlot);
    }

    /// <summary>
    /// Disable all gameObjects in the gunList
    /// </summary>
    private void HideAllGuns()
    {
        for (int i = 0; i < gunList.Length; i++)
        {
            gunList[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Sets each equipped gun's ammo to the amount specified in SaveData
    /// </summary>
    private void SetGunAmmoToSaveData() 
    {
        Assert.IsTrue(equippedGunSlots.Length == savedData.EquippedGuns.Length, "Number of gun slots in save data and in arsenal are different");

        for (int i = 0; i < equippedGunSlots.Length; i++) 
        {
            int gunIdx = equippedGunSlots[i];
            int ammoCount = savedData.EquippedGuns[i].ammoCount;

            if (gunIdx != -1 && ammoCount != -1) 
            {
                gunList[gunIdx].SetAmmo(ammoCount);
            }
        }
    }

    /// <summary>
    /// Removes the equipped gun
    /// </summary>
    /// <param name="gun">Gun reference to be removed/disabled</param>
    private void RemoveEquippedGun(Gun.Gun gun) 
    {
        for(int i = 0; i < equippedGunSlots.Length; i++) 
        {
            int equippedGunIdx = equippedGunSlots[i];
            
            if (gunList[equippedGunIdx] == gun) 
            {
                // Remove the gun
                equippedGunSlots[i] = -1;
                gunList[equippedGunIdx].gameObject.SetActive(false);
                break;
            }
        }
    }
}
