using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    public enum BulletType
    {
        Projectile,
        HitScan,
    }

    [CreateAssetMenu(menuName = "EditorObject/GunStats", fileName = "New GunStats")]
    public class GunStats : ScriptableObject
    {
        [SerializeField] private bool isPlayerGun = true;
        [SerializeField] private bool isTurret = false;
        [SerializeField] private bool isAutomatic = true;
        [Range(0.01f, 20f)] [SerializeField] private float timeBetweenShots = 10f;
        [Range(0, 1000)] [SerializeField] private float damageDealt = 70;

        #region Ammo
        [SerializeField] private bool infiniteAmmo = true;
        [Range(1, 10000)] [SerializeField] private int ammoCount = 200;
        #endregion

        #region Burst Fire
        [Range(1, 20)] [SerializeField] private int numBurstShots = 1;
        [Range(0.01f, 20f)] [SerializeField] private float timeBetweenBurstShots = .01f; // TODO: Enforce positive
        #endregion

        #region Multiple Projectiles
        [Range(1, 1000)] [SerializeField] private int projectileCountPerShot = 1;
        [Range(0, 180)] [SerializeField] private float angleBetweenProjectiles = 90;
        [Range(0, 180)] [SerializeField] private float randomAngleVariationPerProjectile = 0;
        #endregion

        #region Overheat
        [SerializeField] private bool canOverheat = false;
        [Range(0, 100)] [SerializeField] private float overHeatBarrier = 50;
        [Range(1, 99)] [SerializeField] private float overHeatPercentPerShot = 5;
        [Range(0, 100)] [SerializeField] private float coolDownPerSecond = 2.5f;
        #endregion

        #region DEBUG
        [SerializeField] private bool printDebugState = false;
        #endregion

        #region Bullet Specification
        [SerializeField] private BulletType bulletType;

        #region Projectile
        [Range(0f, 100f)] [SerializeField] private float muzzleVelocity = 60;
        #endregion

        #region HitScan
        [Range(0f, 200f)] [SerializeField] private float range = 100f;
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
        #endregion

        #region Ammo
        public bool InfiniteAmmo { get { return infiniteAmmo; } }
        public int AmmoCount { get { return ammoCount; } }
        #endregion

        #region Multiple Projectiles
        public int ProjectileCountPerShot { get { return projectileCountPerShot; } }
        public float AngleBetweenProjectiles { get { return angleBetweenProjectiles; } }
        public float RandomAngleVariationPerProjectile { get { return randomAngleVariationPerProjectile; } }
        #endregion

        #region Overheat
        public bool CanOverheat { get { return canOverheat; } }
        public float OverheatBarrier { get { return overHeatBarrier; } }
        public float OverHeatPercentPerShot { get { return overHeatPercentPerShot; } }
        public float CoolDownPerSecond { get { return coolDownPerSecond; } }
        #endregion

        #region DEBUG
        public bool PrintDebugState { get { return printDebugState; } }
        #endregion

        #region Bullet Specification
        public float DamageDealt { get { return damageDealt; } }
        #region Projectile
        public float MuzzleVelocity { get { return muzzleVelocity; } }
        #endregion
        #region HitScan
        public float Range { get { return range; } }
        #endregion
        #endregion

        #endregion


        public void BurstShotsOff()
        {
            numBurstShots = 1;
        }
    }
}