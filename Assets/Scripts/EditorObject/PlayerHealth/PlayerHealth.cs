using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace EditorObject
{
[CreateAssetMenu(menuName = "EditorObject/PlayerHealth", fileName = "New PlayerHealth")]
    public class PlayerHealth : ScriptableObject
    {
        #region Fields
        /// <summary>
        /// The HP the player starts with
        /// </summary>
        [SerializeField] private float hpOnStart = 200.0f;
        /// <summary>
        /// HP amount to be reached to fill the 1st bar
        /// </summary>
        [SerializeField] private float barMax1HP = 1000.0f;
        /// <summary>
        /// HP amount to be reached to fill the 2nd bar
        /// </summary>
        [SerializeField] private float barMax2HP = 2000.0f;
        /// <summary>
        /// HP amount to be reached to fill the 3rd bar
        /// </summary>
        [SerializeField] private float barMax3HP = 3000.0f;
        #endregion

        #region Properties
        public float HpOnStart { get { return hpOnStart; } }
        public float BarMax1HP { get { return barMax1HP; } }
        public float BarMax2HP { get { return barMax2HP; } }
        public float BarMax3HP { get { return barMax3HP; } }
        #endregion
    }
}