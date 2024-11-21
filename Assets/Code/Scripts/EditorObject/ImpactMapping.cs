using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorObject
{
    /// <summary>
    /// Maps a material to a particleSystem to emit when an object's hitbox 
    /// which shares the material is hit.
    /// </summary>
    [CreateAssetMenu(menuName = "EditorObject/ImpactMapping", fileName = "New Impact Mapping")]
    public class ImpactMapping : ScriptableObject
    {
        [SerializeField] private Material material;
        [SerializeField] private PooledParticle particleSystem;
        [SerializeField] private int pooledNumber = 10;

        public Material Material { get { return material; } }
        public PooledParticle ParticleSystem { get {  return particleSystem; } }
        public int PooledNumber { get {  return pooledNumber; } }
    }
}
