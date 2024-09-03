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
        // TODO: Add type of weilded
        // TODO: add range

        public BulletType BulletType { get { return bulletType; } }
    }
}