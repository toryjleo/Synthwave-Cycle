using EditorObject;
using Generic;
using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactManager : MonoBehaviour
{

    #region Instancing
    private static ImpactManager instance;
    public static ImpactManager Instance { get { return instance; } private set { instance = value; } }
    #endregion


    [SerializeField] private List<ImpactMapping> mappings = new List<ImpactMapping>();
    [SerializeField] PooledParticle errorParticle = null;
    private Dictionary<Material, ObjectPool> impactDictionary = new Dictionary<Material, ObjectPool>();
    private int defaultBuffer = 5;

    void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("Multiple instances of ImpactManager. Destroying this one: " + this.name);
            Destroy(this.gameObject);
        }
        Instance = this;

        foreach(ImpactMapping mapping in mappings) 
        {
            if (impactDictionary.ContainsKey(mapping.Material))
            {
                // Error case
                Debug.LogWarning("Initializing multiple mappings for material: " +  mapping.Material);
            }
            else
            {
                impactDictionary.Add(mapping.Material, new ObjectPool(null, mapping.ParticleSystem));
                impactDictionary[mapping.Material].PoolObjects(defaultBuffer);
            }
        }

        // Initialize the error particle
        if (errorParticle == null) 
        {
            Debug.Log("ImpactManager requires an error particle to be set");
        }
        else 
        {
            impactDictionary[null] = new ObjectPool(null, errorParticle);
            impactDictionary[null].PoolObjects(defaultBuffer);
        }
    }

    // TODO: Reset all pools
    public void Reset() 
    {
        // impactEffectPool.ResetGameObject();
    }

    public void SpawnBulletImpact(Vector3 position, Vector3 forward, Material material) 
    {
        if (impactDictionary.ContainsKey(material)) 
        {
            Debug.Log("Hit thing");
            PooledParticle particle = impactDictionary[material].SpawnFromPool() as PooledParticle;
            particle.Play(position, forward);
        }
        else 
        {
            PooledParticle particle = impactDictionary[null].SpawnFromPool() as PooledParticle;
            particle.Play(position, forward);
            Debug.Log("Not a material");
        }
    }

}
