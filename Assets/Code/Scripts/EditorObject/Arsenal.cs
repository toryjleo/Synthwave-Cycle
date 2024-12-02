using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    [Serializable]
    public class DefinedGun 
    {
        [SerializeField] public GunStats stats;
        [SerializeField] public Color barrelColor = Color.white;
    }

    /// <summary>
    /// Data for an equipped gun
    /// </summary>
    [Serializable]
    public class EquippedGun 
    {
        public EquippedGun(int idx, int count) 
        {
            listIdx = idx;
            ammoCount = count;
        }

        [SerializeField] public int listIdx;
        [SerializeField] public int ammoCount;
    }

    [CreateAssetMenu(menuName = "Game/Arsenal", fileName = "New Arsenal")]
    public class Arsenal : ScriptableObject
    {
        private const int NUMBER_OF_GUN_SLOTS = 3;
        [SerializeField] private DefinedGun[] allUnlockableGuns;
        [SerializeField] private EquippedGun[] equippedGuns = new EquippedGun[NUMBER_OF_GUN_SLOTS];
        [SerializeField] private int lastEquippedSlotIdx = 0;

        public void UpdateSaveData(Gun.Gun[] gunList, int[] equippedGuns, int currentEquippedSlot) 
        {
            // Update Equipped Guns
            this.equippedGuns = new EquippedGun[NUMBER_OF_GUN_SLOTS];
            for (int i = 0; i < equippedGuns.Length; i++) 
            {
                int gunListIdx = equippedGuns[i];
                this.equippedGuns[i] = new EquippedGun(gunListIdx, gunList[gunListIdx].AmmoCount);
            }

            // Update equipped slot
            lastEquippedSlotIdx = currentEquippedSlot;
        }


        public int NumberOfGunSlots
        {
            get { return NUMBER_OF_GUN_SLOTS; }
        }

        public DefinedGun[] AllUnlockableGuns { get => allUnlockableGuns; }

        public EquippedGun[] EquippedGuns { get => equippedGuns; }

        public int LastEquippedSlot { get => lastEquippedSlotIdx; }
    }
}