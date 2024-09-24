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
        private const int SECTION_SPACE = 8;

        private string[] generalProps = { "isPlayerGun", "isTurret", "isAutomatic", "timeBetweenShots", "projectileSpread", "damageDealt" };
        private string[] burstFireProps = { "timeBetweenBurstShots" };
        private string[] overheatProps = { "coolDownBarrier", "overHeatPercentPerShot", "coolDownPerSecond" };
        private string[] multipleProjectileProps = { "distanceBetweenProjectiles" };
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

                GeneratedStats(gunStats);
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
            EditorGUILayout.Space(SECTION_SPACE);

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
            EditorGUILayout.Space(SECTION_SPACE);

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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileScale"));
            }
        }

        /// <summary>
        /// Display Ammo options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void Ammunition(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);

            EditorGUILayout.LabelField("Ammo");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletPenetration"));
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
            EditorGUILayout.Space(SECTION_SPACE);

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
            EditorGUILayout.Space(SECTION_SPACE);

            EditorGUILayout.LabelField("Multiple Projectiles");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileCountPerShot"));
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

        /// <summary>
        /// Prints labels with statistics generated by specified stats
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void GeneratedStats(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);
            EditorGUILayout.LabelField("Generated Stats");

            // Time Between Player Fire
            float timeBetweenPlayerFire = gunStats.TimeBetweenShots;
            if (gunStats.IsBurstFire)
            {
                timeBetweenPlayerFire += ((gunStats.NumBurstShots - 1) * gunStats.TimeBetweenBurstShots);
            }
            EditorGUILayout.LabelField("Time Between Player Fire: " + timeBetweenPlayerFire + " seconds");

            // Time To Cool After Overheat
            float timeToCoolAfterOverheat = (100 - gunStats.CoolDownBarrier) / gunStats.CoolDownPerSecond;
            if (gunStats.CanOverheat) 
            {
                EditorGUILayout.LabelField("Time To Cool After Overheat: " + timeToCoolAfterOverheat + " seconds");
            }
        }
        #endregion
    }
}