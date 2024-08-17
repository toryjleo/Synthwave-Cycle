using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace EditorObject
{
    /// <summary>
    /// Stores the data for a given gear to be used by PlayerMovement
    /// </summary>
    [CreateAssetMenu(menuName = "EditorObject/Gear", fileName = "New Gear")]

    public class Gear : ScriptableObject
    {
        #region Fields
        /// <summary>
        /// Linear scale of graph's x
        /// </summary>
        [SerializeField] private float xScale = 4;
        /// <summary>
        /// Linear scale of velocity and acceleration
        /// </summary>
        [SerializeField] private float yScale = 20.0f;
        /// <summary>
        /// Angle off of inputDirection in which to start applying force upon forward vector aligning
        /// </summary>
        [SerializeField] private float theta = 20;
        /// <summary>
        /// Amount of drag to apply to the tangent of movement
        /// </summary>
        [SerializeField] private float tangentDrag = 35;
        /// <summary>
        /// Amount of drag to apply to resist forward movement
        /// </summary>
        [SerializeField] private float forwardDrag = 35;
        /// <summary>
        /// How fast to rotate the player when turning
        /// </summary>
        [SerializeField] private float rotationSpeed = 5;
        /// <summary>
        /// Speeds up the time it takes to get to full velocity on the graph
        /// </summary>
        [SerializeField] private float graphTraversalSpeed = 1;
        #endregion

        #region Properties
        public float XScale { get { return xScale; } }
        public float YScale { get { return yScale; } }
        public float Theta { get { return theta; } }
        public float TangentDrag { get { return tangentDrag; } }
        public float ForwardDrag { get { return forwardDrag; } }
        public float RotationSpeed { get { return rotationSpeed; } }
        public float GraphTraversalSpeed { get { return graphTraversalSpeed; } }
        #endregion


    }
}
