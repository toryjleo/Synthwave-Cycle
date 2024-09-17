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
        [SerializeField] private float timeBetweenShots = 10f;
        [SerializeField] private float damageDealt = 70;

        #region Ammo
        [SerializeField] private bool infiniteAmmo = true;
        [SerializeField] private int ammoCount = 200;
        #endregion

        #region Burst Fire
        [SerializeField] private bool isBurstFire = false;
        [SerializeField] private int numBurstShots = 2;
        [SerializeField] private float timeBetweenBurstShots = .01f; // TODO: Enforce positive
        #endregion

        #region Multiple Projectiles
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

        #region DEBUG
        [SerializeField] private bool printDebugState = false;
        #endregion

        #region Bullet Specification
        [SerializeField] private BulletType bulletType;

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

        #region Burst Fire
        public bool IsBurstFire { get { return isBurstFire; } }
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

        public void OnValidate()
        {
            numBurstShots = Mathf.Clamp(numBurstShots, 0, int.MaxValue);
            timeBetweenBurstShots = Mathf.Clamp(timeBetweenBurstShots, 0, float.MaxValue);
            timeBetweenShots = Mathf.Clamp(timeBetweenShots, 0, float.MaxValue);
            ammoCount = Mathf.Clamp(ammoCount, 0, int.MaxValue);
            projectileCountPerShot = Mathf.Clamp(projectileCountPerShot, 0, int.MaxValue);
            angleBetweenProjectiles = Mathf.Clamp(angleBetweenProjectiles, 0, 180);
            randomAngleVariationPerProjectile = Mathf.Clamp(randomAngleVariationPerProjectile, 0, 180);

            overHeatBarrier = Mathf.Clamp(overHeatBarrier, 0, 100);
            overHeatPercentPerShot = Mathf.Clamp(overHeatPercentPerShot, 1, 99);
            coolDownPerSecond = Mathf.Clamp(coolDownPerSecond, 0, 100);

            damageDealt = Mathf.Clamp(damageDealt, 1, float.MaxValue);
            muzzleVelocity = Mathf.Clamp(muzzleVelocity,0, float.MaxValue);
            range = Mathf.Clamp(range, 0, float.MaxValue);
        }

        public void BurstShotsOff()
        {
            numBurstShots = 1;
        }

        public void ValidateBurstShots() 
        {
        
        }
    }
}