using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    /// <summary>
    /// Defines a gun that will spawn in game
    /// </summary>
    [Serializable]
    public class DefinedGun 
    {
        /// <summary>
        /// Statistics of gun
        /// </summary>
        [SerializeField] public GunStats stats;
        /// <summary>
        /// Color of gun barrel
        /// </summary>
        [SerializeField] public Color barrelColor = Color.white;
    }

    /// <summary>
    /// Data for an equipped gun
    /// </summary>
    [Serializable]
    public class GunEquipSlot 
    {
        public GunEquipSlot(int idx, int count) 
        {
            listIdx = idx;
            ammoCount = count;
        }
        /// <summary>
        /// Index of gun in allUnlockableGuns
        /// </summary>
        [SerializeField] public int listIdx;
        /// <summary>
        /// Amount of ammo the gun currently has
        /// </summary>
        [SerializeField] public int ammoCount;
    }

    /// <summary>
    /// Class that stores the information of the current state of the player's weapon arsenal.
    /// Also stores all guns the player can unlock.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Arsenal", fileName = "New Arsenal")]
    public class Arsenal : ScriptableObject
    {
        /// <summary>
        /// Number of gun slots available to player
        /// </summary>
        private const int NUMBER_OF_GUN_SLOTS = 3;
        /// <summary>
        /// Each gun that exists for the player in the game
        /// </summary>
        [SerializeField] private DefinedGun[] allUnlockableGuns;
        /// <summary>
        /// All guns currently equipped to the player
        /// </summary>
        [SerializeField] private GunEquipSlot[] equippedGuns = new GunEquipSlot[NUMBER_OF_GUN_SLOTS];
        /// <summary>
        /// Current gun that is held and shootable by the player
        /// </summary>
        [SerializeField] private int lastEquippedSlotIdx = -1;


        public int NumberOfGunSlots { get { return NUMBER_OF_GUN_SLOTS; } }

        public DefinedGun[] AllUnlockableGuns { get => allUnlockableGuns; }

        public GunEquipSlot[] EquippedGuns { get => equippedGuns; }

        public int LastEquippedSlot { get => lastEquippedSlotIdx; }

        #region Methods
        /// <summary>
        /// Updates the data held by this object
        /// </summary>
        /// <param name="gunList">List of guns on player</param>
        /// <param name="equippedGuns">Indices of gunList. Current guns populating player's equip slots</param>
        /// <param name="currentEquippedSlot">Index of equippedGuns. Current gun being held</param>
        public void UpdateSaveData(Gun.Gun[] gunList, int[] equippedGuns, int currentEquippedSlot)
        {
            // Update Equipped Guns
            this.equippedGuns = new GunEquipSlot[NUMBER_OF_GUN_SLOTS];
            for (int i = 0; i < equippedGuns.Length; i++)
            {
                int gunListIdx = equippedGuns[i];
                int ammoCount = (gunListIdx == -1) ? -1 : gunList[gunListIdx].AmmoCount;
                this.equippedGuns[i] = new GunEquipSlot(gunListIdx, ammoCount);
            }

            // Ensure lastEquippedSlotIdx is a slot that contains a usable gun
            if (currentEquippedSlot == -1 || this.equippedGuns[currentEquippedSlot].listIdx == -1)
            {
                for (int i = 0; i < NUMBER_OF_GUN_SLOTS; i++)
                {
                    if (this.equippedGuns[i].listIdx != -1)
                    {
                        currentEquippedSlot = i;
                        break;
                    }
                }
            }

            // Update equipped slot
            lastEquippedSlotIdx = currentEquippedSlot;
        }

        /// <summary>
        /// Reset this object to its initial state, the first time the game is launched.
        /// </summary>
        public void ResetToDefaults() 
        {
            equippedGuns = new GunEquipSlot[NUMBER_OF_GUN_SLOTS];
            for (int i = 0; i < NUMBER_OF_GUN_SLOTS; i++) 
            {
                equippedGuns[i] = new GunEquipSlot(-1, -1);
            }

            lastEquippedSlotIdx = -1;
        }

        #endregion
    }
}