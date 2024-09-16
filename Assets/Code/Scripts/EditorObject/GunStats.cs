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
        [SerializeField] private bool isTurret = false;
        [SerializeField] private bool isAutomatic = true;
        [SerializeField] private BulletType bulletType;
        [SerializeField] private bool isPlayerGun = true;
        [SerializeField] private bool isBurstFire = false;
        [SerializeField] private int numBurstShots = 2; // TODO: Enforce 1 burst shot when burstFire off
        [SerializeField] private float timeBetweenBurstShots = .01f; // TODO: Enforce positive
        [SerializeField] private float timeBetweenShots = 10f;

        [SerializeField] private bool infiniteAmmo = true;
        [SerializeField] private int ammoCount = 200;

        #region Mutliple Projectiles
        [SerializeField] private int projectileCountPerShot = 1;
        [SerializeField] private float angleBetweenProjectiles = 90;
        [SerializeField] private float randomAngleVariationPerProjectile = 0;
        #endregion

        #region Overheat
        [SerializeField] private bool canOverheat = false;
        [SerializeField] private float overHeatBarrier = 50;
        [SerializeField] private float overHeatPercentPerShot = 5;
        [SerializeField] private float coolDownPerSecond = 2.5f;
        #endregion
        [SerializeField] private bool printDebugState = false;

        #region Bullet
        [SerializeField] private float damageDealt = 70;

        #region Projectile
        [SerializeField] private float muzzleVelocity = 60;
        #endregion

        #region HitScan
        [SerializeField] private float range = 100f;
        #endregion
        #endregion

        #region Properties
        public bool IsTurret { get { return isTurret; } }
        public bool IsAutomatic { get { return isAutomatic; } }
        public BulletType BulletType { get { return bulletType; } }
        public bool IsPlayerGun { get { return isPlayerGun; } }

        public bool IsBurstFire { get { return isBurstFire; } }
        public int NumBurstShots { get { return numBurstShots; } }
        public float TimeBetweenBurstShots { get { return timeBetweenBurstShots; } }


        public float TimeBetweenShots { get { return timeBetweenShots;  } }
        public bool InfiniteAmmo { get { return infiniteAmmo; } }
        public int AmmoCount { get { return ammoCount; } }

        public int ProjectileCountPerShot { get { return projectileCountPerShot; } }
        public float AngleBetweenProjectiles {  get { return angleBetweenProjectiles; } }
        public float RandomAngleVariationPerProjectile { get { return randomAngleVariationPerProjectile; } }

        public bool CanOverheat { get { return canOverheat; } }
        public float OverheatBarrier { get { return overHeatBarrier; } }
        public float OverHeatPercentPerShot { get {  return overHeatPercentPerShot; } }
        public float CoolDownPerSecond { get { return coolDownPerSecond; } }

        public bool PrintDebugState {  get { return printDebugState; } }
        public float DamageDealt {  get { return damageDealt; } }
        public float MuzzleVelocity {  get { return muzzleVelocity; } }

        public float Range { get { return range; } }

        #endregion
    }
}