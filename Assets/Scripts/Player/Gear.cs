using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data for a given gear to be used by PlayerMovement
/// </summary>
[CreateAssetMenu(menuName = "EditorObject/Gear", fileName = "New Gear")]
public class Gear : ScriptableObject
{
    #region Fields
    [SerializeField] private float xScale = 4;
    [SerializeField] private float yScale = 20.0f;
    [SerializeField] private float theta = 20;
    [SerializeField] private float drag = 35;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float graphTraversalSpeed = 1;
    #endregion

    #region Properties
    public float XScale { get { return xScale; } }
    public float YScale { get { return yScale; } }
    public float Theta { get { return theta; } }
    public float Drag { get { return drag; } }
    public float RotationSpeed { get {  return rotationSpeed; } }
    public float GraphTraversalSpeed { get { return graphTraversalSpeed; } }
    #endregion


}
