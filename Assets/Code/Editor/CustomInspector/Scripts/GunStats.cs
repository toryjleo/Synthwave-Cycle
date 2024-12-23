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
        private string[] timeBetweenShots = { "timeBetweenShots", "deltaWindupPercentTimeBetweenShots", "deltaWindDownPercentTimeBetweenShots", "maxPercentTimeBetweenShots" };
        private string[] overheatProps = { "coolDownBarrier", "overHeatPercentPerShot", "coolDownPerSecond" };
        private string[] explosionProps = { "radius", "force", "explosionDamage", "isCountDownExplosion" };
        private string[] areaOfEffectProps = { "numPhases" };
        private string[] countdownExplosionProps = { "secondsBeforeExplode" };
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
                Accuracy(gunStats);
                Damage(gunStats);
                Ammunition(gunStats);
                TimeBetweenShots(gunStats);
                MultipleProjectiles(gunStats);
                Overheat(gunStats);
                BulletOptions(gunStats);
                Explosions(gunStats);
                AreaOfEffect(gunStats);

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
        private void Accuracy(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);

            EditorGUILayout.LabelField("Accuracy");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileSpread"));
            if (gunStats.ProjectileCountPerShot > 1)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceBetweenProjectiles"));
            }

            FindAndShowProperties(accuracyOverTime);

        }

        /// <summary>
        /// Display Time Between Shots Options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void TimeBetweenShots(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);

            EditorGUILayout.LabelField("Time Between Shots");

            FindAndShowProperties(timeBetweenShots);

            //Burst Fire checkbox
            if (gunStats.IsBurstFire)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenBurstShots"));
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("numBurstShots"));


            switch (gunStats.BulletType) 
            {
                case EditorObject.BulletType.HitScan:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("range"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("damageDealt"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletPenetration"));
                    break;
                case EditorObject.BulletType.AreaOfEffect:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("damagePerSecond"));
                    break;
                case EditorObject.BulletType.BulletProjectile:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzleVelocity"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileScale"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("damageDealt"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletPenetration"));
                    break;

            }
        }

        /// <summary>
        /// Display Damage options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void Damage(EditorObject.GunStats gunStats) 
        {


        }

        /// <summary>
        /// Display Ammo options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void Ammunition(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);

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
            
        }

        /// <summary>
        /// Display Explosion options
        /// </summary>
        /// <param name="gunStats">ScriptableObject to modify</param>
        private void Explosions(EditorObject.GunStats gunStats)
        {
            EditorGUILayout.Space(SECTION_SPACE);

            EditorGUILayout.LabelField("Explosions");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isExplosive"));
            if (gunStats.IsExplosive)
            {
                FindAndShowProperties(explosionProps);
                if (gunStats.IsCountDownExplosion)
                {
                    FindAndShowProperties(countdownExplosionProps);
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
                EditorGUILayout.Space(SECTION_SPACE);

                EditorGUILayout.LabelField("Area Of Effect");

                FindAndShowProperties(areaOfEffectProps);

                switch (gunStats.NumPhases) 
                {
                    case Gun.AOEPhases.Persistant:
                        break;
                    case Gun.AOEPhases.OnePhase:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("phase1"));
                        break;
                    case Gun.AOEPhases.TwoPhase:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("phase1"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("phase2"));
                        break;
                    default: 
                        break;
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