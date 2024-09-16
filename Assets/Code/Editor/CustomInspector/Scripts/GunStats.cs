using EditorObject;
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
        private string[] generalProps = { "isPlayerGun", "isTurret", "isAutomatic", "timeBetweenShots", "damageDealt", "projectileCountPerShot", "angleBetweenProjectiles", "randomAngleVariationPerProjectile" };
        private string[] burstFireProps = { "numBurstShots", "timeBetweenBurstShots" };

        public override void OnInspectorGUI()
        {
            EditorObject.GunStats gunStats = (EditorObject.GunStats)target;

            FindAndShowProperties(generalProps);

            if (gunStats != null) 
            {

                BurstFire(gunStats);
                BulletOptions(gunStats);
                Ammunition(gunStats);
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Display Burst Fire options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void BurstFire(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Burst Fire");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isBurstFire"));
            //Burst Fire checkbox
            if (gunStats.IsBurstFire)
            {
                FindAndShowProperties(burstFireProps);
            }
        }

        /// <summary>
        /// Display Bullet options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void BulletOptions(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(5);

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
        }

        /// <summary>
        /// Display Ammo options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void Ammunition(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Ammo");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("infiniteAmmo"));
            if (!gunStats.InfiniteAmmo)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ammoCount"));
            }
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