using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace CustomInspector
{
    [CustomEditor(typeof(EditorObject.GunStats))]
    public class GunStats : Editor
    {
        private string[] generalProps = { "isTurret", "isAutomatic", "isPlayerBullet", "isBurstFire", "timeBetweenShots", "damageDealt" };
        private string[] burstFireProps = { "numBurstShots", "timeBetweenBurstShots" };

        public override void OnInspectorGUI()
        {
            EditorObject.GunStats gunStats = (EditorObject.GunStats)target;

            FindAndShowProperties(generalProps);

            //Burst Fire checkbox
            if (gunStats.IsBurstFire)
            {
                FindAndShowProperties(burstFireProps);
            }

            EditorGUILayout.LabelField("Bullet Options");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletType"));
            //Bullet Type is ray cast
            if (gunStats.BulletType == EditorObject.BulletType.HitScan)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("range"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzleVelocity"));
            }

            // Ammunition
            EditorGUILayout.LabelField("Ammo");
            if (!gunStats.InfiniteAmmo)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("magazineSize"));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("infiniteAmmo"));

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