using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    [CreateAssetMenu(menuName = "EditorObject/TransmissionArea", fileName = "New TransmissionArea")]
    public class TransmissionArea : ScriptableObject
    {
        #region Fields
        /// <summary>
        /// Radius of transmission area
        /// </summary>
        private float radius = 20;
        /// <summary>
        /// Rotation to spawn healthpool at on start
        /// </summary>
        private float startAngleDegrees = 0;
        /// <summary>
        /// Angle in degrees to apply to healthpool to rotate it around the transmission area
        /// </summary>
        private float deltaAngleDegrees = 60;
        #endregion

        #region Properties
        public float Radius { get => radius; }
        public float StartAngleDegrees { get => startAngleDegrees; }
        public float DeltaAngleDegrees { get => deltaAngleDegrees; }
        #endregion
    }
}
