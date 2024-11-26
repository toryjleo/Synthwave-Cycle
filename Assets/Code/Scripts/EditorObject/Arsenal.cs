using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    /// <summary>
    /// Data for an equipped gun
    /// </summary>
    [Serializable]
    class StoredGun 
    {
        int listIdx;
        int ammoCount;
    }

    [CreateAssetMenu(menuName = "Game/Arsenal", fileName = "New Arsenal")]
    public class Arsenal : ScriptableObject
    {
        private const int NUMBER_OF_GUN_SLOTS = 3;
        [SerializeField] private Gun.Gun[] allUnlockableGunPrefabs;
        [SerializeField] private StoredGun[] equippedGuns = new StoredGun[NUMBER_OF_GUN_SLOTS];


        public Gun.Gun[] AllUnlockableGunPrefabs 
        {
            get => allUnlockableGunPrefabs;
        }

        public int NumberOfGunSlots 
        {
            get { return NUMBER_OF_GUN_SLOTS; }
        }
    }
}