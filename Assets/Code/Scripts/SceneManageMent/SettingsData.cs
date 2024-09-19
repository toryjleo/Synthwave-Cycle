using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    [CreateAssetMenu(menuName = "Settings/SettingsData", fileName = "New Settings Data")]
    public class SettingsData : ScriptableObject
    {
        [SerializeField] private float mainVolume = 0f;
        [SerializeField] private float musicVolume = 0f;
        [SerializeField] private float effectsVolume = 0f;

        public float MainVolume { get => mainVolume; set => mainVolume = value; }

        public float MusicVolume { get => musicVolume; set => musicVolume = value; }

        public float EffectsVolume { get => effectsVolume; set => effectsVolume = value; }

        public void ResetToDefaults()
        {
            mainVolume = 0f;
            musicVolume = 0f;
            effectsVolume = 0f;
        }
    }
}
