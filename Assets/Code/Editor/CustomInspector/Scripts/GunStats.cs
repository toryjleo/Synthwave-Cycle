using EditorObject;
using GunState;
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

        private string[] generalProps = { "isPlayerGun", "isTurret", "isAutomatic", };
        private string[] accuracy = { "randomSpreadPerProjectile", "projectilesReleasedPerShot" };
        private string[] accuracyOverTime = { "deltaWindUpSpreadPerSecond", "deltaWindDownSpreadPerSecond" };
        private string[] timeBetweenShots = { "timeBetweenShots", "numBurstShots" };
        private string[] timeBetweenShotsOverTime = { "deltaWindupPercentTimeBetweenShots", "deltaWindDownPercentTimeBetweenShots",};
        private string[] overheatProps = { "coolDownPercentageBarrier", "overHeatPercentPerShot", "coolDownPerSecond" };
        private string[] explosionProps = { "radius", "force", "explosionDamage", "isCountDownExplosion" };

        #region Bullet Types
        private string[] areaOfEffectProps = { "damagePerSecond", "numPhases", };
        private string[] hitScanProps = { "range", "damageDealt", "bulletPenetration", };
        private string[] bulletProjectileProps = { "projectileScale", "muzzleVelocity", "damageDealt", "bulletPenetration" };
        #endregion

        private string[] countdownExplosionProps = { "secondsBeforeExplode" };

        #region Dropdown Toggles
        bool showAmmo = true;
        bool showBulletOptions = false;
        bool showAccuracy = false;
        bool showShotTiming = false;
        #region Bullet Effects
        bool showExplosions = true;
        bool showOverheat = true;
        #endregion
        bool showBulletType = true;
        #endregion

        #endregion

        private EditorObject.GunStats GetGunStats
        {
            get { return (EditorObject.GunStats)target; }

        }

        #region Editor
        public override void OnInspectorGUI()
        {
            EditorObject.GunStats gunStats = GetGunStats;

            

            if (gunStats != null)
            {
                GeneralProps(gunStats);
                BulletOptions(gunStats);
                Accuracy(gunStats);
                TimeBetweenShots(gunStats);
            }
            serializedObject.ApplyModifiedProperties();

        }
        #endregion


        #region Custom Methods
        /// <summary>
        /// Display Burst Fire options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void GeneralProps(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);

            FindAndShowProperties(generalProps);
        }

        /// <summary>
        /// Display Bullet options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void BulletOptions(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);

            showBulletOptions = EditorGUILayout.Foldout(showBulletOptions, "Bullet Options");

            if (showBulletOptions) 
            {

                showBulletType = EditorGUILayout.Foldout(showBulletType, "BulletType");
                if (showBulletType)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletType"));

                    HitScan(gunStats);
                    AreaOfEffect(gunStats);
                    BulletProjectile(gunStats);
                }
                

                showAmmo = EditorGUILayout.Foldout(showAmmo, "Ammunition");
                if (showAmmo)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hasInfiniteAmmo"));
                    if (!gunStats.HasInfiniteAmmo)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("ammoCount"));
                    }
                    Overheat(gunStats);
                    Explosions(gunStats);
                }
            }
        }

        /// <summary>
        /// Display Burst Fire options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void Accuracy(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);

            showAccuracy = EditorGUILayout.Foldout(showAccuracy, "Accuracy");
            if (showAccuracy) 
            {
                FindAndShowProperties(accuracy);
                if (gunStats.ProjectilesReleasedPerShot > 1)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("angleBetweenProjectiles"));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("accuracyArrivalOverTime"));
                if (gunStats.AccuracyArrivalOverTime != 1)
                {
                    FindAndShowProperties(accuracyOverTime);
                }
            }
        }

        /// <summary>
        /// Display Time Between Shots Options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void TimeBetweenShots(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);

            showShotTiming = EditorGUILayout.Foldout(showShotTiming, "Shot Timing");

            if (showShotTiming) 
            {
                FindAndShowProperties(timeBetweenShots);

                //Burst Fire checkbox
                if (gunStats.IsBurstFire)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenBurstShots"));
                }


                EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenShotsScalingOverTime"));
                if (gunStats.TimeBetweenShotsScalingOverTime != 1)
                {
                    FindAndShowProperties(timeBetweenShotsOverTime);
                }


                // Time Between Player Fire
                float timeBetweenPlayerFire = gunStats.TimeBetweenShots;
                if (gunStats.IsBurstFire)
                {
                    timeBetweenPlayerFire += ((gunStats.NumBurstShots - 1) * gunStats.TimeBetweenBurstShots);
                }
                EditorGUILayout.LabelField("Time Between Player Fire (without time change): " + timeBetweenPlayerFire + " seconds");
            }

        }

        #region Bullet Effects
        /// <summary>
        /// Display Explosion options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void Explosions(EditorObject.GunStats gunStats)
        {

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isExplosive"));
            if (gunStats.IsExplosive)
            {
                showExplosions = EditorGUILayout.Foldout(showExplosions, "Explosions");
                if (showExplosions)
                {
                    FindAndShowProperties(explosionProps);
                    if (gunStats.IsCountDownExplosion)
                    {
                        FindAndShowProperties(countdownExplosionProps);
                    }
                }
            }
        }

        /// <summary>
        /// Display Overheat options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void Overheat(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("canOverheat"));
            if (gunStats.CanOverheat)
            {
                showOverheat = EditorGUILayout.Foldout(showOverheat, "Overheat");

                if (showOverheat)
                {
                    FindAndShowProperties(overheatProps);
                    // Time To Cool After Overheat
                    float timeToCoolAfterOverheat = (100 - gunStats.CoolDownPercentageBarrier) / gunStats.CoolDownPerSecond;
                    if (gunStats.CanOverheat)
                    {
                        EditorGUILayout.LabelField("Time To Cool After Overheat: " + timeToCoolAfterOverheat + " seconds");
                    }
                }
            }
        }
        #endregion

        #region BulletType Dependants
        /// <summary>
        /// Display AOE options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void AreaOfEffect(EditorObject.GunStats gunStats)
        {
            if (gunStats.IsAreaOfEffect)
            {
                FindAndShowProperties(areaOfEffectProps);

                float lifetime = 0;

                switch (gunStats.NumPhases)
                {
                    case Gun.AOEPhases.Persistant:
                        break;
                    case Gun.AOEPhases.OnePhase:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("phase1"));
                        lifetime = gunStats.Phase1.DurationInSeconds;
                        break;
                    case Gun.AOEPhases.TwoPhase:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("phase1"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("phase2"));
                        lifetime = gunStats.Phase1.DurationInSeconds + gunStats.Phase2.DurationInSeconds;
                        break;
                    default:
                        break;
                }

                EditorGUILayout.LabelField("Lifetime of AOE: " + lifetime + " seconds");
            }
        }


        /// <summary>
        /// Display HitScan options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void HitScan(EditorObject.GunStats gunStats)
        {
            if (gunStats.IsHitScan)
            {
                FindAndShowProperties(hitScanProps);
            }
        }

        /// <summary>
        /// Display BulletProjectile options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void BulletProjectile(EditorObject.GunStats gunStats)
        {
            if (gunStats.IsBulletProjectile)
            {
                FindAndShowProperties(bulletProjectileProps);
            }
        }
        #endregion

        /// <summary>
        /// A utility method to show all input properties
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