using EditorObject;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace CustomInspector
{
    /// <summary>
    /// Adds custom layout for the gunstats inspector
    /// </summary>
    [CustomEditor(typeof(EditorObject.GunStats))]
    public class GunStats : Editor
    {
        #region Members
        private string[] generalProps = { "isPlayerGun", "isTurret", "isAutomatic", "timeBetweenShots", "damageDealt" };
        private string[] burstFireProps = { "timeBetweenBurstShots" };
        private string[] overheatProps = { "overHeatBarrier", "overHeatPercentPerShot", "coolDownPerSecond" };
        private string[] multipleProjectileProps = { "angleBetweenProjectiles" };
        #endregion

        private EditorObject.GunStats GetGunStats
        {
            get { return (EditorObject.GunStats)target; }

        }

        #region Editor
        public override void OnInspectorGUI()
        {
            EditorObject.GunStats gunStats = GetGunStats;

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
        #endregion


        #region Custom Methods
        /// <summary>
        /// Display Burst Fire options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void BurstFire(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Burst Fire");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("numBurstShots"));
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileCountPerShot"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomAngleVariationPerProjectile"));
            if (gunStats.ProjectileCountPerShot > 1) 
            {
                FindAndShowProperties(multipleProjectileProps);
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
#endregion
    }
}