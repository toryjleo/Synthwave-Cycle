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

    #region Properties

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

    /// <summary>
    /// True if the arsenal has an open slot
    /// </summary>
    private bool HasOpenSlot 
    {
        get 
        {
            if (equippedGunSlots == null) 
            {
                return false;
            }
            else 
            {
                for(int i = 0; i < equippedGunSlots.Length; i++) 
                {
                    if (equippedGunSlots[i] == -1) 
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    #endregion

    private void Start()
    {
        TestSceneInit();
    }


    private void Update()
    {
        GatherPlayerInput();
    }

    #region Initialization

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

        currentEquippedSlot = (savedData.LastEquippedSlot == -1) ? 0 : savedData.LastEquippedSlot;
        HideAllGuns();
        SwapToSlot();
        SetGunAmmoToSaveData();

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

    #endregion

    #region Input

    /// <summary>
    /// Get player input this frame
    /// </summary>
    private void GatherPlayerInput() 
    {
        if (CurrentGun != null)
        {
            CurrentGun.ExternalFire = CheckCanShootGun();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SwapToSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            SwapToSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            SwapToSlot(2);
        }
    }

    /// <summary>
    /// Gathers input and returns if the arsenal can shoot
    /// </summary>
    /// <returns>True if the player can fire this frame</returns>
    private bool CheckCanShootGun()
    {
        if (CurrentGun != null && CurrentGun.IsAutomatic && Input.GetButton("Fire1"))
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

    #region Slot Management

    /// <summary>
    /// Equips gun at the desired slot
    /// </summary>
    /// <param name="slot">Index of the equippedGunSlots to set the quipped gun</param>
    private void SwapToSlot(int slot) 
    {
        // Disable the gun that is out
        if (CurrentGun != null) 
        {
            CurrentGun.gameObject.SetActive(false);
        }

        // Enable the new slot
        currentEquippedSlot = slot;
        SwapToSlot();
    }

    /// <summary>
    /// Enables gun in the currentEquippedSlot
    /// </summary>
    private void SwapToSlot()
    {
        if (CurrentGun != null)
        {
            CurrentGun.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Equips a specified gun at a specific gun slot
    /// </summary>
    /// <param name="slotIndex">Gun slot to equip to</param>
    /// <param name="gunListIndex">Index in the gunList of the gun to equip</param>
    private void EquipGunInSlot(int slotIndex, int gunListIndex)
    {
        bool needToSwap = (slotIndex == currentEquippedSlot);
        equippedGunSlots[slotIndex] = gunListIndex;
        gunList[gunListIndex].SetMaxAmmo();

        if (needToSwap)
        {
            SwapToSlot(slotIndex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stats">The gun type of the pickup</param>
    /// <param name="ammoToUse">The amount of ammo the pickup contains</param>
    /// <returns>The number of remaining bullets after using the pickup</returns>
    public int ConsumePickup(GunStats stats, int ammoToUse)
    {
        int idxInGunList = -1;

        // Find if gun is equipped
        for (int i = 0; i < equippedGunSlots.Length; i++)
        {
            if (equippedGunSlots[i] != -1 && gunList[equippedGunSlots[i]].IsThisGun(stats))
            {
                // Found gun and it is equipped
                idxInGunList = equippedGunSlots[i];
                break;
            }
        }

        if (idxInGunList != -1)
        {
            // Gun is equipped and we found its index
            Gun.Gun gunToAddAmmo = gunList[idxInGunList];

            if (gunToAddAmmo.AtMaxAmmo)
            {
                return ammoToUse;
            }
            else
            {
                return gunToAddAmmo.AddAmmo(ammoToUse);
            }
        }
        else
        {
            // Find the index in gunList

            for (idxInGunList = 0; idxInGunList < gunList.Length; idxInGunList++)
            {
                if (gunList[idxInGunList].IsThisGun(stats))
                {
                    break;
                }
            }

            if (idxInGunList >= gunList.Length)
            {
                Debug.LogError("Trying to find a gun not in the list");
                return ammoToUse;
            }
            else
            {
                // Gun is not equipped and we found its index
                if (HasOpenSlot)
                {
                    if (currentEquippedSlot != -1 && equippedGunSlots[currentEquippedSlot] == -1)
                    {
                        // Equip gun in currentEquippedSlot
                        EquipGunInSlot(currentEquippedSlot, idxInGunList);
                    }
                    else
                    {
                        // Equip in first open slot
                        for (int i = 0; i < equippedGunSlots.Length; i++)
                        {
                            if (equippedGunSlots[i] == -1)
                            {
                                EquipGunInSlot(i, idxInGunList);
                                break;
                            }
                        }
                    }
                    return 0;
                }
                else
                {
                    if (currentEquippedSlot == -1)
                    {
                        Debug.LogError("Player is using an unusable gun slot and trying to equip");
                        return ammoToUse;
                    }
                    else if (Input.GetKey(KeyCode.Space))
                    {
                        // Equip gun in currentEquippedSlot
                        CurrentGun?.gameObject.SetActive(false);
                        EquipGunInSlot(currentEquippedSlot, idxInGunList);
                        return 0;
                    }
                    else
                    {
                        return ammoToUse;
                    }
                }

            }
        }
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

    #endregion

    /// <summary>
    /// Triggers logic that should happen on level complete
    /// </summary>
    public void LevelComplete() 
    {
        savedData.UpdateSaveData(gunList, equippedGunSlots, currentEquippedSlot);
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
