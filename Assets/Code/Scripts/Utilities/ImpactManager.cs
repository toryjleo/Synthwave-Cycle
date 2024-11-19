using EditorObject;
using Generic;
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
            // TODO: Check if material already exists. If so, throw error

            // TODO: Initialize objectpool of type
            impactDictionary.Add(mapping.Material, new ObjectPool(null, mapping.ParticleSystem));
            impactDictionary[mapping.Material].PoolObjects(defaultBuffer);
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
        }
        else 
        {
            Debug.Log("Not a material");
        }
    }

}
