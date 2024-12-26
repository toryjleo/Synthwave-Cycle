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
        /// <summary>
        /// Number of seconds after the final shot the gun must wait before shooting again
        /// </summary>
        [Range(0.01f, 20f)][SerializeField] private float timeBetweenShots = 10f;

        [Range(0.01f, 100)][SerializeField] private float deltaWindupPercentTimeBetweenShots = 0.0f;
        [Range(0.01f, 100)][SerializeField] private float deltaWindDownPercentTimeBetweenShots = 0.0f;
        [Range(0.01f, 400)][SerializeField] private float maxPercentTimeBetweenShots = 100.0f;

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
        [Range(1, 10000)][SerializeField] private int ammoCount = 200;
        #endregion

        #region Burst Fire
        /// <summary>
        /// Number of bullet bursts per fire action
        /// </summary>
        [Range(1, 20)][SerializeField] private int numBurstShots = 1;
        /// <summary>
        /// Time between each bullet burst
        /// </summary>
        [Range(0.01f, 10)][SerializeField] private float timeBetweenBurstShots = .01f;
        #endregion

        #region Multiple Projectiles
        /// <summary>
        /// Number of projectiles shot per gun shot
        /// </summary>
        [Range(1, 40)][SerializeField] private int projectilesReleasedPerShot = 1;
        /// <summary>
        /// Angle between each projectile shot
        /// </summary>
        [Range(0, 180)][SerializeField] private float distanceBetweenProjectiles = 0;
        /// <summary>
        /// Adds a random variation to each projectile's shot
        /// </summary>
        [Range(0, 180)][SerializeField] private float projectileSpread = 0;

        [Range(0, 100f)][SerializeField] private float deltaWindUpSpreadPerSecond = 0;
        [Range(0, 100f)][SerializeField] private float deltaWindDownSpreadPerSecond = 0;

        // TODO: make this the limit of change
        [Range(.001f, 400f)][SerializeField] private float maxChangeToNormalAccuracy = 100;
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

        #region DEBUG
        /// <summary>
        /// If the gun should print its state
        /// </summary>
        [SerializeField] private bool printDebugState = false;
        #endregion

        #region Bullet Specification
        /// <summary>
        /// Type of bullet to shoot
        /// </summary>
        [SerializeField] private BulletType bulletType;
        /// <summary>
        /// Number of enemies that bullet can penetrate before despawning
        /// </summary>
        [Range(0, 20)][SerializeField] private int bulletPenetration;

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

        #region Area Of Effect
        [Range(0, 1000)][SerializeField] private float damagePerSecond = 10;
        [SerializeField] private Gun.AOEPhases numPhases = Gun.AOEPhases.Persistant;
        [SerializeField] private AOEPhase phase1;
        [SerializeField] private AOEPhase phase2;
        #endregion
        #endregion

        #region Properties
        public bool IsTurret { get { return isTurret; } }
        public bool IsAutomatic { get { return isAutomatic; } }
        public BulletType BulletType { get { return bulletType; } }
        public bool IsPlayerGun { get { return isPlayerGun; } }

        #region Burst Fire
        public bool IsBurstFire { get { return numBurstShots > 1; } }
        public int NumBurstShots { get { return numBurstShots; } }
        public float TimeBetweenBurstShots { get { return timeBetweenBurstShots; } }
        public float TimeBetweenShots { get { return timeBetweenShots; } }
        public float DeltaWindupPercentTimeBetweenShots { get { return deltaWindupPercentTimeBetweenShots / 100f; } }
        public float DeltaWindDownPercentTimeBetweenShots { get { return deltaWindDownPercentTimeBetweenShots / 100f; } }
        public float MaxPercentTimeBetweenShots { get { return maxPercentTimeBetweenShots / 100f; } }

        #endregion

        #region Ammo
        public bool HasInfiniteAmmo { get { return hasInfiniteAmmo; } }
        public int AmmoCount { get { return ammoCount; } }
        #endregion

        #region Multiple Projectiles
        public int ProjectilesReleasedPerShot { get { return projectilesReleasedPerShot; } }
        public float DistanceBetweenProjectiles { get { return distanceBetweenProjectiles; } }
        public float ProjectileSpread { get { return projectileSpread; } }
        public float DeltaWindUpSpreadPerSecond { get { return deltaWindUpSpreadPerSecond / 100f; } }
        public float DeltaWindDownSpreadPerSecond { get { return deltaWindDownSpreadPerSecond / 100f; } }
        public float MaxAccuracyChange { get { return maxChangeToNormalAccuracy / 100f; } }
        #endregion

        #region Overheat
        public bool CanOverheat { get { return canOverheat; } }
        public float CoolDownPercentageBarrier { get { return coolDownPercentageBarrier; } }
        public float OverHeatPercentPerShot { get { return overHeatPercentPerShot; } }
        public float CoolDownPerSecond { get { return coolDownPerSecond; } }
        #endregion

        #region DEBUG
        public bool PrintDebugState { get { return printDebugState; } }
        #endregion

        #region Bullet Specification
        public float DamageDealt { get { return damageDealt; } }
        public int BulletPenetration { get { return bulletPenetration; } }
        #region Projectile
        public float MuzzleVelocity { get { return muzzleVelocity; } }
        public Vector3 ProjectileScale { get { return projectileScale; } }
        #endregion
        #region HitScan
        /// <summary>
        /// Maximum range the projectile travels
        /// </summary>
        public float Range { get { return range; } }
        #endregion
        public bool IsExplosive { get { return isExplosive; } }
        public bool IsCountDownExplosion { get { return isCountDownExplosion; } }
        #endregion

        #region Explosion
        public float Radius { get { return radius; } }
        public float Force { get { return force; } }
        public float ExplosionDamage { get { return explosionDamage; } }
        public float SecondsBeforeExplode { get { return secondsBeforeExplode; } }
        #endregion


        public bool IsHitScan { get { return BulletType == EditorObject.BulletType.HitScan; } }
        public bool IsBulletProjectile { get { return BulletType == EditorObject.BulletType.BulletProjectile; } }

        #region Area of Effect
        public bool IsAreaOfEffect { get { return BulletType == EditorObject.BulletType.AreaOfEffect; } }
        public float DamagePerSecond { get { return damagePerSecond; } }
        public Gun.AOEPhases NumPhases { get { return numPhases; } }
        public AOEPhase Phase1 { get { return phase1; } }
        public AOEPhase Phase2 { get { return phase2; } }
        #endregion

        #endregion
    }
}