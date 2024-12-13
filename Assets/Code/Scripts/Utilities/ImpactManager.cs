using EditorObject;
using Generic;
using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

/// <summary>
/// A singleton that manages all particles played in the game.
/// It maps every material to a particle to play when it is hit
/// </summary>
public class ImpactManager : MonoBehaviour
{
    #region Instancing
    private static ImpactManager instance;
    public static ImpactManager Instance { get { return instance; } private set { instance = value; } }
    #endregion


    [SerializeField] private List<ImpactMapping> mappings = new List<ImpactMapping>();
    private Dictionary<Material, ObjectPool> impactDictionary = new Dictionary<Material, ObjectPool>();

    [SerializeField] PooledParticle errorParticle = null;
    private ObjectPool errorPool = null;
    private int defaultPinkErrorNumber = 12;

    Material groundMaterial = null;

    private void Awake()
    {
        if (instance == null) 
        {
            Init();
        }
    }

    public void Init()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        PoolParticles();
    }

    public void Reset() 
    {
        errorPool.ResetGameObject();

        foreach (Material key in impactDictionary.Keys) 
        {
            impactDictionary[key].ResetGameObject();
        }
    }


    /// <summary>
    /// Pools the particles to be used
    /// </summary>
    private void PoolParticles()
    {
        foreach (ImpactMapping mapping in mappings)
        {
            AddMapping(mapping);
        }

        // Initialize the error particle
        if (errorParticle == null)
        {
            Debug.Log("ImpactManager requires an error particle to be set");
        }
        else
        {
            errorPool = new ObjectPool(null, errorParticle);
            errorPool.PoolObjects(defaultPinkErrorNumber);
        }
    }

    public void AddMapping(ImpactMapping mapping) 
    {
        if (impactDictionary.ContainsKey(mapping.Material))
        {
            // Error case
            Debug.LogWarning("Initializing multiple mappings for material: " + mapping.Material);
        }
        else
        {
            impactDictionary.Add(mapping.Material, new ObjectPool(null, mapping.ParticleSystem));
            impactDictionary[mapping.Material].PoolObjects(mapping.PooledNumber);
        }
    }

    public void AddGroundMapping(ImpactMapping mapping)
    {
        AddMapping(mapping);
        groundMaterial = mapping.Material;
    }

    public void SpawnHitScanBulletMiss(Vector3 position) 
    {
        SpawnBulletImpact(position, Vector3.up, groundMaterial);
    }

    /// <summary>
    /// Spawns a particle at the specified orientation.
    /// Will play a pink error particle for any material without a mapping.
    /// </summary>
    /// <param name="position">Location for particle to spawn</param>
    /// <param name="forward">Forward direction for the particle</param>
    /// <param name="material">Material that the particle is mapped to</param>
    public void SpawnBulletImpact(Vector3 position, Vector3 forward, Material material) 
    {
        PooledParticle particle = null;

        if (material != null && impactDictionary.ContainsKey(material)) 
        {
            particle = impactDictionary[material].SpawnFromPool() as PooledParticle;
        }
        else 
        {
            // Debug.Log("Hitting: " + material.ToString());
            particle = errorPool.SpawnFromPool() as PooledParticle;
            
        }

        particle.Play(position, forward);
    }

}
