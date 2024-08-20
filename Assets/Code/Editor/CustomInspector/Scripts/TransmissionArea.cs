using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

namespace CustomInspector
{
    /// <summary>
    /// Class to cleanup the transmissionarea SciptableObject inspector
    /// </summary>
    [CustomEditor(typeof(EditorObject.TransmissionArea))]
    public class TransmissionArea : Editor
    {
        private bool showTransmissionArea = true;
        private bool showHealthPool = true;

        private string[] transmissionAreaProps = { "radius", "transmissionAreaStart", "startAngleDegrees", "deltaAngleDegrees", "clockwiseRotationAnglePerSecond", "outOfBoundsScale" };
        private string[] healthPoolProps = { "yScale", "minScale", "maxScale", "shrinkPerSecond" };


        public override void OnInspectorGUI() 
        {
            // HealthPool
            showHealthPool = EditorGUILayout.Foldout(showHealthPool, "Health Pool");

            if (showHealthPool) 
            {
                FindAndShowProperties(healthPoolProps);
            }

            // Transmission Area
            showTransmissionArea = EditorGUILayout.Foldout(showTransmissionArea, "Transmission Area");

            if (showTransmissionArea)
            {
                FindAndShowProperties(transmissionAreaProps);

            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Shows all input properties
        /// </summary>
        /// <param name="propNames">List of properties to show</param>
        private void FindAndShowProperties(string[] propNames) 
        {
            foreach (string name in propNames)
            {
                var prop = serializedObject.FindProperty(name);
                EditorGUILayout.PropertyField(prop);
            }
        }

            
    }
}