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
        private string[] generalProps = { "isPlayerGun", "isTurret", "isAutomatic", "timeBetweenShots", "damageDealt" };
        private string[] burstFireProps = { "numBurstShots", "timeBetweenBurstShots" };
        private string[] overheatProps = { "overHeatBarrier", "overHeatPercentPerShot", "coolDownPerSecond" };
        private string[] multipleProjectileProps = { "projectileCountPerShot", "angleBetweenProjectiles", "randomAngleVariationPerProjectile" };

        public override void OnInspectorGUI()
        {
            EditorObject.GunStats gunStats = GetGunStats();

            FindAndShowProperties(generalProps);

            if (gunStats != null)
            {
                Ammunition(gunStats);
                BurstFire(gunStats);
                MultipleProjectiles(gunStats);
                Overheat(gunStats);
                BulletOptions(gunStats);
            }
            serializedObject.ApplyModifiedProperties();
            
        }

        private EditorObject.GunStats GetGunStats()
        {
            return (EditorObject.GunStats)target;
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
            else 
            {
                gunStats.BurstShotsOff();
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
        /// Display Overheat options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void Overheat(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Overheat");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("canOverheat"));
            //Overheat checkbox
            if (gunStats.CanOverheat)
            {
                FindAndShowProperties(overheatProps);
            }
        }

        /// <summary>
        /// Display Multiple Projectile options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void MultipleProjectiles(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Multiple Projectiles");
            FindAndShowProperties(multipleProjectileProps);
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