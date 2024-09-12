using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomInspector
{
    [CustomEditor(typeof(EditorObject.GunStats))]
    public class GunStats : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorObject.GunStats gunStats = (EditorObject.GunStats)target;
            if (gunStats != null)
            {
                EditorGUILayout.LabelField("Yuh");
            }
        }
    }
}