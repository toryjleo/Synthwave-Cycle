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
        int prefabListIdx;
        int ammoCount;
    }

    [CreateAssetMenu(menuName = "Game/Arsenal", fileName = "New Arsenal")]
    public class Arsenal : ScriptableObject
    {
        private const int NumberOfGunSlots = 3;
        [SerializeField] private Gun.Gun[] allUnlockableGunPrefabs;
        [SerializeField] private StoredGun[] equippedGuns = new StoredGun[NumberOfGunSlots];

    }
}