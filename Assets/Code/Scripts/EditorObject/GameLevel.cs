using UnityEngine;

namespace EditorObject
{
    /// <summary>
    /// Holds information for a specific level in the game, including the ground material and wave sequence
    /// </summary>
    [CreateAssetMenu(menuName = "EditorObject/Game Level", fileName = "New Game Level")]
    public class GameLevel : ScriptableObject
    {
        #region Fields
        /// <summary>
        /// Wave Sequence to be played through
        /// </summary>
        [SerializeField] private WaveSequence waveSequence;
        /// <summary>
        /// Material to use for the ground texture
        /// </summary>
        [SerializeField] private ImpactMapping groundMatAndParticle;
        /// <summary>
        /// Name of the level
        /// </summary>
        [SerializeField] private string levelName;
        /// <summary>
        /// Image to be used in Level Select
        /// </summary>
        [SerializeField] private Sprite levelImage;
        /// <summary>
        /// Level cosmetics including particle effects and other visuals
        /// </summary>
        [SerializeField] private GameObject[] levelCosmetics;
        /// <summary>
        /// The place in the level sequence this level is
        /// </summary>
        [SerializeField] private int levelNumber;
        #endregion

        #region Properties
        public WaveSequence WaveSequence { get { return waveSequence; } }
        public Material GroundMat { get { return groundMatAndParticle.Material; } }
        public ImpactMapping GroundImpactMapping { get { return groundMatAndParticle; } }
        public string LevelName { get { return levelName; } }
        public Sprite LevelImage { get { return levelImage; } }
        public GameObject[] LevelCosmetics { get { return levelCosmetics; } }
        public int LevelNumber { get { return levelNumber; } }
        #endregion
    }
}