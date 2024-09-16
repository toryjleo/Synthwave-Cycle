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
        [SerializeField] private int magazineSize = 200;


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
        public int MagazineSize { get { return magazineSize; } }

        public float DamageDealt {  get { return damageDealt; } }
        public float MuzzleVelocity {  get { return muzzleVelocity; } }

        public float Range { get { return range; } }

        #endregion
    }
}