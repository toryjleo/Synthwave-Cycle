using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    /// <summary>
    /// Holds the level and weapon unlock progress, also defines the sequence of levels in order
    /// </summary>
    [CreateAssetMenu(menuName = "Game/GameSave", fileName = "New Game Save")]
    public class GameSave : ScriptableObject
    {
        [SerializeField] private int currentLevel = 0;
        [SerializeField] private int maxLevelProgress = 0;
        [SerializeField] private int gunTrackProgressLevel = 0;
        [SerializeField] private float gunTrackProgressPercent = 0f;
        [SerializeField] public GameLevel[] levelSequence;
        [SerializeField] public EditorObject.Arsenal arsenal;

        public int CurrentLevel { get => currentLevel; set => currentLevel = value; }

        public int MaxLevelProgess { get => maxLevelProgress; set => maxLevelProgress = value; }

        public int GunTrackProgressLevel { get => gunTrackProgressLevel; set => gunTrackProgressLevel = value; }

        public float GunTrackProgressPercent { get => gunTrackProgressPercent; }

        /// <summary>
        /// Set this editorobject to its initial state, the first time the game is launched.
        /// </summary>
        public void ResetToDefaults()
        {
            currentLevel = 0;
            maxLevelProgress = 0;
            gunTrackProgressLevel = 0;
            gunTrackProgressPercent = 0f;

            ResetArsenalToDefaults(this.arsenal);
        }

        /// <summary>
        /// Set the arsenal editorobject that stores the player gun data to its initial state, the first time the game is launched.
        /// </summary>
        /// <param name="arsenal">Arsenal to reset</param>
        private void ResetArsenalToDefaults(Arsenal arsenal) 
        {
            if (arsenal == null)
            {
                Debug.LogError("GameSave EditorObject does not have reference to Arsenal");
            }
            else
            {
                arsenal.ResetToDefaults();
            }
        }
    }
}