using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    [CreateAssetMenu(menuName = "EditorObject/TransmissionArea", fileName = "New TransmissionArea")]
    public class TransmissionArea : ScriptableObject
    {
        #region TransmissionArea
        #region Fields
        /// <summary>
        /// Radius of transmission area
        /// </summary>
        [SerializeField] private float radius = 20;
        /// <summary>
        /// The starting location of the transmissionArea
        /// </summary>
        [SerializeField] private Vector3 transmissionAreaStart = Vector3.zero;
        /// <summary>
        /// Rotation to spawn healthpool at on start
        /// </summary>
        [SerializeField] private float startAngleDegrees = 0;
        /// <summary>
        /// Angle in degrees to apply to healthpool to rotate it around the transmission area
        /// </summary>
        [SerializeField] private float deltaAngleDegrees = 60;
        /// <summary>
        /// Angle in degrees for the healthpool to rotate around the transmission area in a clockwise fashion
        /// </summary>
        [SerializeField] private float clockwiseRotationAnglePerSecond = 0;
        /// <summary>
        /// Linearly scales how far the player can travel outside the transmission area before they are out of bounds
        /// </summary>
        [SerializeField] private float outOfBoundsScale = 2;
        #endregion

        #region Properties
        public float Radius { get => radius; }
        public Vector3 TransmissionAreaStart { get => transmissionAreaStart; }
        public float StartAngleDegrees { get => startAngleDegrees; }
        public float DeltaAngleDegrees { get => deltaAngleDegrees; }
        public float ClockwiseRotationAnglePerSecond { get => clockwiseRotationAnglePerSecond; }
        public float OutOfBoundsScale { get => outOfBoundsScale; }
        #endregion
        #endregion

        #region HealthPool
        /// <summary>
        /// The smallest the healthpool can shrink to before despawning
        /// </summary>
        [SerializeField] private float minScale = .2f;
        /// <summary>
        /// The scale the healthpool starts at
        /// </summary>
        [SerializeField] private float maxScale = 5.0f;
        /// <summary>
        /// Controls the Healthpool's y-scale. The healthpool must be tall enough to collide with the player.
        /// </summary>
        [SerializeField] private float yScale = 2.0f;
        /// <summary>
        /// The scale for the healthpool to shrink every second
        /// </summary>
        [SerializeField] private float shrinkPerSecond = .75f;

        #region Properties
        public float MinScale {  get => minScale; }
        public float MaxScale { get => maxScale;}
        public float YScale { get => yScale; }
        public float ShrinkPerSecond { get => shrinkPerSecond; }
        #endregion
        #endregion
    }
}
