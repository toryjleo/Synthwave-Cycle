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
        [SerializeField] private BulletType bulletType;
        [SerializeField] private float fireRate = 10f;

        #region Raycast
        [SerializeField] private float range = 100f;
        #endregion

        #region Properties
        public BulletType BulletType { get { return bulletType; } }
        public float FireRate { get { return fireRate;  } }

        public float Range { get { return range; } }

        #endregion
    }
}