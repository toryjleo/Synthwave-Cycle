using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    [CreateAssetMenu(menuName = "Game/GameSave", fileName = "New Game Save")]
    public class GameSave : ScriptableObject
    {
        [SerializeField] private int currentLevel = 0;
        [SerializeField] private int maxLevelProgress = 0;
        [SerializeField] private int gunTrackProgressLevel = 0;
        [SerializeField] private float gunTrackProgressPercent = 0f;
        [SerializeField] public GameLevel[] levelSequence;

        public int CurrentLevel { get => currentLevel; set => currentLevel = value; }

        public int MaxLevelProgess { get => maxLevelProgress; set => maxLevelProgress = value; }

        public int GunTrackProgressLevel { get => gunTrackProgressLevel; set => gunTrackProgressLevel = value; }

        public float GunTrackProgressPercent { get => gunTrackProgressPercent; }

        public void ResetToDefaults()
        {
            currentLevel = 0;
            maxLevelProgress = 0;
            gunTrackProgressLevel = 0;
            gunTrackProgressPercent = 0f;
        }
    }
}