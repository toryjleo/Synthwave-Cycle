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

        private string[] generalProps = { "isPlayerGun", "isTurret", "isAutomatic", };
        private string[] accuracyOverTime = { "maxChangeToNormalAccuracy", "deltaWindUpSpreadPerSecond", "deltaWindDownSpreadPerSecond" };
        private string[] timeBetweenShots = { "timeBetweenShots", "deltaWindupPercentTimeBetweenShots", "deltaWindDownPercentTimeBetweenShots", "maxPercentTimeBetweenShots", "numBurstShots", "projectilesReleasedPerShot" };
        private string[] overheatProps = { "coolDownPercentageBarrier", "overHeatPercentPerShot", "coolDownPerSecond" };
        private string[] explosionProps = { "radius", "force", "explosionDamage", "isCountDownExplosion" };
        private string[] areaOfEffectProps = { "damagePerSecond", "numPhases", };

        private string[] countdownExplosionProps = { "secondsBeforeExplode" };

        bool showAmmo = true;
        bool showBulletOptions = false;
        bool showAccuracy = false;
        bool showShotTiming = false;
        bool showExplosions = false;
        bool showAreaOfEffect = true;
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
                showAmmo = EditorGUILayout.Foldout(showAmmo, "Ammunition");
                if (showAmmo)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hasInfiniteAmmo"));
                    if (!gunStats.HasInfiniteAmmo)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("ammoCount"));
                    }
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("canOverheat"));
                    //Overheat checkbox
                    if (gunStats.CanOverheat)
                    {
                        FindAndShowProperties(overheatProps);
                        // Time To Cool After Overheat
                        float timeToCoolAfterOverheat = (100 - gunStats.CoolDownPercentageBarrier) / gunStats.CoolDownPerSecond;
                        if (gunStats.CanOverheat)
                        {
                            EditorGUILayout.LabelField("Time To Cool After Overheat: " + timeToCoolAfterOverheat + " seconds");
                        }
                    }
                    Explosions(gunStats);

                }
                EditorGUILayout.Space(SECTION_SPACE);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletType"));

                switch (gunStats.BulletType)
                {
                    case EditorObject.BulletType.HitScan:
                        // TODO: HitScan DropDown
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("range"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("damageDealt"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletPenetration"));
                        break;
                    case EditorObject.BulletType.AreaOfEffect:
                        AreaOfEffect(gunStats);
                        break;
                    case EditorObject.BulletType.BulletProjectile:
                        // TODO: BulletProjectile dropdown
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzleVelocity"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileScale"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("damageDealt"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletPenetration"));
                        break;
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
            if(showAccuracy) 
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileSpread"));
                if (gunStats.ProjectilesReleasedPerShot > 1)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceBetweenProjectiles"));
                }

                FindAndShowProperties(accuracyOverTime);
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
            }

            // Time Between Player Fire
            float timeBetweenPlayerFire = gunStats.TimeBetweenShots;
            if (gunStats.IsBurstFire)
            {
                timeBetweenPlayerFire += ((gunStats.NumBurstShots - 1) * gunStats.TimeBetweenBurstShots);
            }
            EditorGUILayout.LabelField("Time Between Player Fire: " + timeBetweenPlayerFire + " seconds");
        }

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
        /// Display AOE options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void AreaOfEffect(EditorObject.GunStats gunStats)
        {
            if (gunStats.IsAreaOfEffect)
            {

                showAreaOfEffect = EditorGUILayout.Foldout(showAreaOfEffect, "Area Of Effect");

                if (showAreaOfEffect) 
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

        // TODO: Move GeneratedStats to relevant field

        /// <summary>
        /// Prints labels with statistics generated by specified stats
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void GeneratedStats(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);
            EditorGUILayout.LabelField("Generated Stats");




        }
        #endregion
    }
}