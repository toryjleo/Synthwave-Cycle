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
        [SerializeField] private BulletType bulletType;
        [SerializeField] private bool playerBullet = true;
        [SerializeField] private float fireRate = 10f;

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
        public BulletType BulletType { get { return bulletType; } }
        public bool PlayerBullet { get { return playerBullet; } }
        public float FireRate { get { return fireRate;  } }
        public bool InfiniteAmmo { get { return infiniteAmmo; } }
        public int MagazineSize { get { return magazineSize; } }

        public float DamageDealt {  get { return damageDealt; } }
        public float MuzzleVelocity {  get { return muzzleVelocity; } }

        public float Range { get { return range; } }

        #endregion
    }
}