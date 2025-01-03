using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    public enum BulletType
    {
        BulletProjectile,
        HitScan,
        AreaOfEffect,
    }

    [System.Serializable]
    public struct AOEPhase 
    {
        /// <summary>
        /// In seconds
        /// </summary>
        public float DurationInSeconds;
        /// <summary>
        /// In meters/second
        /// </summary>
        public float RateOfScaleGrowth;
    }

    /// <summary>
    /// Stores the data for a Gun component
    /// </summary>
    [CreateAssetMenu(menuName = "EditorObject/GunStats", fileName = "New GunStats")]
    public class GunStats : ScriptableObject, Generic.IPoolableInstantiateData
    {




        #region Members
        #region General
        /// <summary>
        /// If the gun is for a player. Else an enemy
        /// </summary>
        [SerializeField] private bool isPlayerGun = true;
        /// <summary>
        /// If the gun will follow the mouse
        /// </summary>
        [SerializeField] private bool isTurret = false;
        /// <summary>
        /// If the gun will continue shooting on fire held down
        /// </summary>
        [SerializeField] private bool isAutomatic = true;
        #endregion
        #region Bullet Options
        /// <summary>
        /// Type of bullet to shoot
        /// </summary>
        [SerializeField] private BulletType bulletType;
        /// <summary>
        /// Number of enemies that bullet can penetrate before despawning
        /// </summary>
        [Range(0, 20)][SerializeField] private int bulletPenetration;

                /// <summary>
        /// Number of hit points to depleat on bullet hit
        /// </summary>
        [Range(0, 1000)][SerializeField] private float damageDealt = 70;

        #region Ammo
        /// <summary>
        /// If the gun has infinite ammo
        /// </summary>
        [SerializeField] private bool hasInfiniteAmmo = true;
        /// <summary>
        /// Number of shots before the gun runs out of ammo
        /// </summary>
        [Range(1, 5000)][SerializeField] private int ammoCount = 200;
        #endregion

        #region Multiple Projectiles
        /// <summary>
        /// Number of projectiles shot per gun shot
        /// </summary>
        [Range(1, 40)][SerializeField] private int projectilesReleasedPerShot = 1;
        /// <summary>
        /// Adds a random variation to each projectile's shot
        /// </summary>
        [Range(0, 180)][SerializeField] private float randomSpreadPerProjectile = 0;
        #endregion

        #region DEBUG
        /// <summary>
        /// If the gun should print its state
        /// </summary>
        [SerializeField] private bool printDebugState = false;
        #endregion


        #region Projectile
        /// <summary>
        /// Speed at which the bullet travels out of the gun
        /// </summary>
        [Range(0f, 100f)][SerializeField] private float muzzleVelocity = 60;
        [SerializeField] private Vector3 projectileScale = Vector3.one;
        #endregion

        #region HitScan
        /// <summary>
        /// Maximum range the projectile travels
        /// </summary>
        [Range(0f, 200f)][SerializeField] private float range = 100f;
        #endregion

        #region Explosion
        [SerializeField] private bool isExplosive = false;
        [SerializeField] private bool isCountDownExplosion = false;
        [SerializeField] private float radius = 5.0f;
        [SerializeField] private float force = 12000;
        [SerializeField] private float explosionDamage = 25;
        [SerializeField] private float secondsBeforeExplode = 1.0f;
        #endregion

        #region Overheat
        /// <summary>
        /// If the gun can overheat from firing
        /// </summary>
        [SerializeField] private bool canOverheat = false;
        /// <summary>
        /// The percentage the gun must cool down to before shooting again
        /// </summary>
        [Range(0, 100)][SerializeField] private float coolDownPercentageBarrier = 50;
        /// <summary>
        /// How much each shot will overheat the gun
        /// </summary>
        [Range(1, 100)][SerializeField] private float overHeatPercentPerShot = 5;
        /// <summary>
        /// Percentage the gun cools down each second
        /// </summary>
        [Range(0, 100)][SerializeField] private float coolDownPerSecond = 2.5f;
        #endregion

        #region Area Of Effect
        [Range(0, 1000)][SerializeField] private float damagePerSecond = 10;
        [SerializeField] private Gun.AOEPhases numPhases = Gun.AOEPhases.Persistant;
        [SerializeField] private AOEPhase phase1;
        [SerializeField] private AOEPhase phase2;
        #endregion
        #endregion

        #region Accuracy

        /// <summary>
        /// Angle between each projectile shot
        /// </summary>
        [Range(0, 180)][SerializeField] private float angleBetweenProjectiles = 0;


        [Range(.001f, 400f)][SerializeField] private float accuracyArrivalOverTime = 100;

        [Range(1, 90f)][SerializeField] private float deltaWindUpSpreadPerSecond = 0;
        [Range(1, 100f)][SerializeField] private float deltaWindDownSpreadPerSecond = 0;
        #endregion

        #region TimeBetweenShots
        /// <summary>
        /// Number of seconds after the final shot the gun must wait before shooting again
        /// </summary>
        [Range(0.01f, 5f)][SerializeField] private float timeBetweenShots = 10f;
        /// <summary>
        /// Number of bullet bursts per fire action
        /// </summary>
        [Range(1, 20)][SerializeField] private int shotBurstCount = 1;

        /// <summary>
        /// Time between each bullet burst
        /// </summary>
        [Range(0.001f, 1)][SerializeField] private float timeBetweenBurstShots = .01f;
        [Range(0.01f, 400)][SerializeField] private float timeBetweenShotsScalingOverTime = 100.0f;

        [Range(0.01f, 100)][SerializeField] private float deltaWindupPercentTimeBetweenShots = 0.0f;
        [Range(0.01f, 100)][SerializeField] private float deltaWindDownPercentTimeBetweenShots = 0.0f;

        #endregion
        #endregion


        #region Properties
        public bool IsTurret { get { return isTurret; } }
        public bool IsAutomatic { get { return isAutomatic; } }
        public bool IsPlayerGun { get { return isPlayerGun; } }

        #region Bullet Options
        public BulletType BulletType { get { return bulletType; } }
        public float DamageDealt { get { return damageDealt; } }
        public int BulletPenetration { get { return bulletPenetration; } }
        #region Projectile
        public float MuzzleVelocity { get { return muzzleVelocity; } }
        public Vector3 ProjectileScale { get { return projectileScale; } }
        #endregion

        #region HitScan
        public bool IsHitScan { get { return BulletType == EditorObject.BulletType.HitScan; } }
        /// <summary>
        /// Maximum range the projectile travels
        /// </summary>
        public float Range { get { return range; } }
        #endregion

        public bool IsBulletProjectile { get { return BulletType == EditorObject.BulletType.BulletProjectile; } }

        #region Area of Effect
        public bool IsAreaOfEffect { get { return BulletType == EditorObject.BulletType.AreaOfEffect; } }
        public float DamagePerSecond { get { return damagePerSecond; } }
        public Gun.AOEPhases NumPhases { get { return numPhases; } }
        public AOEPhase Phase1 { get { return phase1; } }
        public AOEPhase Phase2 { get { return phase2; } }
        #endregion

        public bool IsCountDownExplosion { get { return isCountDownExplosion; } }

        #region Ammo
        public bool HasInfiniteAmmo { get { return hasInfiniteAmmo; } }
        public int AmmoCount { get { return ammoCount; } }
        #endregion

        #region Multiple Projectiles
        public int ProjectilesReleasedPerShot { get { return projectilesReleasedPerShot; } }
        public float RandomSpreadPerProjectile { get { return randomSpreadPerProjectile; } }
        #endregion

        public bool IsBurstFire { get { return shotBurstCount > 1; } }

        #region Overheat
        public bool CanOverheat { get { return canOverheat; } }
        public float CoolDownPercentageBarrier { get { return coolDownPercentageBarrier; } }
        public float OverHeatPercentPerShot { get { return overHeatPercentPerShot; } }
        public float CoolDownPerSecond { get { return coolDownPerSecond; } }
        #endregion

        #region Explosion
        public bool IsExplosive { get { return isExplosive; } }
        public float Radius { get { return radius; } }
        public float Force { get { return force; } }
        public float ExplosionDamage { get { return explosionDamage; } }
        public float SecondsBeforeExplode { get { return secondsBeforeExplode; } }
        #endregion
        #endregion


        #region Accuracy
        public float AngleBetweenProjectiles { get { return angleBetweenProjectiles; } }
        public float AccuracyArrivalOverTime { get { return accuracyArrivalOverTime / 100f; } }
        public float DeltaWindUpSpreadPerSecond { get { return deltaWindUpSpreadPerSecond / 100f; } }
        public float DeltaWindDownSpreadPerSecond { get { return deltaWindDownSpreadPerSecond / 100f; } }
        #endregion

        #region Time Between Shots
        public float TimeBetweenShots { get { return timeBetweenShots; } }
        public int ShotBurstCount { get { return shotBurstCount; } }
        public float TimeBetweenBurstShots { get { return timeBetweenBurstShots; } }
        public float TimeBetweenShotsScalingOverTime { get { return timeBetweenShotsScalingOverTime / 100f; } }
        public float DeltaWindupPercentTimeBetweenShots { get { return deltaWindupPercentTimeBetweenShots / 100f; } }
        public float DeltaWindDownPercentTimeBetweenShots { get { return deltaWindDownPercentTimeBetweenShots / 100f; } }
        #endregion

        #region DEBUG
        public bool PrintDebugState { get { return printDebugState; } }
        #endregion

        #endregion
    }
}